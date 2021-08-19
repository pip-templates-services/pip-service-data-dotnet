using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PipServices3.Commons.Data;
using PipServices3.SqlServer.Persistence;
using PipTemplatesServiceData.Data.Version1;

namespace PipTemplatesServiceData.Persistence
{
    public class EntitiesJsonSqlServerPersistence : IdentifiableJsonSqlServerPersistence<EntityV1, string>, IEntitiesPersistence
    {

        public EntitiesJsonSqlServerPersistence(): base("entities_json") { }

        protected override void DefineSchema()
        {
            this.ClearSchema();
            this.EnsureTable();
            this.EnsureSchema("ALTER TABLE [entities_json] ADD [data_key] AS JSON_VALUE([data],'$.id')");
            this.EnsureIndex("entities_json_key", new Dictionary<string, bool>{ { "data_key", true } }, new IndexOptions{ Unique=true});
        }

        private string ComposeFilter(FilterParams filter)
        {
            filter = filter ?? new FilterParams();

            var filters = new List<string>();

            var id = filter.GetAsNullableString("id");
            if (id != null)
                filters.Add("JSON_VALUE([data],'$.id')='" + id + "'");

            var siteId = filter.GetAsNullableString("site_id");
            if (siteId != null)
                filters.Add("JSON_VALUE([data],'$.site_id')='" + siteId + "'");

            var name = filter.GetAsNullableString("name");
            if (name != null)
                filters.Add("JSON_VALUE([data],'$.name')='" + name + "'");

            var tempNames = filter.GetAsString("names");
            var names = tempNames != null ? tempNames.Split(',') : null;
            if (names != null)
                filters.Add("JSON_VALUE([data],'$.name') IN ('" + string.Join("','", names) + "')");

            return filters.Count > 0 ? string.Join(" AND ", filters.ToArray()) : null;
        }

        public async Task<EntityV1> GetOneByNameAsync(string correlationId, string name)
        {
            var @params = new[] { name };
            var query = "SELECT * FROM " + QuoteIdentifier(_tableName) + " WHERE JSON_VALUE([data],'$.name') = @Param1";

            var result = (await ExecuteReaderAsync(query, @params)).FirstOrDefault();

            if (result == null)
            {
                _logger.Trace(correlationId, "Nothing found from {0} with name = {1}", _tableName, name);
                return default;
            }

            _logger.Trace(correlationId, "Retrieved from {0} with name = {1}", _tableName, name);

            var item = ConvertToPublic(result);

            return item;
        }

        public async Task<DataPage<EntityV1>> GetPageByFilterAsync(string correlationId, FilterParams filter, PagingParams paging)
        {
            return await base.GetPageByFilterAsync(correlationId, this.ComposeFilter(filter), paging, "id", null);
        }
    }
}

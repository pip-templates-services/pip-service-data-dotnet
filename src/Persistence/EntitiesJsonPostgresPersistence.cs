using PipServices3.Commons.Data;
using PipServices3.Postgres.Persistence;
using PipTemplatesServiceData.Data.Version1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipTemplatesServiceData.Persistence
{
    public class EntitiesJsonPostgresPersistence : IdentifiableJsonPostgresPersistence<EntityV1, string>, IEntitiesPersistence
    {

        public EntitiesJsonPostgresPersistence(): base("entities_json")
        {
            _maxPageSize = 1000;
        }

        protected override void DefineSchema()
        {
            EnsureTable("VARCHAR(32)", "JSONB");
            EnsureIndex("entities_json_site_id", new Dictionary<string, bool> { { "(data->>'site_id')", true } }, new IndexOptions());
        }

        private string ComposeFilter(FilterParams filter)
        {
            filter = filter ?? new FilterParams();

            var filters = new List<string>();

            var id = filter.GetAsNullableString("id");
            if (id != null)
                filters.Add("data->>'id'='" + id + "'");

            var siteId = filter.GetAsNullableString("site_id");
            if (siteId != null)
                filters.Add("data->>'site_id'='" + siteId + "'");

            var name = filter.GetAsNullableString("name");
            if (name != null)
                filters.Add("data->>'name'='" + name + "'");

            var tempNames = filter.GetAsString("names");
            var names = tempNames != null ? tempNames.Split(',') : null;
            if (names != null)
                filters.Add("data->>'name' IN ('" + string.Join("','", names) + "')");

            return filters.Count > 0 ? string.Join(" AND ", filters.ToArray()) : null;
        }

        public async Task<EntityV1> GetOneByNameAsync(string correlationId, string name)
        {
            var @params = new[] { name };
            var query = "SELECT * FROM " + QuoteIdentifier(_tableName) + " WHERE data->>'name'=@Param1";

            var result = (await ExecuteReaderAsync(correlationId, query, @params)).FirstOrDefault();

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

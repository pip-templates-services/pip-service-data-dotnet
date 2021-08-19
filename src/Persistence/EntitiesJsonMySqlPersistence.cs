using PipServices3.Commons.Data;
using PipServices3.MySql.Persistence;
using PipTemplatesServiceData.Data.Version1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipTemplatesServiceData.Persistence
{
    public class EntitiesJsonMySqlPersistence : IdentifiableJsonMySqlPersistence<EntityV1, string>, IEntitiesPersistence
    {
        public EntitiesJsonMySqlPersistence() : base("entities_json") { }

        protected override void DefineSchema()
        {
            this.ClearSchema();
            this.EnsureTable();
            this.EnsureSchema("ALTER TABLE `entities_json` ADD `data_id` VARCHAR(50) AS (JSON_UNQUOTE(`data`->\"$.id\"))");
            this.EnsureIndex("entities_json_id", new Dictionary<string, bool> { { "data_id", true } }, new IndexOptions { Unique = true });

        }

        private string ComposeFilter(FilterParams filter)
        {
            filter ??= new FilterParams();

            var filters = new List<string>();

            var id = filter.GetAsNullableString("id");
            if (id != null)
                filters.Add("data->'$.id'='" + id + "'");

            var siteId = filter.GetAsNullableString("site_id");
            if (siteId != null)
                filters.Add("data->'$.site_id'='" + siteId + "'");

            var name = filter.GetAsNullableString("name");
            if (name != null)
                filters.Add("data->'$.name'='" + name + "'");

            var tempNames = filter.GetAsNullableString("names");
            if (tempNames != null)
            {
                var names = tempNames.Split(',');
                filters.Add("data->'$.name' IN ('" + string.Join("','", names) + "')");
            }

            return filters.Count > 0 ? string.Join(" AND ", filters) : null;
        }

        public async Task<EntityV1> GetOneByNameAsync(string correlationId, string name)
        {
            var query = "SELECT * FROM " + this.QuoteIdentifier(this._tableName) + " WHERE data->'$.name' = '" + name + "'";

            var result = (await ExecuteReaderAsync(query)).FirstOrDefault();

            if (result == null)
                this._logger.Trace(correlationId, "Cannot find entity with name=%s", this._tableName, name);
            else
                this._logger.Trace(correlationId, "Found entity with name=%s", this._tableName, name);

            var item = ConvertToPublic(result);
            return item;
        }

        public async Task<DataPage<EntityV1>> GetPageByFilterAsync(string correlationId, FilterParams filter, PagingParams paging)
        {
            return await base.GetPageByFilterAsync(correlationId, this.ComposeFilter(filter), paging, "id", null);
        }
    }
}

using PipServices3.Commons.Convert;
using PipServices3.Commons.Data;
using PipServices3.MySql.Persistence;
using PipTemplatesServiceData.Data.Version1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipTemplatesServiceData.Persistence
{
    public class EntitiesMySqlPersistence : IdentifiableMySqlPersistence<EntityV1, string>, IEntitiesPersistence
    {
        public EntitiesMySqlPersistence() : base("entities") { }

        protected override void DefineSchema()
        {
            this.ClearSchema();
            this.EnsureSchema("CREATE TABLE entities (id VARCHAR(32), site_id VARCHAR(32), type VARCHAR(15), name VARCHAR(50), content VARCHAR(150))");
            this.EnsureIndex("entities_site_id", new Dictionary<string, bool> { { "site_id", true } }, new IndexOptions());
        }

        private string ComposeFilter(FilterParams filter)
        {
            filter ??= new FilterParams();

            var filters = new List<string>();

            var id = filter.GetAsNullableString("id");
            if (id != null)
                filters.Add("id='" + id + "'");

            var siteId = filter.GetAsNullableString("site_id");
            if (siteId != null)
                filters.Add("site_id='" + siteId + "'");

            var tempIds = filter.GetAsNullableString("ids");
            if (tempIds != null)
            {
                var ids = tempIds.Split(",");
                filters.Add("id IN ('" + string.Join("','", ids) + "')");
            }

            var name = filter.GetAsNullableString("name");
            if (name != null)
                filters.Add("name='" + name + "'");

            var tempNames = filter.GetAsNullableString("names");
            if (tempNames != null)
            {
                var names = tempNames.Split(',');
                filters.Add("name IN ('" + string.Join("','", names) + "')");
            }

            return filters.Count > 0 ? string.Join(" AND ", filters) : null;
        }

        public async Task<EntityV1> GetOneByNameAsync(string correlationId, string name)
        {
            var query = "SELECT * FROM " + this.QuoteIdentifier(this._tableName) + " WHERE name='" + name + "'";

            var result = await ExecuteReaderAsync(query);

            var item = result != null && result[0] != null ? result[0] : null;

            if (item == null)
                this._logger.Trace(correlationId, "Nothing found from %s with name = %s", this._tableName, name);
            else
                this._logger.Trace(correlationId, "Retrieved from %s with name = %s", this._tableName, name);

            return ConvertToPublic(item);
        }

        public async Task<DataPage<EntityV1>> GetPageByFilterAsync(string correlationId, FilterParams filter, PagingParams paging)
        {
            return await base.GetPageByFilterAsync(correlationId, this.ComposeFilter(filter), paging, "id", null);

        }
    }
}

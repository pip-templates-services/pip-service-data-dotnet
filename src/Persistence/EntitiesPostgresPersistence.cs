using PipServices3.Commons.Data;
using PipServices3.Postgres.Persistence;
using PipTemplatesServiceData.Data.Version1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipTemplatesServiceData.Persistence
{
    public class EntitiesPostgresPersistence : IdentifiablePostgresPersistence<EntityV1, string>, IEntitiesPersistence
    {
        public EntitiesPostgresPersistence() : base("entities")
        {
            _maxPageSize = 1000;
        }

        protected override void DefineSchema()
        {
            this.ClearSchema();
            this.EnsureSchema("CREATE TABLE entities (id VARCHAR(32), site_id VARCHAR(32), type VARCHAR(15), name VARCHAR(25), content VARCHAR(150))");
            this.EnsureIndex("entities_site_id", new Dictionary<string, bool>{ { "site_id", true } }, new IndexOptions { Unique = true });
        }

        private string ComposeFilter(FilterParams filter)
        {
            filter ??= new FilterParams();
            var key = filter.GetAsNullableString("key");

            var filterCondition = new List<string>();

            var id = filter.GetAsNullableString("id");
            if (id != null)
                filterCondition.Add("id='" + id + "'");

            var siteId = filter.GetAsNullableString("site_id");
            if (siteId != null)
                filterCondition.Add("site_id='" + siteId + "'");

            var tempIds = filter.GetAsNullableString("ids");
            if (tempIds != null)
            {
                var ids = tempIds.Split(",");
                filterCondition.Add("name='" + ids + "'");
            }

            var name = filter.GetAsNullableString("name");
            if (name != null)
                filterCondition.Add("name='" + name + "'");

            var tempNames = filter.GetAsNullableString("names");
            if (tempNames != null)
            {
                var names = tempNames.Split(',');
                filterCondition.Add("name IN ('" + string.Join("','", names) + "')");
            }

            return filterCondition.Count > 0 ? string.Join(" AND ", filterCondition) : null;
        }

        public async Task<EntityV1> GetOneByNameAsync(string correlationId, string name)
        {
            var query = "SELECT * FROM " + this.QuoteIdentifier(this._tableName) + " WHERE \"name\"=@Param1";
            var parameters = new List<string>() { name };

            var result = await ExecuteReaderAsync(correlationId, query, parameters);

            var item = result != null && result.Count > 0 ? result[0] ?? null : null;

            if (item == null)
                this._logger.Trace(correlationId, "Cannot find entity with name=%s", this._tableName, name);
            else
                this._logger.Trace(correlationId, "Found entity with name=%s", this._tableName, name);

            var newItem = this.ConvertToPublic(item);

            return newItem;
        }

        public async Task<DataPage<EntityV1>> GetPageByFilterAsync(string correlationId, FilterParams filter, PagingParams paging)
        {
            return await base.GetPageByFilterAsync(correlationId, this.ComposeFilter(filter), paging, "id", null);
        }
    }
}

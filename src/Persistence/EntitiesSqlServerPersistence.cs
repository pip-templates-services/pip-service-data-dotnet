using PipServices3.Commons.Data;
using PipServices3.SqlServer.Persistence;
using PipTemplatesServiceData.Data.Version1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipTemplatesServiceData.Persistence
{
    public class EntitiesSqlServerPersistence : IdentifiableSqlServerPersistence<EntityV1, string>, IEntitiesPersistence
    {
        public EntitiesSqlServerPersistence() : base("entities") { }

        protected override void DefineSchema()
        {
            this.ClearSchema();
            this.EnsureSchema("CREATE TABLE [entities] ([id] VARCHAR(32), [site_id] VARCHAR(32), [type] VARCHAR(15), [name] VARCHAR(50), [content] nvarchar(max))");
            this.EnsureIndex("entities_site_id", new Dictionary<string, bool> { { "site_id", true } }, new IndexOptions { Unique = false });
        }

        private string ComposeFilter(FilterParams filter)
        {
            filter ??= new FilterParams();

            var filterCondition = new List<string>();

            var id = filter.GetAsNullableString("id");
            if (id != null)
                filterCondition.Add("[id]='" + id + "'");

            var siteId = filter.GetAsNullableString("site_id");
            if (siteId != null)
                filterCondition.Add("[site_id]='" + siteId + "'");

            var tempIds = filter.GetAsNullableString("ids");
            if (tempIds != null)
            {
                var ids = tempIds.Split(",");
                filterCondition.Add("[id] IN ('" + string.Join("','", ids) + "')");
            }

            var name = filter.GetAsNullableString("name");
            if (name != null)
                filterCondition.Add("[name]='" + name + "'");

            var tempNames = filter.GetAsNullableString("names");
            if (tempNames != null)
            {
                var names = tempNames.Split(',');
                filterCondition.Add("[name] IN ('" + string.Join("','", names) + "')");
            }

            return filterCondition.Count > 0 ? string.Join(" AND ", filterCondition) : null;
        }

        public async Task<DataPage<EntityV1>> GetPageByFilterAsync(string correlationId, FilterParams filter, PagingParams paging)
        {
            return await base.GetPageByFilterAsync(correlationId, this.ComposeFilter(filter), paging, "id", null);
        }

        public async Task<EntityV1> GetOneByNameAsync(string correlationId, string name)
        {
            var query = "SELECT * FROM " + this.QuoteIdentifier(this._tableName) + " WHERE [name]=@Param1";
            var parameters = new List<string>() { name };

            var result = await ExecuteReaderAsync(query, parameters);

            var newItem = result != null && result.Count == 1
                ? ConvertToPublic(result[0]) : default;

            if (newItem == null)
                this._logger.Trace(correlationId, "Nothing found from %s with name = %s", this._tableName, name);
            else
                this._logger.Trace(correlationId, "Retrieved from %s with name = %s", this._tableName, name);

            return newItem;
        }
    }
}

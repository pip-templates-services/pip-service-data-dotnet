using MongoDB.Driver;
using PipServices3.Commons.Data;
using PipServices3.MongoDb.Persistence;
using PipTemplatesServiceData.Data.Version1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipTemplatesServiceData.Persistence
{
    public class EntitiesMongoDbPersistence : IdentifiableMongoDbPersistence<EntityV1, string>, IEntitiesPersistence
    {
        public EntitiesMongoDbPersistence() : base("entities")
        {
            _maxPageSize = 1000;
        }

        private new FilterDefinition<EntityV1> ComposeFilter(FilterParams filterParams)
        {
            filterParams = filterParams ?? new FilterParams();

            var builder = Builders<EntityV1>.Filter;
            var filter = builder.Empty;

            var id = filterParams.GetAsNullableString("id");
            if (!string.IsNullOrEmpty(id))
                filter &= builder.Eq(b => b.Id, id);

            var siteId = filterParams.GetAsNullableString("site_id");
            if (!string.IsNullOrEmpty(siteId))
                filter &= builder.Eq(b => b.SiteId, siteId);

            var name = filterParams.GetAsNullableString("name");
            if (!string.IsNullOrEmpty(name))
                filter &= builder.Eq(b => b.Name, name);

            var names = filterParams.GetAsNullableString("names");
            var nameList = !string.IsNullOrEmpty(names) ? names.Split(',') : null;
            if (nameList != null)
                filter &= builder.In(b => b.Name, nameList);

            return filter;
        }

        public async Task<EntityV1> GetOneByNameAsync(string correlationId, string name)
        {
            var builder = Builders<EntityV1>.Filter;
            var filter = builder.Eq(x => x.Name, name);
            var result = await _collection.Find(filter).FirstOrDefaultAsync();

            if (result != null)
                _logger.Trace(correlationId, "Retrieved from {0} with name = {1}", _collectionName, name);
            else
                _logger.Trace(correlationId, "Nothing found from {0} with name = {1}", _collectionName, name);

            return result;
        }

        public async Task<DataPage<EntityV1>> GetPageByFilterAsync(string correlationId, FilterParams filter, PagingParams paging)
        {
            return await base.GetPageByFilterAsync(correlationId, this.ComposeFilter(filter), paging);
        }
    }
}

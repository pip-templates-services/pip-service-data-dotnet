using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using PipServices3.Commons.Data;
using PipServices3.Data.Persistence;
using PipTemplatesServiceData.Data.Version1;

namespace PipTemplatesServiceData.Persistence
{
    public class EntitiesMemoryPersistence : IdentifiableMemoryPersistence<EntityV1, string>, IEntitiesPersistence
    {
        public EntitiesMemoryPersistence() : base()
        {
            _maxPageSize = 1000;
        }

        private List<Func<EntityV1, bool>> ComposeFilter(FilterParams filter)
        {
            filter ??= new FilterParams();

            var id = filter.GetAsNullableString("id");
            var siteId = filter.GetAsNullableString("site_id");
            var name = filter.GetAsNullableString("name");
            var names = filter.GetAsNullableString("names");
            var namesList = names != null ? names.Split(',') : null;

            return new List<Func<EntityV1, bool>>()
            {
                (item) =>
                {
                    if (id != null && item.Id != id)
                        return false;
                    if (siteId != null && item.SiteId != siteId)
                        return false;
                    if (name != null && item.Name != name)
                        return false;
                    if (names != null && names.IndexOf(item.Name) < 0)
                        return false;
                    return true;
                }
            };
        }

        public async Task<EntityV1> GetOneByNameAsync(string correlationId, string name)
        {
            EntityV1 item = null;

            lock (_lock)
            {
                item = _items.Find((entity) => { return entity.Name == name; });
            }

            if (item != null) _logger.Trace(correlationId, "Found entity by {0}", name);
            else _logger.Trace(correlationId, "Cannot find entity by {0}", name);

            return await Task.FromResult(item);
        }

        public async Task<DataPage<EntityV1>> GetPageByFilterAsync(string correlationId, FilterParams filter, PagingParams paging)
        {
            return await base.GetPageByFilterAsync(correlationId, this.ComposeFilter(filter), paging);
        }
    }
}

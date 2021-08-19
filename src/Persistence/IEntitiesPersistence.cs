
using System.Threading.Tasks;

using PipServices3.Commons.Data;
using PipTemplatesServiceData.Data.Version1;

namespace PipTemplatesServiceData.Persistence
{
    public interface IEntitiesPersistence
    {
        Task<DataPage<EntityV1>> GetPageByFilterAsync(string correlationId, FilterParams filter, PagingParams paging);
        Task<EntityV1> GetOneByIdAsync(string correlationId, string id);
        Task<EntityV1> GetOneByNameAsync(string correlationId, string name);
        Task<EntityV1> CreateAsync(string correlationId, EntityV1 item);
        Task<EntityV1> UpdateAsync(string correlationId, EntityV1 item);
        Task<EntityV1> DeleteByIdAsync(string correlationId, string id);
    }
}

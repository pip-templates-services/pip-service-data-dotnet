using PipServices3.Commons.Commands;
using PipServices3.Commons.Config;
using PipServices3.Commons.Data;
using PipServices3.Commons.Refer;
using PipTemplatesServiceData.Data.Version1;
using PipTemplatesServiceData.Persistence;


using System.Threading.Tasks;

namespace PipTemplatesServiceData.Logic
{
    public class EntitiesController : IEntitiesController, IConfigurable, IReferenceable, ICommandable
    {

        public EntitiesController() 
        { }

        private IEntitiesPersistence _persistence;
        private EntitiesCommandSet _commandSet;

        public void Configure(ConfigParams config) { }

        public void SetReferences(IReferences references)
        {
            this._persistence = references.GetOneRequired<IEntitiesPersistence>(
                new Descriptor("pip-service-data", "persistence", "*", "*", "1.0")
            );
        }

        public CommandSet GetCommandSet()
        {
            if (this._commandSet == null)
                this._commandSet = new EntitiesCommandSet(this);

            return this._commandSet;
        }

        public async Task<EntityV1> CreateEntityAsync(string correlationId, EntityV1 entity)
        {
            entity.Id ??= IdGenerator.NextLong();
            entity.Type ??= EntityTypeV1.Unknown;

            return await this._persistence.CreateAsync(correlationId, entity);
        }

        public async Task<EntityV1> DeleteEntityByIdAsync(string correlationId, string entityId)
        {
            return await this._persistence.DeleteByIdAsync(correlationId, entityId);
        }

        public async Task<DataPage<EntityV1>> GetEntitiesAsync(string correlationId, FilterParams filter, PagingParams paging)
        {
            return await this._persistence.GetPageByFilterAsync(correlationId, filter, paging);
        }

        public async Task<EntityV1> GetEntityByIdAsync(string correlationId, string entityId)
        {
            return await this._persistence.GetOneByIdAsync(correlationId, entityId);
        }

        public async Task<EntityV1> GetEntityByNameAsync(string correlationId, string entityId)
        {
            return await this._persistence.GetOneByNameAsync(correlationId, entityId);
        }

        public async Task<EntityV1> UpdateEntityAsync(string correlationId, EntityV1 entity)
        {
            entity.Type ??= EntityTypeV1.Unknown;

            return await this._persistence.UpdateAsync(correlationId, entity);
        }

    }
}

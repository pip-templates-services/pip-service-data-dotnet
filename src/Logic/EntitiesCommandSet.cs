using PipServices3.Commons.Commands;
using PipServices3.Commons.Convert;
using PipServices3.Commons.Data;
using PipServices3.Commons.Validate;
using PipTemplatesServiceData.Data.Version1;

namespace PipTemplatesServiceData.Logic
{
    public class EntitiesCommandSet : CommandSet
    {
        private IEntitiesController _controller;

        public EntitiesCommandSet(IEntitiesController controller) : base()
        {
            this._controller = controller;

            this.AddCommand(this.MakeGetEntitiesCommand());
            this.AddCommand(this.MakeGetEntityByIdCommand());
            this.AddCommand(this.MakeGetEntityByNameCommand());
            this.AddCommand(this.MakeCreateEntityCommand());
            this.AddCommand(this.MakeUpdateEntityCommand());
            this.AddCommand(this.MakeDeleteEntityByIdCommand());
        }

        private ICommand MakeGetEntitiesCommand()
        {
            return new Command(
                "get_entities",
                new ObjectSchema()
                    .WithOptionalProperty("filter", new FilterParamsSchema())
                    .WithOptionalProperty("paging", new PagingParamsSchema()).AllowUndefined(true),

                async (correlationId, parameters) =>
                {
                    var filter = FilterParams.FromValue(parameters.Get("filter"));
                    var paging = PagingParams.FromValue(parameters.Get("paging"));
                    return await this._controller.GetEntitiesAsync(correlationId, filter, paging);
                }
            );
        }

        private ICommand MakeGetEntityByIdCommand()
        {
            return new Command(
                "get_entity_by_id",
                new ObjectSchema()
                    .WithRequiredProperty("entity_id", TypeCode.String).AllowUndefined(true),
                async (correlationId, parameters) =>
                {
                    var entityId = parameters.GetAsString("entity_id");
                    return await this._controller.GetEntityByIdAsync(correlationId, entityId);
                }
            );
        }

        private ICommand MakeGetEntityByNameCommand()
        {
            return new Command(
                "get_entity_by_name",
                new ObjectSchema()
                    .WithRequiredProperty("name", TypeCode.String).AllowUndefined(true),
                async (correlationId, parameters) =>
                {
                    var name = parameters.GetAsString("name");
                    return await this._controller.GetEntityByNameAsync(correlationId, name);
                }
            );
        }

        private ICommand MakeCreateEntityCommand()
        {
            return new Command(
                "create_entity",
                new ObjectSchema()
                    .WithRequiredProperty("entity", new EntityV1Schema()).AllowUndefined(true),
                async (correlationId, parameters) =>
                {
                    var entity = ConvertToBeacon(parameters.GetAsObject("entity"));
                    return await this._controller.CreateEntityAsync(correlationId, (EntityV1)entity);
                }
            );
        }

        private ICommand MakeUpdateEntityCommand()
        {
            return new Command(
                "update_entity",
                new ObjectSchema()
                    .WithRequiredProperty("entity", new EntityV1Schema()).AllowUndefined(true),
                async (correlationId, parameters) =>
                {
                    var entity = ConvertToBeacon(parameters.GetAsObject("entity"));
                    return await this._controller.UpdateEntityAsync(correlationId, entity);
                }
            );
        }

        private ICommand MakeDeleteEntityByIdCommand()
        {
            return new Command(
                "delete_entity_by_id",
                new ObjectSchema()
                    .WithRequiredProperty("entity_id", TypeCode.String).AllowUndefined(true),
                async (correlationId, parameters) =>
                {
                    var entityId = parameters.GetAsString("entity_id");
                    return await this._controller.DeleteEntityByIdAsync(correlationId, entityId);
                }
            );
        }

        private EntityV1 ConvertToBeacon(object value)
        {
            return JsonConverter.FromJson<EntityV1>(JsonConverter.ToJson(value));
        }
    }
}

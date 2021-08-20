using Grpc.Core;
using PipServices3.Commons.Data;
using PipServices3.Commons.Refer;
using PipServices3.Grpc.Services;
using PipTemplatesServiceData.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PipTemplatesServiceData.Services.Version1
{
    public class EntitiesGrpcServiceV1 : GrpcService
    {
        private IEntitiesController _controller;

        public EntitiesGrpcServiceV1(): this("entities_v1") { }

        public EntitiesGrpcServiceV1(string name) : base(name)
        {
            this._dependencyResolver.Put("controller", new Descriptor("pip-service-data", "controller", "*", "*", "*"));
        }

        public override void SetReferences(IReferences references)
        {
            base.SetReferences(references);
            this._controller = this._dependencyResolver.GetOneRequired<IEntitiesController>("controller");
        }

        private async Task<EntitiesV1.EntitiesPageReply> GetEntitiesAsync(EntitiesV1.EntitiesPageRequest request, ServerCallContext context)
        {
            var correlationId = request.CorrelationId;
            var filter = new FilterParams();

            EntitiesGrpcConverterV1.SetMap(filter, request.Filter);
            var paging = EntitiesGrpcConverterV1.ToPagingParams(request.Paging);

            var response = new EntitiesV1.EntitiesPageReply();
            var timing = this.Instrument(correlationId, "get_entities");

            try
            {
                var result = await _controller.GetEntitiesAsync(correlationId, filter, paging);
                var page = EntitiesGrpcConverterV1.FromEntitiesPage(result);
                response.Page = page;
            }
            catch (Exception ex)
            {
                var err = EntitiesGrpcConverterV1.FromError(ex);
                response.Error = err;
            }
            finally
            {
                timing.EndTiming();
            }

            return response;
        }

        private async Task<EntitiesV1.EntityReply> GetEntityByIdAsync(EntitiesV1.EntityIdRequest request, ServerCallContext context)
        {
            var correlationId = request.CorrelationId;
            var id = request.EntityId;

            var response = new EntitiesV1.EntityReply();
            var timing = this.Instrument(correlationId, "get_entity_by_id");

            try
            {
                var result = await this._controller.GetEntityByIdAsync(
                    correlationId,
                    id
                );
                var entity = EntitiesGrpcConverterV1.FromEntity(result);
                response.Entity = entity;
            }
            catch (Exception err)
            {
                var error = EntitiesGrpcConverterV1.FromError(err);
                response.Error = error;
            }
            finally
            {
                timing.EndTiming();
            }

            return response;
        }

        private async Task<EntitiesV1.EntityReply> GetEntityByNameAsync(EntitiesV1.EntityNameRequest request, ServerCallContext context)
        {
            var correlationId = request.CorrelationId;
            var name = request.Name;

            var response = new EntitiesV1.EntityReply();
            var timing = this.Instrument(correlationId, "get_entity_by_name");

            try
            {
                var result = await this._controller.GetEntityByNameAsync(
                    correlationId,
                    name
                );

                var entity = EntitiesGrpcConverterV1.FromEntity(result);
                response.Entity = entity;
            }
            catch (Exception err)
            {
                var error = EntitiesGrpcConverterV1.FromError(err);
                response.Error = error;
            }
            finally
            {
                timing.EndTiming();
            }

            return response;
        }

        private async Task<EntitiesV1.EntityReply> CreateEntityAsync(EntitiesV1.EntityRequest request, ServerCallContext context)
        {
            var correlationId = request.CorrelationId;
            var entityProto = request.Entity;
            var entity = EntitiesGrpcConverterV1.ToEntity(entityProto);

            var response = new EntitiesV1.EntityReply();
            var timing = this.Instrument(correlationId, "create_entity");
            
            try
            {
                var result = await this._controller.CreateEntityAsync(
                    correlationId,
                    entity
                );
                entityProto = EntitiesGrpcConverterV1.FromEntity(result);
                response.Entity = entityProto;
            }
            catch (Exception err)
            {
                var error = EntitiesGrpcConverterV1.FromError(err);
                response.Error = error;
            }
            finally
            {
                timing.EndTiming();
            }

            return response;
        }

        private async Task<EntitiesV1.EntityReply> UpdateEntityAsync(EntitiesV1.EntityRequest request, ServerCallContext context)
        {
            var correlationId = request.CorrelationId;
            var entityRecv = request.Entity;
            var entity = EntitiesGrpcConverterV1.ToEntity(entityRecv);

            var response = new EntitiesV1.EntityReply();
            var timing = this.Instrument(correlationId, "update_entity");

            try
            {
                var result = await this._controller.UpdateEntityAsync(
                    correlationId,
                    entity
                );
                entityRecv = EntitiesGrpcConverterV1.FromEntity(result);
                response.Entity = entityRecv;
            }
            catch (Exception err)
            {
                var error = EntitiesGrpcConverterV1.FromError(err);
                response.Error = error;
            }
            finally
            {
                timing.EndTiming();
            }

            return response;
        }

        private async Task<EntitiesV1.EntityReply> DeleteEntityByIdAsync(EntitiesV1.EntityIdRequest request, ServerCallContext context)
        {
            var correlationId = request.CorrelationId;
            var id = request.EntityId;

            var response = new EntitiesV1.EntityReply();
            var timing = this.Instrument(correlationId, "delete_entity_by_id");

            try
            {
                var result = await this._controller.DeleteEntityByIdAsync(
                    correlationId,
                    id
                );
                var entity = EntitiesGrpcConverterV1.FromEntity(result);
                response.Entity = entity;
            }
            catch (Exception err)
            {
                var error = EntitiesGrpcConverterV1.FromError(err);
                response.Error = error;
            }
            finally
            {
                timing.EndTiming();
            }

            return response;
        }

        protected override void OnRegister()
        {
            RegisterMethod<EntitiesV1.EntitiesPageRequest, EntitiesV1.EntitiesPageReply>(
                "get_entities",
                this.GetEntitiesAsync
            );

            RegisterMethod<EntitiesV1.EntityIdRequest, EntitiesV1.EntityReply>(
                "get_entity_by_id",
                this.GetEntityByIdAsync
            );

            RegisterMethod<EntitiesV1.EntityNameRequest, EntitiesV1.EntityReply>(
                "get_entity_by_name",
                this.GetEntityByNameAsync
            );

            RegisterMethod<EntitiesV1.EntityRequest, EntitiesV1.EntityReply>(
                "create_entity",
                this.CreateEntityAsync
            );

            RegisterMethod<EntitiesV1.EntityRequest, EntitiesV1.EntityReply>(
                "update_entity",
                this.UpdateEntityAsync
            );

            RegisterMethod<EntitiesV1.EntityIdRequest, EntitiesV1.EntityReply>(
                "delete_entity_by_id",
                this.DeleteEntityByIdAsync
            );
        }
    }
}

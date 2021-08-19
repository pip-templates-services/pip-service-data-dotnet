using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using PipServices3.Commons.Data;
using PipServices3.Commons.Refer;
using PipServices3.Rpc.Services;
using PipTemplatesServiceData.Data.Version1;
using PipTemplatesServiceData.Logic;
using Microsoft.Extensions.Primitives;
using PipServices3.Commons.Convert;
using System.IO;

namespace PipTemplatesServiceData.Services.Version1
{
    public class EntitiesRestServiceV1 : RestService
    {
        private IEntitiesController _controller;

        public EntitiesRestServiceV1() : base()
        {
            this._baseRoute = "v1/entities";
            this._dependencyResolver.Put(
                "controller",
                new Descriptor("pip-service-data", "controller", "default", "*", "*")
            );
        }

        public override void SetReferences(IReferences references)
        {
            base.SetReferences(references);
            this._controller = this._dependencyResolver.GetOneRequired<IEntitiesController>("controller");
        }

        public async Task GetEntitiesAsync(HttpRequest request, HttpResponse response, RouteData routeData)
        {
            var correlationId = this.GetCorrelationId(request);
            var filter = this.GetFilterParams(request);
            var paging = this.GetPagingParams(request);

            var timing = this.Instrument(correlationId, "get_entities");

            try
            {
                var result = await this._controller.GetEntitiesAsync(correlationId, filter, paging);
                await this.SendResultAsync(response, result);
            }
            catch (Exception ex)
            {
                await this.SendErrorAsync(response, ex);
            }
            finally
            {
                timing.EndTiming();
            }
        }

        public async Task GetEntityByIdAsync(HttpRequest request, HttpResponse response, RouteData routeData)
        {
            var correlationId = this.GetCorrelationId(request);
            var id = routeData.Values["id"].ToString();

            var timing = this.Instrument(correlationId, "get_entity_by_id");

            try
            {
                var result = await this._controller.GetEntityByIdAsync(correlationId, id);
                await this.SendResultAsync(response, result);
            }
            catch (Exception ex)
            {
                await this.SendErrorAsync(response, ex);
            }
            finally
            {
                timing.EndTiming();
            }
        }

        public async Task GetEntityByNameAsync(HttpRequest request, HttpResponse response, RouteData routeData)
        {
            var correlationId = this.GetCorrelationId(request);
            var name = routeData.Values["name"].ToString();

            var timing = this.Instrument(correlationId, "get_entity_by_name");
            try
            {
                var result = await this._controller.GetEntityByNameAsync(correlationId, name);
                await this.SendResultAsync(response, result);
            }
            catch (Exception ex)
            {
                await this.SendResultAsync(response, ex);
            }
            finally
            {
                timing.EndTiming();
            }
        }

        public async Task CreateEntityAsync(HttpRequest request, HttpResponse response, RouteData routeData)
        {
            var correlationId = this.GetCorrelationId(request);
            EntityV1 data = null;
            using (var streamReader = new StreamReader(request.Body))
            {
                data = JsonConverter.FromJson<EntityV1>(streamReader.ReadToEnd());
            }

            var timing = this.Instrument(correlationId, "create_entity");

            try
            {
                var result = await this._controller.CreateEntityAsync(correlationId, data);
                await this.SendResultAsync(response, result);
            }
            catch (Exception ex)
            {
                await this.SendErrorAsync(response,ex);
            }
            finally
            {
                timing.EndTiming();
            }
        }

        public async Task UpdateEntityAsync(HttpRequest request, HttpResponse response, RouteData routeData)
        {
            var correlationId = this.GetCorrelationId(request);
            EntityV1 data = null;
            using (var streamReader = new StreamReader(request.Body))
            {
                data = JsonConverter.FromJson<EntityV1>(streamReader.ReadToEnd());
            }

            var timing = this.Instrument(correlationId, "update_entity");

            try
            {
                var result = await this._controller.UpdateEntityAsync(correlationId, data);
                await this.SendResultAsync(response, result);
            }
            catch (Exception ex)
            {
                await this.SendResultAsync(response, ex);
            }
            finally
            {
                timing.EndTiming();
            }
        }

        public async Task DeleteEntityByIdAsync(HttpRequest request, HttpResponse response, RouteData routeData)
        {
            var correlationId = this.GetCorrelationId(request);
            var id = routeData.Values["id"].ToString();

            var timing = this.Instrument(correlationId, "delete_entity_by_id");

            try
            {
                var result = await this._controller.DeleteEntityByIdAsync(correlationId, id);
                await this.SendResultAsync(response, result);
            }
            catch (Exception ex)
            {
                await this.SendErrorAsync(response, ex);
            }
            finally
            {
                timing.EndTiming();
            }
        }

        public override void Register()
        {
            this.RegisterRoute("get", "/entities", this.GetEntitiesAsync);
            this.RegisterRoute("get", "/entities/{id}", this.GetEntityByIdAsync);
            this.RegisterRoute("get", "/entities/name/{name}", this.GetEntityByNameAsync);
            this.RegisterRoute("post", "/entities", this.CreateEntityAsync);
            this.RegisterRoute("put", "/entities", this.UpdateEntityAsync);
            this.RegisterRoute("delete", "/entities/{id}", this.DeleteEntityByIdAsync);

            this.RegisterOpenApiSpecFromFile(Directory.GetCurrentDirectory() + "/../../../../src/swagger/entities_v1.yaml");
        }

    }
}

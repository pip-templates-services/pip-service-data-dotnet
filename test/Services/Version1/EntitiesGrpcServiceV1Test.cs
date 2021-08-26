using Grpc.Core;
using PipServices3.Commons.Config;
using PipServices3.Commons.Data;
using PipServices3.Commons.Refer;
using PipTemplatesServiceData.Data.Version1;
using PipTemplatesServiceData.Logic;
using PipTemplatesServiceData.Persistence;
using PipTemplatesServiceData.Services.Version1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PipTemplatesServiceData.Test.Services.Version1
{
    [Collection("Sequential")]
    public class EntitiesGrpcServiceV1Test: IDisposable
    {
        private EntityV1 ENTITY1 = new EntityV1
        {
            Id = "1",
            Name = "00001",
            Type = EntityTypeV1.Type1,
            SiteId = "1",
            Content = "ABC"
        };

        private EntityV1 ENTITY2 = new EntityV1
        {
            Id = "2",
            Name = "00002",
            Type = EntityTypeV1.Type2,
            SiteId = "1",
            Content = "XYZ"
        };

        private ConfigParams grpcConfig = ConfigParams.FromTuples(
            "connection.protocol", "http",
            "connection.host", "localhost",
            "connection.port", "3000"
        );

        private EntitiesMemoryPersistence persistence;
        private EntitiesController controller;
        private EntitiesGrpcServiceV1 service;
        private EntitiesV1.Entities.EntitiesClient client;

        private string correlationId;

        public EntitiesGrpcServiceV1Test()
        {
            correlationId = IdGenerator.NextLong();

            persistence = new EntitiesMemoryPersistence();
            persistence.Configure(new ConfigParams());

            controller = new EntitiesController();
            controller.Configure(new ConfigParams());

            service = new EntitiesGrpcServiceV1("entities_v1.Entities");
            service.Configure(grpcConfig);

            var references = References.FromTuples(
                new Descriptor("pip-service-data", "persistence", "memory", "default", "1.0"), persistence,
                new Descriptor("pip-service-data", "controller", "default", "default", "1.0"), controller,
                new Descriptor("pip-service-data", "service", "grpc", "default", "1.0"), service
            );

            controller.SetReferences(references);
            service.SetReferences(references);

            persistence.OpenAsync(null).Wait();
            service.OpenAsync(null).Wait();

            var channel = new Channel("localhost:3000", ChannelCredentials.Insecure);
            client = new EntitiesV1.Entities.EntitiesClient(channel);
        }

        public void Dispose()
        {
            service.CloseAsync(null).Wait();
            persistence.CloseAsync(null).Wait();
        }

        [Fact]
        public async Task TestCrudOperationsAsync()
        {
            var callOptions = new CallOptions();

            // Create the first entity
            var request = new EntitiesV1.EntityRequest() { 
                CorrelationId=correlationId,
                Entity= EntitiesGrpcConverterV1.FromEntity(ENTITY1)
            };

            var response = await client.create_entityAsync(request, callOptions);

            var entity = EntitiesGrpcConverterV1.ToEntity(response.Entity);

            Assert.NotNull(entity);
            Assert.Equal(ENTITY1.Name, entity.Name);
            Assert.Equal(ENTITY1.SiteId, entity.SiteId);
            Assert.Equal(ENTITY1.Type, entity.Type);
            Assert.Equal(ENTITY1.Name, entity.Name);
            Assert.NotNull(entity.Content);

            // Create the second entity
            request = new EntitiesV1.EntityRequest()
            {
                CorrelationId = correlationId,
                Entity = EntitiesGrpcConverterV1.FromEntity(ENTITY2)
            };
            response = await client.create_entityAsync(request, callOptions);

            entity = EntitiesGrpcConverterV1.ToEntity(response.Entity);

            Assert.NotNull(entity);
            Assert.Equal(ENTITY2.Name, entity.Name);
            Assert.Equal(ENTITY2.SiteId, entity.SiteId);
            Assert.Equal(ENTITY2.Type, entity.Type);
            Assert.Equal(ENTITY2.Name, entity.Name);
            Assert.NotNull(entity.Content);

            // Get all entities
            var pageRequest = new EntitiesV1.EntitiesPageRequest() { 
                Paging= new EntitiesV1.PagingParams()
             };

            var responsePage = await client.get_entitiesAsync(pageRequest);

            var page = responsePage != null ? responsePage.Page : null;

            Assert.NotNull(page);
            Assert.Equal(2, page.Data.Count);

            var entity1 = page.Data[0];

            // Update the entity
            entity1.Name = "ABC";

            request = new EntitiesV1.EntityRequest();
            request.Entity = entity1;

            response = await client.update_entityAsync(request);

            entity = response!=null ? EntitiesGrpcConverterV1.ToEntity(response.Entity) : null;

            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);
            Assert.Equal("ABC", entity.Name);

            // Get entity by name
            var requestName = new EntitiesV1.EntityNameRequest() { Name=entity1.Name };

            response = await client.get_entity_by_nameAsync(requestName);

            entity = response != null ? EntitiesGrpcConverterV1.ToEntity(response.Entity) : null;

            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);

            // Delete the entity
            var requestId = new EntitiesV1.EntityIdRequest() { EntityId=entity1.Id };

            response = await client.delete_entity_by_idAsync(requestId);

            entity = response != null ? EntitiesGrpcConverterV1.ToEntity(response.Entity) : null;

            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);

            // Try to get deleted entity

            requestId = new EntitiesV1.EntityIdRequest() { EntityId = entity1.Id };

            response = await client.delete_entity_by_idAsync(requestId);

            entity = response != null ? EntitiesGrpcConverterV1.ToEntity(response.Entity) : null;

            Assert.Null(entity);

        }
    }
}

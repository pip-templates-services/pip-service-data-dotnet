using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using PipServices3.Commons.Config;
using PipServices3.Commons.Data;
using PipServices3.Commons.Refer;
using PipServices3.Rpc.Test;
using PipTemplatesServiceData.Data.Version1;
using PipTemplatesServiceData.Logic;
using PipTemplatesServiceData.Persistence;
using PipTemplatesServiceData.Services.Version1;

namespace PipTemplatesServiceData.Test.Services.Version1
{
    [Collection("Sequential")]
    public class EntitiesCommandableHttpServiceV1Test : IDisposable
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

        private ConfigParams httpConfig = ConfigParams.FromTuples(
            "connection.protocol", "http",
            "connection.host", "localhost",
            "connection.port", 3000
        );

        private EntitiesMemoryPersistence persistence;
        private EntitiesController controller;
        private EntitiesCommandableHttpServiceV1 service;
        private TestCommandableHttpClient client;

        private string correlationId;

        public EntitiesCommandableHttpServiceV1Test()
        {
            correlationId = IdGenerator.NextLong();

            persistence = new EntitiesMemoryPersistence();
            persistence.Configure(new ConfigParams());

            controller = new EntitiesController();
            controller.Configure(new ConfigParams());

            service = new EntitiesCommandableHttpServiceV1();
            service.Configure(httpConfig);

            client = new TestCommandableHttpClient("v1/entities");
            client.Configure(httpConfig);

            var references = References.FromTuples(
                new Descriptor("pip-service-data", "persistence", "memory", "default", "1.0"), persistence,
                new Descriptor("pip-service-data", "controller", "default", "default", "1.0"), controller,
                new Descriptor("pip-service-data", "service", "http", "default", "1.0"), service
            );

            controller.SetReferences(references);
            service.SetReferences(references);

            persistence.OpenAsync(null).Wait();
            service.OpenAsync(null).Wait();
            client.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            client.CloseAsync(null).Wait();
            service.CloseAsync(null).Wait();
            persistence.CloseAsync(null).Wait();
        }

        [Fact]
        public async Task TestCrudOperationsAsync()
        {
            var entity = await client.CallCommandAsync<EntityV1>("create_entity", correlationId, new { entity = ENTITY1 });
            Assert.NotNull(entity);
            Assert.Equal(ENTITY1.Name, entity.Name);
            Assert.Equal(ENTITY1.SiteId, entity.SiteId);
            Assert.Equal(ENTITY1.Type, entity.Type);
            Assert.Equal(ENTITY1.Name, entity.Name);
            Assert.NotNull(entity.Content);

            // Create the second entity
            entity = await client.CallCommandAsync<EntityV1>("create_entity", correlationId, new { entity = ENTITY2 });

            Assert.NotNull(entity);
            Assert.Equal(ENTITY2.Name, entity.Name);
            Assert.Equal(ENTITY2.SiteId, entity.SiteId);
            Assert.Equal(ENTITY2.Type, entity.Type);
            Assert.Equal(ENTITY2.Name, entity.Name);
            Assert.NotNull(entity.Content);

            // Get all entities
            var page = await client.CallCommandAsync<DataPage<EntityV1>>(
                "get_entities",
                correlationId,
                new
                {
                    filter = new FilterParams(),
                    paging = new PagingParams()
                }
            );

            Assert.NotNull(page);
            Assert.Equal(2, page.Data.Count);

            // Update the entity
            var entity1 = page.Data[0];
            entity1.Name = "ABC";

            entity = await client.CallCommandAsync<EntityV1>("update_entity", correlationId, new { entity = entity1 });

            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);
            Assert.Equal("ABC", entity.Name);

            // Get entity by name
            entity = await client.CallCommandAsync<EntityV1>("get_entity_by_name", correlationId, new { name = entity1.Name });

            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);

            // Delete the entity
            entity = await client.CallCommandAsync<EntityV1>("delete_entity_by_id", correlationId, new { entity_id = entity1.Id });
            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);

            // Try to get deleted entity
            entity = await client.CallCommandAsync<EntityV1>("get_entity_by_id", correlationId, new { entity_id = entity1.Id });
            Assert.Null(entity);
        }
    }
}

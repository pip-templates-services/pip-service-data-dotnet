using PipServices3.Commons.Config;
using PipServices3.Commons.Data;
using PipServices3.Commons.Refer;
using PipTemplatesServiceData.Data.Version1;
using PipTemplatesServiceData.Logic;
using PipTemplatesServiceData.Persistence;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipTemplatesServiceData.Test.Logic
{
    [Collection("Sequential")]
    public class EntitiesControllerTest : IDisposable
    {
        public EntityV1 ENTITY1 = new EntityV1
        {
            Id = "1",
            Name = "00001",
            Type = EntityTypeV1.Type1,
            SiteId = "1",
            Content = "ABC"
        };

        public EntityV1 ENTITY2 = new EntityV1
        {
            Id = "2",
            Name = "00002",
            Type = EntityTypeV1.Type2,
            SiteId = "2",
            Content = "XYZ"
        };

        private EntitiesMemoryPersistence persistence;
        private EntitiesController controller;

        public EntitiesControllerTest()
        {
            persistence = new EntitiesMemoryPersistence();
            controller = new EntitiesController();

            controller.Configure(new ConfigParams());

            var references = References.FromTuples(
                new Descriptor("pip-service-data", "persistence", "memory", "*", "1.0"), persistence,
                new Descriptor("pip-service-data", "controller", "default", "*", "1.0"), controller
            );

            controller.SetReferences(references);

            persistence.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            persistence.CloseAsync(null).Wait();
        }

        [Fact]
        public async Task TestCrudOperationsAsync()
        {
            // Create the first entity
            var entity = await controller.CreateEntityAsync(null, ENTITY1);
            Assert.NotNull(entity);
            Assert.Equal(ENTITY1.Name, entity.Name);
            Assert.Equal(ENTITY1.SiteId, entity.SiteId);
            Assert.Equal(ENTITY1.Type, entity.Type);
            Assert.Equal(ENTITY1.Name, entity.Name);
            Assert.NotNull(entity.Content);

            // Create the second entity
            entity = await controller.CreateEntityAsync(null, ENTITY2);
            Assert.NotNull(entity);
            Assert.Equal(ENTITY2.Name, entity.Name);
            Assert.Equal(ENTITY2.SiteId, entity.SiteId);
            Assert.Equal(ENTITY2.Type, entity.Type);
            Assert.Equal(ENTITY2.Name, entity.Name);
            Assert.NotNull(entity.Content);

            // Get all entities
            var page = await controller.GetEntitiesAsync(null, new FilterParams(), new PagingParams());
            Assert.NotNull(page);
            Assert.Equal(2, page.Data.Count);

            var entity1 = page.Data[0];

            // Update the entity
            entity1.Name = "ABC";

            entity = await controller.UpdateEntityAsync(null, entity1);
            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);
            Assert.Equal("ABC", entity.Name);

            // Get entity by name
            entity = await controller.GetEntityByNameAsync(null, entity1.Name);
            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);

            // Delete the entity
            entity = await controller.DeleteEntityByIdAsync(null, entity1.Id);
            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);

            // Try to get deleted entity
            entity = await controller.GetEntityByIdAsync(null, entity1.Id);
            Assert.Null(entity);
        }
    }
}

using System.Threading.Tasks;
using PipServices3.Commons.Data;
using PipTemplatesServiceData.Data.Version1;
using PipTemplatesServiceData.Persistence;
using Xunit;

namespace PipTemplatesServiceData.Test.Persistence
{
    [Collection("Sequential")]
    public class EntitiesPersistenceFixture
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

        private EntityV1 ENTITY3 = new EntityV1
        {
            Id = "3",
            Name = "00003",
            Type = EntityTypeV1.Type1,
            SiteId = "2",
            Content = "DEF"
        };

        private IEntitiesPersistence persistence;

        public EntitiesPersistenceFixture(IEntitiesPersistence persistence)
        {
            Assert.NotNull(persistence);
            this.persistence = persistence;
        }

        private async Task TestCreateEntitiesAsync()
        {
            // Create the first entity
            var entity = await this.persistence.CreateAsync(null, ENTITY1);
            Assert.NotNull(entity);
            Assert.Equal(ENTITY1.Name, entity.Name);
            Assert.Equal(ENTITY1.SiteId, entity.SiteId);
            Assert.Equal(ENTITY1.Type, entity.Type);
            Assert.Equal(ENTITY1.Name, entity.Name);
            Assert.NotNull(entity.Content);

            // Create the second entity
            entity = await this.persistence.CreateAsync(null, ENTITY2);
            Assert.NotNull(entity);
            Assert.Equal(ENTITY2.Name, entity.Name);
            Assert.Equal(ENTITY2.SiteId, entity.SiteId);
            Assert.Equal(ENTITY2.Type, entity.Type);
            Assert.Equal(ENTITY2.Name, entity.Name);
            Assert.NotNull(entity.Content);

            // Create the third entity
            entity = await this.persistence.CreateAsync(null, ENTITY3);
            Assert.NotNull(entity);
            Assert.Equal(ENTITY3.Name, entity.Name);
            Assert.Equal(ENTITY3.SiteId, entity.SiteId);
            Assert.Equal(ENTITY3.Type, entity.Type);
            Assert.Equal(ENTITY3.Name, entity.Name);
            Assert.NotNull(entity.Content);
        }

        public async Task TestCrudOperationsAsync()
        {
            // Create items
            await this.TestCreateEntitiesAsync();

            // Get all entities
            var page = await this.persistence.GetPageByFilterAsync(
                null, new FilterParams(), new PagingParams());
            Assert.NotNull(page);
            Assert.Equal(3, page.Data.Count);

            var entity1 = page.Data[0];

            // Update the entity
            entity1.Name = "ABC";

            var entity = await this.persistence.UpdateAsync(null, entity1);
            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);
            Assert.Equal("ABC", entity.Name);

            // Get entity by name
            entity = await this.persistence.GetOneByNameAsync(null, entity1.Name);
            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);

            // Delete the entity
            entity = await this.persistence.DeleteByIdAsync(null, entity1.Id);
            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);

            // Try to get deleted entity
            entity = await this.persistence.GetOneByIdAsync(null, entity1.Id);
            Assert.Null(entity);
        }

        public async Task TestGetWithFiltersAsync()
        {
            // Create items
            await this.TestCreateEntitiesAsync();

            // Filter by id
            var page = await this.persistence.GetPageByFilterAsync(
                null,
                FilterParams.FromTuples(
                    "id", "1"
                ),
                new PagingParams()
            );

            Assert.Single(page.Data);

            // Filter by names
            page = await this.persistence.GetPageByFilterAsync(
                null,
                FilterParams.FromTuples(
                    "names", "00001,00003"
                ),
                new PagingParams()
            );

            Assert.Equal(2, page.Data.Count);

            // Filter by site_id
            page = await this.persistence.GetPageByFilterAsync(
                null,
                FilterParams.FromTuples(
                    "site_id", "1"
                ),
                new PagingParams()
            );

            Assert.Equal(2, page.Data.Count);

        }
    }
}

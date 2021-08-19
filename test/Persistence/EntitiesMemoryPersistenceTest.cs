using PipServices3.Commons.Config;
using PipTemplatesServiceData.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PipTemplatesServiceData.Test.Persistence
{
    [Collection("Sequential")]
    public class EntitiesMemoryPersistenceTest: IDisposable
    {
        private EntitiesMemoryPersistence persistence;
        private EntitiesPersistenceFixture fixture;

        public EntitiesMemoryPersistenceTest()
        {
            persistence = new EntitiesMemoryPersistence();
            persistence.Configure(new ConfigParams());

            fixture = new EntitiesPersistenceFixture(persistence);

            persistence.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            persistence.CloseAsync(null).Wait();
        }
        
        [Fact]
        public async Task TestCrudOperationsAsync()
        {
            await fixture.TestCrudOperationsAsync();
        }

        [Fact]
        public async Task TestGetWithFiltersAsync()
        {
            await fixture.TestGetWithFiltersAsync();
        }
    }
}

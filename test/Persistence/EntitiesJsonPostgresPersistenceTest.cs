using PipServices3.Commons.Config;
using PipTemplatesServiceData.Persistence;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipTemplatesServiceData.Test.Persistence
{
    [Collection("Sequential")]
    public class EntitiesJsonPostgresPersistenceTest: IDisposable
    {
        private bool _enabled = false;
        private EntitiesJsonPostgresPersistence _persistence;
        private EntitiesPersistenceFixture _fixture;

        public EntitiesJsonPostgresPersistenceTest()
        {
            var postgresDb = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "test";
            var postgresHost = Environment.GetEnvironmentVariable("POSTGRES_SERVICE_HOST") ?? "localhost";
            var postgresPort = Environment.GetEnvironmentVariable("POSTGRES_SERVICE_PORT") ?? "5432";
            var postgresUri = Environment.GetEnvironmentVariable("POSTGRES_SERVICE_URI");
            var postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
            var postgresPass = Environment.GetEnvironmentVariable("POSTGRES_PASS") ?? "postgres";

            _enabled = !string.IsNullOrEmpty(postgresUri) || !string.IsNullOrEmpty(postgresHost);

            if (_enabled)
            {
                var config = ConfigParams.FromTuples(
                    "connection.database", postgresDb,
                    "connection.host", postgresHost,
                    "connection.port", postgresPort,
                    "connection.uri", postgresUri,
                    "credential.username", postgresUser,
                    "credential.password", postgresPass
                );

                _persistence = new EntitiesJsonPostgresPersistence();
                _persistence.Configure(config);
                _persistence.OpenAsync(null).Wait();
                _persistence.ClearAsync(null).Wait();

                _fixture = new EntitiesPersistenceFixture(_persistence);
            }
        }

        public void Dispose()
        {
            if (_enabled)
                _persistence.CloseAsync(null).Wait();
        }

        [Fact]
        public async Task TestCrudOperationsAsync()
        {
            if (_enabled)
                await _fixture.TestCrudOperationsAsync();
        }

        [Fact]
        public async Task TestGetWithFiltersAsync()
        {
            if (_enabled)
                await _fixture.TestGetWithFiltersAsync();
        }
    }
}

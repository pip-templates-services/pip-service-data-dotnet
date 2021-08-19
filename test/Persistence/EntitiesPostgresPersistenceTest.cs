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
    public class EntitiesPostgresPersistenceTest
    {
        private EntitiesPostgresPersistence persistence;
        private EntitiesPersistenceFixture fixture;
        private string postgresUri;
        private string postgresHost;
        private string postgresPort;
        private string postgresDatabase;
        private string postgresUser;
        private string postgresPassword;

        private bool _enabled = false;
    

        public EntitiesPostgresPersistenceTest()
        {
            postgresUri = Environment.GetEnvironmentVariable("POSTGRES_SERVICE_URI");
            postgresHost = Environment.GetEnvironmentVariable("POSTGRES_SERVICE_HOST") ?? "localhost";
            postgresPort = Environment.GetEnvironmentVariable("POSTGRES_SERVICE_PORT") ?? "5432";
            postgresDatabase = Environment.GetEnvironmentVariable("POSTGRES_SERVICE_DB") ?? "test";
            postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
            postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASS") ?? "postgres";

            // Exit if postgres connection is not set
            if (postgresUri == null && postgresHost == null)
                return;

            _enabled = !string.IsNullOrEmpty(postgresUri) || !string.IsNullOrEmpty(postgresHost);

            if (_enabled)
            {
                persistence = new EntitiesPostgresPersistence();
                persistence.Configure(ConfigParams.FromTuples(
                    "connection.uri", postgresUri,
                    "connection.host", postgresHost,
                    "connection.port", postgresPort,
                    "connection.database", postgresDatabase,
                    "credential.username", postgresUser,
                    "credential.password", postgresPassword
                ));

                fixture = new EntitiesPersistenceFixture(persistence);

                persistence.OpenAsync(null).Wait();
                persistence.ClearAsync(null).Wait();
            }
        }


        public void Dispose()
        {
            if (_enabled)
                persistence.ClearAsync(null).Wait();
        }

        [Fact]
        public async Task TestCrudOperationsAsync()
        {
            if (_enabled)
                await fixture.TestCrudOperationsAsync();
        }

        [Fact]
        public async Task TestGetWithFiltersAsync()
        {
            if (_enabled)
                await fixture.TestGetWithFiltersAsync();
        }

    }
}

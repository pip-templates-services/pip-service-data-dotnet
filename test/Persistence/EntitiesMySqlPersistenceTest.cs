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
    public class EntitiesMySqlPersistenceTest: IDisposable
    {
        private EntitiesMySqlPersistence persistence;
        private EntitiesPersistenceFixture fixture;

        private string mysqlUri;
        private string mysqlHost;
        private string mysqlPort;
        private string mysqlDatabase;
        private string mysqlUser;
        private string mysqlPassword;

        private bool _enabled = false;

        public EntitiesMySqlPersistenceTest()
        {
            mysqlUri = Environment.GetEnvironmentVariable("MYSQL_URI");
            mysqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
            mysqlPort = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
            mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DB") ?? "test";
            mysqlUser = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "user";
            mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "password";

            _enabled = !string.IsNullOrEmpty(mysqlUri) || !string.IsNullOrEmpty(mysqlHost);

            if (_enabled)
            {
                persistence = new EntitiesMySqlPersistence();
                persistence.Configure(ConfigParams.FromTuples(
                    "connection.uri", mysqlUri,
                    "connection.host", mysqlHost,
                    "connection.port", mysqlPort,
                    "connection.database", mysqlDatabase,
                    "credential.username", mysqlUser,
                    "credential.password", mysqlPassword
                ));

                fixture = new EntitiesPersistenceFixture(persistence);

                persistence.OpenAsync(null).Wait();
                persistence.ClearAsync(null).Wait();
            }
            
        }

        public void Dispose()
        {
            if (_enabled)
                persistence.CloseAsync(null).Wait();
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

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
    public class EntitiesJsonMySqlPersistenceTest : IDisposable
    {
        private EntitiesJsonMySqlPersistence _persistence;
        private EntitiesPersistenceFixture _fixture;

        private bool _enabled = false;
        private string mysqlUri;
        private string mysqlHost;
        private string mysqlPort;
        private string mysqlDatabase;
        private string mysqlUser;
        private string mysqlPassword;

        public EntitiesJsonMySqlPersistenceTest()
        {
            mysqlUri = Environment.GetEnvironmentVariable("MYSQL_URI");
            mysqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
            mysqlPort = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
            mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DB") ?? "test";
            mysqlUser = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "user";
            mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "password";

            _enabled = !string.IsNullOrEmpty(mysqlUri) || !string.IsNullOrEmpty(mysqlHost);

            var dbConfig = ConfigParams.FromTuples(
            "connection.uri", mysqlUri,
            "connection.host", mysqlHost,
            "connection.port", mysqlPort,
            "connection.database", mysqlDatabase,
            "credential.username", mysqlUser,
            "credential.password", mysqlPassword
        );
            _persistence = new EntitiesJsonMySqlPersistence();
            _persistence.Configure(dbConfig);
            _fixture = new EntitiesPersistenceFixture(_persistence);

            _persistence.OpenAsync(null).Wait();
            _persistence.ClearAsync(null).Wait();
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

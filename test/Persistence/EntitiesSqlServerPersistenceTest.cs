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
	public class EntitiesSqlServerPersistenceTest: IDisposable
    {
		private EntitiesSqlServerPersistence persistence;
		private EntitiesPersistenceFixture fixture;

		private string sqlserverUri;
		private string sqlserverHost;
		private string sqlserverPort;
		private string sqlserverDatabase;
		private string sqlserverUsername;
		private string sqlserverPassword;
		private bool _enabled = false;

		public EntitiesSqlServerPersistenceTest()
        {
			sqlserverUri = Environment.GetEnvironmentVariable("SQLSERVER_URI");
			sqlserverHost = Environment.GetEnvironmentVariable("SQLSERVER_SERVICE_HOST"); // ?? "localhost";
			sqlserverPort = Environment.GetEnvironmentVariable("SQLSERVER_SERVICE_PORT") ?? "1433";
			sqlserverDatabase = Environment.GetEnvironmentVariable("SQLSERVER_DB") ?? "master";
			sqlserverUsername = Environment.GetEnvironmentVariable("SQLSERVER_USER") ?? "sa";
			sqlserverPassword = Environment.GetEnvironmentVariable("SQLSERVER_PASS") ?? "sqlserver_123";

			// Exit if postgres connection is not set

			_enabled = !string.IsNullOrEmpty(sqlserverUri) || !string.IsNullOrEmpty(sqlserverHost);

            if (_enabled)
            {
				persistence = new EntitiesSqlServerPersistence();
				persistence.Configure(ConfigParams.FromTuples(
					"connection.uri", sqlserverUri,
					"connection.host", sqlserverHost,
					"connection.port", sqlserverPort,
					"connection.database", sqlserverDatabase,
					"credential.username", sqlserverUsername,
					"credential.password", sqlserverPassword
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

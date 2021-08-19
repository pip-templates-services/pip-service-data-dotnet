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
	public class EntitiesJsonSqlServerPersistenceTest: IDisposable
    {
		private bool _enabled = false;
		private EntitiesJsonSqlServerPersistence _persistence;
		private EntitiesPersistenceFixture _fixture;

		public EntitiesJsonSqlServerPersistenceTest()
		{
			var sqlserverDb = Environment.GetEnvironmentVariable("SQLSERVER_DB") ?? "master";
			var sqlserverHost = Environment.GetEnvironmentVariable("SQLSERVER_HOST") // ?? "localhost";
			var sqlserverPort = Environment.GetEnvironmentVariable("SQLSERVER_PORT") ?? "1433";
			var sqlserverUri = Environment.GetEnvironmentVariable("SQLSERVER_URI");
			var sqlserverUser = Environment.GetEnvironmentVariable("SQLSERVER_USER") ?? "sa";
			var sqlserverPassword = Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD") ?? "sqlserver_123";

			_enabled = !string.IsNullOrEmpty(sqlserverUri) || !string.IsNullOrEmpty(sqlserverHost);

			if (_enabled)
			{
				var config = ConfigParams.FromTuples(
					"connection.database", sqlserverDb,
					"connection.host", sqlserverHost,
					"connection.port", sqlserverPort,
					"connection.uri", sqlserverUri,
					"credential.username", sqlserverUser,
					"credential.password", sqlserverPassword,
					"options.connect_timeout", 5
				);

				_persistence = new EntitiesJsonSqlServerPersistence();
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

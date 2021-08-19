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
	public class EntitiesMongoDbPersistenceTest: IDisposable
    {
		private bool _enabled = false;
		private EntitiesMongoDbPersistence _persistence;
		private EntitiesPersistenceFixture _fixture;

		public EntitiesMongoDbPersistenceTest()
		{
			var MONGO_DB = Environment.GetEnvironmentVariable("MONGO_DB") ?? "test";
			var MONGO_SERVICE_HOST = Environment.GetEnvironmentVariable("MONGO_SERVICE_HOST") ?? "localhost";
			var MONGO_SERVICE_PORT = Environment.GetEnvironmentVariable("MONGO_SERVICE_PORT") ?? "27017";
			var MONGO_SERVICE_URI = Environment.GetEnvironmentVariable("MONGO_SERVICE_URI");

			_enabled = !string.IsNullOrEmpty(MONGO_SERVICE_URI) || !string.IsNullOrEmpty(MONGO_SERVICE_HOST);

			if (_enabled)
			{
				var config = ConfigParams.FromTuples(
					"connection.database", MONGO_DB,
					"connection.host", MONGO_SERVICE_HOST,
					"connection.port", MONGO_SERVICE_PORT,
					"connection.uri", MONGO_SERVICE_URI
				);

				_persistence = new EntitiesMongoDbPersistence();
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

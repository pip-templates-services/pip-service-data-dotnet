using PipServices3.Commons.Config;
using PipTemplatesServiceData.Persistence;

using System.Threading.Tasks;
using Xunit;

namespace PipTemplatesServiceData.Test.Persistence
{
	[Collection("Sequential")]
	public class EntitiesFilePersistenceTest
    {
		private EntitiesFilePersistence _persistence;
		private EntitiesPersistenceFixture _fixture;

		public EntitiesFilePersistenceTest()
		{
			ConfigParams config = ConfigParams.FromTuples(
				"path", "Entities.json"
			);
			_persistence = new EntitiesFilePersistence();
			_persistence.Configure(config);
			_persistence.OpenAsync(null).Wait();
			_persistence.ClearAsync(null).Wait();

			_fixture = new EntitiesPersistenceFixture(_persistence);
		}

		[Fact]
		public async Task TestCrudOperationsAsync()
		{
			await _fixture.TestCrudOperationsAsync();
		}

		[Fact]
		public async Task TestGetWithFiltersAsync()
		{
			await _fixture.TestGetWithFiltersAsync();
		}
	}
}

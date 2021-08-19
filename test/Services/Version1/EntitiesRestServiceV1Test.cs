using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using PipServices3.Commons.Config;
using PipServices3.Commons.Convert;
using PipServices3.Commons.Data;
using PipServices3.Commons.Refer;
using PipTemplatesServiceData.Data.Version1;
using PipTemplatesServiceData.Logic;
using PipTemplatesServiceData.Persistence;
using PipTemplatesServiceData.Services.Version1;

namespace PipTemplatesServiceData.Test.Services.Version1
{
    [Collection("Sequential")]
    public class EntitiesRestServiceV1Test : IDisposable
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

        private ConfigParams httpConfig = ConfigParams.FromTuples(
            "connection.protocol", "http",
            "connection.host", "localhost",
            "connection.port", 3000
        );

        private EntitiesRestServiceV1 service;

        private HttpClient rest;

        public EntitiesRestServiceV1Test()
        {
            var persistence = new EntitiesMemoryPersistence();
            var controller = new EntitiesController();

            rest = new HttpClient();
            rest.BaseAddress = new Uri("http://localhost:3000");

            service = new EntitiesRestServiceV1();
            service.Configure(httpConfig);

            var references = References.FromTuples(
                new Descriptor("pip-service-data", "persistence", "memory", "default", "1.0"), persistence,
                new Descriptor("pip-service-data", "controller", "default", "default", "1.0"), controller,
                new Descriptor("pip-service-data", "service", "http", "default", "1.0"), service
            );
            controller.SetReferences(references);
            service.SetReferences(references);

            service.OpenAsync(null).Wait();
        }

        public void Dispose()
        {
            service.CloseAsync(null).Wait();
        }

        [Fact]
        public async Task TestCrudOperationsAsync()
        {
            // Create the first entity
            var json = JsonConverter.ToJson(ENTITY1);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await rest.PostAsync("/v1/entities/entities", data);
            var entity = JsonConverter.FromJson<EntityV1>(response.Content.ReadAsStringAsync().Result);

            Assert.NotNull(entity);
            Assert.Equal(ENTITY1.Name, entity.Name);
            Assert.Equal(ENTITY1.SiteId, entity.SiteId);
            Assert.Equal(ENTITY1.Type, entity.Type);
            Assert.Equal(ENTITY1.Name, entity.Name);
            Assert.NotNull(entity.Content);

            // Create the second entity
            json = JsonConverter.ToJson(ENTITY2);
            data = new StringContent(json, Encoding.UTF8, "application/json");

            response = await rest.PostAsync("/v1/entities/entities", data);
            entity = JsonConverter.FromJson<EntityV1>(response.Content.ReadAsStringAsync().Result);

            Assert.NotNull(entity);
            Assert.Equal(ENTITY2.Name, entity.Name);
            Assert.Equal(ENTITY2.SiteId, entity.SiteId);
            Assert.Equal(ENTITY2.Type, entity.Type);
            Assert.Equal(ENTITY2.Name, entity.Name);
            Assert.NotNull(entity.Content);


            response = await rest.GetAsync("/v1/entities/entities");
            var page = JsonConverter.FromJson<DataPage<EntityV1>>(response.Content.ReadAsStringAsync().Result);

            Assert.NotNull(page);
            Assert.Equal(2, page.Data.Count);

            var entity1 = page.Data[0];

            // Update the entity
            entity1.Name = "ABC";

            json = JsonConverter.ToJson(entity1);
            data = new StringContent(json, Encoding.UTF8, "application/json");

            response = await rest.PutAsync("/v1/entities/entities", data);
            entity = JsonConverter.FromJson<EntityV1>(response.Content.ReadAsStringAsync().Result);

            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);
            Assert.Equal("ABC", entity.Name);

            // Get entity by name
            response = await rest.GetAsync("/v1/entities/entities/name/" + entity1.Name);
            entity = JsonConverter.FromJson<EntityV1>(response.Content.ReadAsStringAsync().Result);

            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);

            // Delete the entity
            response = await rest.DeleteAsync("/v1/entities/entities/" + entity1.Id);
            entity = JsonConverter.FromJson<EntityV1>(response.Content.ReadAsStringAsync().Result);

            Assert.NotNull(entity);
            Assert.Equal(entity1.Id, entity.Id);

            // Try to get deleted entity
            response = await rest.GetAsync("/v1/entities/entities/" + entity1.Id);
            entity = JsonConverter.FromJson<EntityV1>(response.Content.ReadAsStringAsync().Result);

            Assert.Null(entity);

        }
    }
}

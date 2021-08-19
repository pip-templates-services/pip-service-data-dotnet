using PipServices3.Commons.Refer;
using PipServices3.Components.Build;
using PipTemplatesServiceData.Logic;
using PipTemplatesServiceData.Persistence;
using PipTemplatesServiceData.Services.Version1;

namespace PipTemplatesServiceData.Build
{
    public class EntitiesServiceFactory: Factory
    {
        private static Descriptor MemoryPersistenceDescriptor = new Descriptor("pip-service-data", "persistence", "memory", "*", "1.0");
        private static Descriptor FilePersistenceDescriptor = new Descriptor("pip-service-data", "persistence", "file", "*", "1.0");
        private static Descriptor MongoDbPersistenceDescriptor = new Descriptor("pip-service-data", "persistence", "mongodb", "*", "1.0");
        // private static Descriptor CouchbasePersistenceDescriptor = new Descriptor("pip-service-data", "persistence", "couchbase", "*", "1.0");
        private static Descriptor EntitiesPostgresPersistence = new Descriptor("pip-service-data", "persistence", "postgres", "*", "1.0");
        private static Descriptor EntitiesJsonPostgresPersistence = new Descriptor("pip-service-data", "persistence", "json-postgres", "*", "1.0");
        private static Descriptor EntitiesMySqlPersistence = new Descriptor("pip-service-data", "persistence", "mysql", "*", "1.0");
        private static Descriptor EntitiesJsonMySqlPersistence = new Descriptor("pip-service-data", "persistence", "json-mysql", "*", "1.0");
        private static Descriptor EntitiesSqlServerPersistence = new Descriptor("pip-service-data", "persistence", "sqlserver", "*", "1.0");
        private static Descriptor EntitiesJsonSqlServerPersistence = new Descriptor("pip-service-data", "persistence", "json-sqlserver", "*", "1.0");
        private static Descriptor ControllerDescriptor = new Descriptor("pip-service-data", "controller", "default", "*", "1.0");
        private static Descriptor CommandableHttpServiceV1Descriptor = new Descriptor("pip-service-data", "service", "commandable-http", "*", "1.0");
        private static Descriptor CommandableGrpcServiceV1Descriptor = new Descriptor("pip-service-data", "service", "commandable-grpc", "*", "1.0");
        //private static Descriptor CommandableLambdaServiceV1Descriptor = new Descriptor("pip-service-data", "service", "commandable-lambda", "*", "1.0");
        private static Descriptor GrpcServiceV1Descriptor = new Descriptor("pip-service-data", "service", "grpc", "*", "1.0");
        private static Descriptor RestServiceV1Descriptor = new Descriptor("pip-service-data", "service", "rest", "*", "1.0");
        //private static Descriptor LambdaServiceV1Descriptor = new Descriptor("pip-service-data", "service", "lambda", "*", "1.0");
    
        public EntitiesServiceFactory(): base()
        {
            this.RegisterAsType(EntitiesServiceFactory.MemoryPersistenceDescriptor, typeof(EntitiesMemoryPersistence));
            this.RegisterAsType(EntitiesServiceFactory.FilePersistenceDescriptor, typeof(EntitiesFilePersistence));
            this.RegisterAsType(EntitiesServiceFactory.MongoDbPersistenceDescriptor, typeof(EntitiesMongoDbPersistence));
            //this.RegisterAsType(EntitiesServiceFactory.CouchbasePersistenceDescriptor, typeof(EntitiesCouchbasePersistence));
            this.RegisterAsType(EntitiesServiceFactory.EntitiesPostgresPersistence, typeof(EntitiesPostgresPersistence));
            this.RegisterAsType(EntitiesServiceFactory.EntitiesJsonPostgresPersistence, typeof(EntitiesJsonPostgresPersistence));
            this.RegisterAsType(EntitiesServiceFactory.EntitiesMySqlPersistence, typeof(EntitiesMySqlPersistence));
            this.RegisterAsType(EntitiesServiceFactory.EntitiesJsonMySqlPersistence, typeof(EntitiesJsonMySqlPersistence));
            this.RegisterAsType(EntitiesServiceFactory.EntitiesSqlServerPersistence, typeof(EntitiesSqlServerPersistence));
            this.RegisterAsType(EntitiesServiceFactory.EntitiesJsonSqlServerPersistence, typeof(EntitiesJsonSqlServerPersistence));
            this.RegisterAsType(EntitiesServiceFactory.ControllerDescriptor, typeof(EntitiesController));
            this.RegisterAsType(EntitiesServiceFactory.CommandableHttpServiceV1Descriptor, typeof(EntitiesCommandableHttpServiceV1));
            this.RegisterAsType(EntitiesServiceFactory.CommandableGrpcServiceV1Descriptor, typeof(EntitiesCommandableGrpcServiceV1));
            //this.RegisterAsType(EntitiesServiceFactory.CommandableLambdaServiceV1Descriptor, typeof(EntitiesCommandableLambdaServiceV1));
            this.RegisterAsType(EntitiesServiceFactory.GrpcServiceV1Descriptor, typeof(EntitiesGrpcServiceV1));
            this.RegisterAsType(EntitiesServiceFactory.RestServiceV1Descriptor, typeof(EntitiesRestServiceV1));
            //this.RegisterAsType(EntitiesServiceFactory.LambdaServiceV1Descriptor, typeof(EntitiesLambdaServiceV1));
        }


    }
}

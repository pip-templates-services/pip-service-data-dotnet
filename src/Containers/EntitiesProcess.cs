using PipServices3.Container;
using PipServices3.DataDog.Build;
using PipServices3.ElasticSearch.Build;
using PipServices3.Grpc.Build;
using PipServices3.Prometheus.Build;
using PipServices3.Rpc.Build;
using PipServices3.Swagger.Build;
using PipTemplatesServiceData.Build;

namespace PipTemplatesServiceData.Containers
{
    public class EntitiesProcess : ProcessContainer
    {
        public EntitiesProcess() : base("pip-service-data", "Entities data microservice")
        {
            this._factories.Add(new EntitiesServiceFactory());
            this._factories.Add(new DefaultElasticSearchFactory());
            this._factories.Add(new DefaultPrometheusFactory());
            this._factories.Add(new DefaultDataDogFactory());
            this._factories.Add(new DefaultRpcFactory());
            this._factories.Add(new DefaultSwaggerFactory());
            this._factories.Add(new DefaultGrpcFactory());
        }
    }
}

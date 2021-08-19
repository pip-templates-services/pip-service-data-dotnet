using PipServices3.Commons.Refer;
using PipServices3.Rpc.Services;

namespace PipTemplatesServiceData.Services.Version1
{
    public class EntitiesCommandableHttpServiceV1: CommandableHttpService
    {
        public EntitiesCommandableHttpServiceV1(): base("v1/entities")
        {
            this._dependencyResolver.Put("controller", new Descriptor("pip-service-data", "controller", "*", "*", "1.0"));
        }
    }
}

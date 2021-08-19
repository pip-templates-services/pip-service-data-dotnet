using PipServices3.Commons.Refer;
using PipServices3.Grpc.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipTemplatesServiceData.Services.Version1
{
    public class EntitiesCommandableGrpcServiceV1: CommandableGrpcService
    {
        public EntitiesCommandableGrpcServiceV1(): base("v1.entities")
        {
            this._dependencyResolver.Put("controller", new Descriptor("pip-service-data", "controller", "*", "*", "1.0"));
        }
    }
}

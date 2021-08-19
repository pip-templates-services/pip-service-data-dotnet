using PipServices3.Commons.Config;
using PipServices3.Data.Persistence;
using PipTemplatesServiceData.Data.Version1;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipTemplatesServiceData.Persistence
{
    public class EntitiesFilePersistence: EntitiesMemoryPersistence
    {
        protected JsonFilePersister<EntityV1>  _persister;

        public EntitiesFilePersistence(string path=null):base()
        {
            _persister = new JsonFilePersister<EntityV1>(path);
            _loader = this._persister;
            _saver = this._persister;
        }

        public override void Configure(ConfigParams config)
        {
            base.Configure(config);
            this._persister.Configure(config);
        }
    }
}

using PipServices3.Commons.Validate;
using PipServices3.Commons.Convert;


namespace PipTemplatesServiceData.Data.Version1
{
    public class EntityV1Schema : ObjectSchema
    {
        public EntityV1Schema() : base()
        {
            this.WithOptionalProperty("id", TypeCode.String);
            this.WithOptionalProperty("site_id", TypeCode.String);
            this.WithOptionalProperty("type", TypeCode.String);
            this.WithOptionalProperty("name", TypeCode.String);
            this.WithOptionalProperty("content", TypeCode.String);

        }
    }
}

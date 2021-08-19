
using System.Runtime.Serialization;

using PipServices3.Commons.Data;

namespace PipTemplatesServiceData.Data.Version1
{
    [DataContract]
    public class EntityV1 : IStringIdentifiable, ICloneable
    {
        public EntityV1() { }

        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "site_id")]
        public string SiteId { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "content")]
        public string Content { get; set; }

        public object Clone()
        {
            return new EntityV1()
            {
                Id = this.Id,
                SiteId = this.SiteId,
                Type = this.Type,
                Name = this.Name,
                Content = this.Content
            };
        }
    }
}

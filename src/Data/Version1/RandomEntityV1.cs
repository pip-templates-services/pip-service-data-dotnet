using PipServices3.Commons.Data;
using PipServices3.Commons.Random;

namespace PipTemplatesServiceData.Data.Version1
{
    public class RandomEntityV1
    {
        public static EntityV1 NextEntity(int siteCount)
        {
            return new EntityV1()
            {
                Id = IdGenerator.NextLong(),
                SiteId = RandomEntityV1.NextSiteId(siteCount),
                Type = RandomEntityV1.NextEntityType(),
                Name = RandomString.NextString(10, 25),
                Content = RandomString.NextString(0, 50)
            };
        }

        public static string NextSiteId(int siteCount = 100)
        {
            return RandomInteger.NextInteger(1, siteCount).ToString();
        }

        public static string NextEntityType()
        {
            int choice = RandomInteger.NextInteger(0, 3);
            switch (choice)
            {
                case 0:
                    return EntityTypeV1.Type2;
                case 1:
                    return EntityTypeV1.Type1;
                case 2:
                    return EntityTypeV1.Type3;
                case 3:
                    return EntityTypeV1.Unknown;
                default:
                    return EntityTypeV1.Unknown;
            }
        }
    }
}

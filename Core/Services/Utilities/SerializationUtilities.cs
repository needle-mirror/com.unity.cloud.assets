using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    static class SerializationUtilities
    {
        public static JsonConverter DatasetIdConverter => new DatasetIdConverter();

        public static readonly JsonConverter[] Converters = {
            new OrganizationIdConverter(),
            new ProjectIdConverter(),
            new AssetIdConverter(),
            new AssetVersionIdConverter(),
            DatasetIdConverter,
            new CollectionPathStringConverter()
        };
    }
}

using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    struct AssetCollectionPathDto
    {
        [JsonProperty("path")]
        public string Path { get; set; }
    }
}

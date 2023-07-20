using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    struct AssetPageDto<TAsset> where TAsset : IAsset
    {
        [JsonProperty("nextPaginationToken")]
        public string Token { get; set; }

        [JsonProperty("assets")]
        public TAsset[] Assets { get; set; }
    }
}

using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    struct PageTokenDto
    {
        [JsonProperty("nextPaginationToken")]
        public string Token { get; set; }
    }
}

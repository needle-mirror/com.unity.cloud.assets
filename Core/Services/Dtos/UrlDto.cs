using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    struct UrlDto
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    struct AssetDownloadUrlsDto
    {
        [JsonProperty("files")]
        public List<FileUrl> FileUrls { get; set; }
    }
}

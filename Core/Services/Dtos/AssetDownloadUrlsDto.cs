using System.Collections.Generic;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    struct AssetDownloadUrlsDto
    {
        [JsonProperty("files")]
        public List<AssetFile> Files { get; set; }

        [JsonProperty("attachments")]
        public List<AssetFile> Attachments { get; set; }
    }
}

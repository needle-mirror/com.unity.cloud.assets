using System;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    struct UploadUrlDto
    {
        [JsonProperty("uploadUrl")]
        public string UploadUrl { get; set; }
    }
}

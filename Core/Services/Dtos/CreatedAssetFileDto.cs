using System;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    struct CreatedAssetFileDto
    {
        [JsonProperty("storageId")]
        public string StorageId { get; set; }

        [JsonProperty("assetId")]
        public string AssetId { get; set; }

        [JsonProperty("assetVersion")]
        public int AssetVersion { get; set; }

        [JsonProperty("fileId")]
        public string FileId { get; set; }

        [JsonProperty("uploadUrl")]
        public string UploadUrl { get; set; }

        public CreatedAssetFileDto(string storageId, string assetId, int assetVersion, string fileId, string uploadUrl)
        {
            StorageId = storageId;
            AssetId = assetId;
            AssetVersion = assetVersion;
            FileId = fileId;
            UploadUrl = uploadUrl;
        }
    }
}

using System;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    struct CreatedAssetDto
    {
        [JsonProperty("storageId")]
        public string StorageId { get; set; }

        [JsonProperty("assetId")]
        public string AssetId { get; set; }

        [JsonProperty("assetVersion")]
        public int AssetVersion { get; set; }

        public CreatedAssetDto(string storageId, string assetId, int assetVersion)
        {
            StorageId = storageId;
            AssetId = assetId;
            AssetVersion = assetVersion;
        }
    }
}

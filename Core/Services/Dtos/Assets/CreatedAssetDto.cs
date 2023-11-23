using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct CreatedAssetDto
    {
        [DataMember(Name = "storageId")]
        public string StorageId { get; set; }

        [DataMember(Name = "assetId")]
        public string AssetId { get; set; }

        [DataMember(Name = "assetVersion")]
        public int AssetVersion { get; set; }

        public CreatedAssetDto(string storageId, string assetId, int assetVersion)
        {
            StorageId = storageId;
            AssetId = assetId;
            AssetVersion = assetVersion;
        }
    }
}

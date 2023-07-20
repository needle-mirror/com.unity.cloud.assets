using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class contains all the information pertaining to an element of the <see cref="AssetsInCollectionDto"/>.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    class AssetInCollectionElementDto
    {
        [JsonProperty(propertyName: "storageId")]
        public string StorageId { get; set; }

        [JsonProperty(propertyName: "assetId")]
        public string AssetId { get; set; }

        [JsonProperty(propertyName: "assetVersion")]
        public int Version { get; set; }
    }
}

using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class contains all the information pertaining to the operation of cloud asset with collection.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    class AssetsInCollectionDto
    {
        [JsonProperty(propertyName: "assets")]
        public AssetInCollectionElementDto[] Assets { get; set; }
    }
}

using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class contains the information necessary to add/remove an asset from a collection.
    /// </summary>
    [DataContract]
    class AssetInCollectionData : IAssetInCollectionData
    {
        /// <inheritdoc/>
        public string StorageId { get; set; }

        /// <inheritdoc/>
        public string AssetId { get; set; }

        /// <inheritdoc/>
        public string Version { get; set; }

        public AssetInCollectionData() { }

        public AssetInCollectionData(string assetId, string version, string storageId)
        {
            AssetId = assetId;
            Version = version;
            StorageId = storageId;
        }
    }
}

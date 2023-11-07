using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class contains the information necessary to add or remove an asset from a collection.
    /// </summary>
    interface IAssetInCollectionData
    {
        [DataMember(Name = "storageId")]
        string StorageId { get; }

        [DataMember(Name = "assetId")]
        string AssetId { get; }

        [DataMember(Name = "assetVersion")]
        string Version { get; }
    }
}

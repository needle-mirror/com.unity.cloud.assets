using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class AssetReferenceData : IAssetReferenceData
    {
        public string ReferenceId { get; set; }
        public bool IsValid { get; set; }
        public AssetIdentifierDto Source { get; set; }
        public AssetIdentifierDto Target { get; set; }
    }
}

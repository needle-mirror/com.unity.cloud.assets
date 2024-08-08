using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class CreateAssetReferenceRequestBody : IAssetReferenceRequestBody
    {
        [DataMember(Name = "assetVersion")]
        public string AssetVersion { get; set; }

        [DataMember(Name = "target")]
        public AssetIdentifierDto Target { get; set; }
    }
}

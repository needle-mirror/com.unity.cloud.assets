using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct AssetVersionDto
    {
        [DataMember(Name = "assetVersion")]
        public string Version { get; set; }
    }
}

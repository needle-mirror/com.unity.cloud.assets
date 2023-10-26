using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class AssetPageDto
    {
        [DataMember(Name = "next")]
        public string Token { get; set; }

        [DataMember(Name = "assets")]
        public AssetData[] Assets { get; set; }
    }
}

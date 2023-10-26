using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class AssetDataWithIdentifiers
    {
        public static readonly string SerializedType = typeof(AssetDataWithIdentifiers).FullName;

        [DataMember(Name = "type")]
        string Type { get; set; } = SerializedType;

        [DataMember(Name = "ids")]
        public AssetIdentifier Identifier { get; set; }

        [DataMember(Name = "data")]
        public AssetData Data { get; set; }
    }
}

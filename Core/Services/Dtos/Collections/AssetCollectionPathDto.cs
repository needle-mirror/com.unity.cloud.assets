using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct AssetCollectionPathDto
    {
        [DataMember(Name = "path")]
        public CollectionPath Path { get; set; }
    }
}

using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct CollectionDescriptorDto
    {
        [DataMember(Name = "projectDescriptor")]
        public string ProjectDescriptor { get; set; }

        [DataMember(Name = "collectionPath")]
        public string CollectionPath { get; set; }
        
        [DataMember(Name = "libraryId")]
        public string AssetLibraryId { get; set; }
    }
}

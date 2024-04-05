using System.Runtime.Serialization;

namespace Unity.Cloud.Common
{
    [DataContract]
    struct CollectionDescriptorDto
    {
        [DataMember(Name = "projectDescriptor")]
        public string ProjectDescriptor { get; set; }

        [DataMember(Name = "collectionPath")]
        public string CollectionPath { get; set; }
    }
}

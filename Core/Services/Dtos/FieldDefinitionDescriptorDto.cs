using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct FieldDefinitionDescriptorDto
    {
        [DataMember(Name = "organizationId")]
        public string OrganizationId { get; set; }

        [DataMember(Name = "fieldKey")]
        public string FieldKey { get; set; }
    }
}

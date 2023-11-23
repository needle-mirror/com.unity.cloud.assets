using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class FieldDefinitionListDto
    {
        [DataMember(Name = "fieldDefinitions")]
        public FieldDefinitionData[] FieldDefinitions { get; set; }

        [DataMember(Name = "next")]
        public string NextPageToken { get; set; }
    }
}

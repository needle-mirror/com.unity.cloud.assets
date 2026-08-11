using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct CreatedProjectDto
    {
        [DataMember(Name = "projectId")]
        public string Id { get; set; }
    }
}

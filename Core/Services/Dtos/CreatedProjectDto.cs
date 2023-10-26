using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    struct CreatedProjectDto
    {
        [DataMember(Name = "projectId")]
        public string Id { get; set; }
    }
}

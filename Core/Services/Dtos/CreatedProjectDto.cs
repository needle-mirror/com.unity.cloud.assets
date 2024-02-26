using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct CreatedProjectDto
    {
        [DataMember(Name = "projectId")]
        public string Id { get; set; }
    }
}

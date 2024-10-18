using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    struct VersionNumberDto
    {
        [DataMember(Name = "versionNumber")]
        public int? VersionNumber { get; set; }
    }
}

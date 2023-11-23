using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class ProjectPageDto
    {
        [DataMember(Name = "projects")]
        public ProjectData[] Projects { get; set; }

        [DataMember(Name = "projectsRole")]
        public Dictionary<string, string[]> ProjectsRole { get; set; }
    }
}

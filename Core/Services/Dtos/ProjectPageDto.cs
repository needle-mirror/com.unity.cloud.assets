using System.Collections.Generic;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    class ProjectPageDto
    {
        [JsonProperty("projects")]
        public AssetProject[] Projects { get; set; }

        [JsonProperty("projectsRole")]
        public Dictionary<string, string[]> ProjectsRole { get; set; }
    }
}

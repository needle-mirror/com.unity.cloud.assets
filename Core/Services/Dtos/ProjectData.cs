using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class ProjectData : ProjectBaseData, IProjectData
    {
        /// <inheritdoc/>
        public ProjectId Id { get; }

        public ProjectData(string id)
            : this(new ProjectId(id)) { }

        internal ProjectData(ProjectId id)
        {
            Id = id;
        }
    }
}

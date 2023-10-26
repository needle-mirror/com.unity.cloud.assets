using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class ProjectBaseData : IProjectBaseData
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public IDeserializable Metadata { get; set; }
    }
}

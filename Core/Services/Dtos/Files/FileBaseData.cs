using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class FileBaseData : IFileBaseData
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> Tags { get; set; }

        /// <inheritdoc />
        public IDeserializable PortalMetadata { get; set; }

        /// <inheritdoc />
        public IDeserializable Metadata { get; set; }

        /// <inheritdoc />
        public IDeserializable SystemMetadata { get; set; }
    }
}

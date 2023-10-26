using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class DatasetBaseData : IDatasetBaseData
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public List<string> Tags { get; set; } = new List<string>();// For now initialize list

        /// <inheritdoc />
        public IDeserializable PortalMetadata { get; set; }

        /// <inheritdoc />
        public IDeserializable Metadata { get; set; }

        /// <inheritdoc />
        public IDeserializable SystemMetadata { get; set; }
    }
}

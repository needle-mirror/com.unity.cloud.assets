using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    interface IMetadataInfo
    {
        /// <summary>
        /// The portal metadata.
        /// </summary>
        [DataMember(Name = "portalMetadata")]
        IDeserializable PortalMetadata { get; }

        /// <summary>
        /// The user metadata.
        /// </summary>
        [DataMember(Name = "metadata")]
        IDeserializable Metadata { get; }

        /// <summary>
        /// The system metadata.
        /// </summary>
        [DataMember(Name = "systemMetadata")]
        IDeserializable SystemMetadata { get; }
    }
}

using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// File properties that are common across all the file-entities: creation, update, pending, and uploaded.
    /// </summary>
    public interface IFileUpdate
    {
        /// <summary>
        /// The description of the file.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The tags of the file.
        /// </summary>
        IEnumerable<string> Tags { get; }

        /// <summary>
        /// The metadata of the file.
        /// </summary>
        IDeserializable Metadata { get; }

        /// <summary>
        /// The portal metadata of the file.
        /// </summary>
        IDeserializable PortalMetadata { get; }

        /// <summary>
        /// The system metadata of the file.
        /// </summary>
        IDeserializable SystemMetadata { get; }
    }
}

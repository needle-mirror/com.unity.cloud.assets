using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class FileUpdate : IFileUpdate
    {
        /// <summary>
        /// The description of the asset file.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The tags of the asset file.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// The metadata of the asset file.
        /// </summary>
        public IDeserializable Metadata { get; set; }

        /// <summary>
        /// The system metadata of the asset file.
        /// </summary>
        public IDeserializable SystemMetadata { get; set; }

        /// <summary>
        /// The system metadata of the asset file.
        /// </summary>
        public IDeserializable PortalMetadata { get; set; }

        public FileUpdate() { }

        public FileUpdate(IFile file)
        {
            Description = file.Description;
            Tags = file.Tags;
            Metadata = file.Metadata;
            SystemMetadata = file.SystemMetadata;
            PortalMetadata = file.PortalMetadata;
        }
    }
}

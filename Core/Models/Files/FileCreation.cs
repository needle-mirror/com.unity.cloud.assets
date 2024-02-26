using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class FileCreation : IFileCreation
    {
        /// <inheritdoc/>
        public string Path { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public IEnumerable<string> Tags { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, MetadataValue> Metadata { get; set; }
    }
}

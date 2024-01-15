using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class FileCreation : FileUpdate, IFileCreation
    {
        /// <inheritdoc/>
        public string Path { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, object> Metadata { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, object> SystemMetadata { get; set; }
    }
}

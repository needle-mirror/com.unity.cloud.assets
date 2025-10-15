using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    class FileMetadataHistory : EntityMetadataHistory, IFileMetadataHistory
    {
        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> Tags { get; set; }
    }
}

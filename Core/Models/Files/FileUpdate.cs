using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class FileUpdate : IFileUpdate
    {
        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public IEnumerable<string> Tags { get; set; }

        public FileUpdate() { }

        public FileUpdate(IFile file)
        {
            Description = file.Description;
            Tags = file.Tags;
        }
    }
}

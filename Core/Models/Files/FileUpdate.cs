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

        public FileUpdate() { }

        public FileUpdate(IFile file)
        {
            Description = file.Description;
            Tags = file.Tags;
        }
    }
}

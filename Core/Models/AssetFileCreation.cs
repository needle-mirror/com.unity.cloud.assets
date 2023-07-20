using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class AssetFileCreation : IAssetFileCreation
    {
        /// <summary>
        /// The name of the asset file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the asset file.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The type of the asset file.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The tags of the asset file.
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// The file size of the asset file.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// The details of the asset file.
        /// </summary>
        public Dictionary<string, IDeserializable>  Details { get; set; }

        /// <summary>
        /// The metadata of the asset file.
        /// </summary>
        public Dictionary<string, IDeserializable>  Metadata { get; set; }
    }
}

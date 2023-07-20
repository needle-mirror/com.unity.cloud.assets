using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IAssetFileCreation
    {
        /// <summary>
        /// The name of the asset file.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of the asset file.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The type of the asset file.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// The tags of the asset file.
        /// </summary>
        List<string> Tags { get; }

        /// <summary>
        /// The file size of the asset file.
        /// </summary>
        long FileSize { get; }

        /// <summary>
        /// The details of the asset file.
        /// </summary>
        Dictionary<string, IDeserializable> Details { get; }

        /// <summary>
        /// The metadata of the asset file.
        /// </summary>
        Dictionary<string, IDeserializable>  Metadata { get; }
    }
}

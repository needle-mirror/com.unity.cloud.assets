using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IAssetCreation : IAssetInfo
    {
        /// <summary>
        /// The user metadata of the asset.
        /// </summary>
        Dictionary<string, object> Metadata { get; }

        /// <summary>
        /// The system metadata of the asset.
        /// </summary>
        Dictionary<string, object> SystemMetadata { get; }

        /// <summary>
        /// The collections to which the asset should be added.
        /// </summary>
        List<CollectionPath> Collections { get; }
    }
}

using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class AssetCreation : AssetInfo, IAssetCreation
    {
        /// <inheritdoc/>
        public Dictionary<string, object> Metadata { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, object> SystemMetadata { get; set; }

        /// <inheritdoc/>
        public List<CollectionPath> Collections { get; set; }

        public AssetCreation(string name)
        {
            Name = name;
        }
    }
}

using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class AssetCreation : AssetInfo, IAssetCreation
    {
        /// <inheritdoc/>
        public List<CollectionPath> Collections { get; set; }

        public AssetCreation(string name)
        {
            Name = name;
        }
    }
}

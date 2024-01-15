using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IAssetInfo
    {
        /// <summary>
        /// The name of the asset.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The tags of the asset.
        /// </summary>
        List<string> Tags { get; set; }

        /// <summary>
        /// The type of the asset.
        /// </summary>
        AssetType Type { get; set; }
    }
}

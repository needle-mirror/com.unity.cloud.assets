using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IAssetCreation : IAssetInfo
    {
        /// <summary>
        /// The collections to which the asset should be added.
        /// </summary>
        List<CollectionPath> Collections { get; set; }
    }
}

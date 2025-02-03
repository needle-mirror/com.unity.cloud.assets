using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// The properties of an <see cref="IAssetCollection"/>.
    /// </summary>
    public struct AssetCollectionProperties
    {
        /// <summary>
        /// Describes the collection.
        /// </summary>
        public string Description { get; internal set; }
    }
}

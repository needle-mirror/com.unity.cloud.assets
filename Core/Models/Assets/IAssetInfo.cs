using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IAssetInfo
    {
        /// <inheritdoc cref="IAsset.Name"/>
        string Name { get; }

        /// <inheritdoc cref="IAsset.Description"/>
        string Description { get; }

        /// <inheritdoc cref="IAsset.Tags"/>
        List<string> Tags { get; }
    }
}

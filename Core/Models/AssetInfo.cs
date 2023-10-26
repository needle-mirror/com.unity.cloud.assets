using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class AssetInfo : IAssetInfo
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public AssetType Type { get; set; }

        /// <inheritdoc/>
        public List<string> Tags { get; set; }

        /// <inheritdoc/>
        public IDeserializable PortalMetadata { get; set; }

        /// <inheritdoc/>
        public IDeserializable Metadata { get; set; }

        /// <inheritdoc/>
        public IDeserializable SystemMetadata { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This object contains the information about an asset collection stored on the cloud.
    /// </summary>
    sealed class AssetCollection : IAssetCollection
    {
        readonly IAssetDataSource m_DataSource;

        /// <inheritdoc />
        public CollectionDescriptor Descriptor { get; }

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public string Description { get; private set; }

        /// <inheritdoc />
        public CollectionPath ParentPath { get; private set; }

        internal AssetCollection(IAssetDataSource dataSource, CollectionDescriptor descriptor, string name, string description, string parentPath = null)
            : this(name, description, parentPath)
        {
            m_DataSource = dataSource;
            Descriptor = descriptor;
        }

        /// <summary>
        /// Creates and initializes an <see cref="AssetCollection"/>.
        /// </summary>
        /// <param name="name">The name of the collection. </param>
        /// <param name="description">The description of the collection. </param>
        /// <param name="parentPath">(Optional) The path to the parent collection. </param>
        /// <exception cref="ArgumentNullException">This exception is thrown if the <paramref name="name"/> or <paramref name="description"/> are null or empty. </exception>
        internal AssetCollection(string name, string description, string parentPath = null)
        {
            Name = name;
            Description = description;
            ParentPath = new CollectionPath(parentPath);
        }

        /// <inheritdoc />
        public string GetFullCollectionPath()
        {
            return Descriptor.Path;
        }

        /// <inheritdoc />
        public async Task RefreshAsync(CancellationToken cancellationToken)
        {
            var data = await m_DataSource.GetCollectionAsync(Descriptor, cancellationToken);
            Name = data.Name;
            Description = data.Description;
            ParentPath = new CollectionPath(data.ParentPath);
        }

        /// <inheritdoc />
        public Task UpdateAsync(IAssetCollectionUpdate assetCollectionUpdate, CancellationToken cancellationToken)
        {
            return m_DataSource.UpdateCollectionAsync(Descriptor, assetCollectionUpdate.From(), cancellationToken);
        }

        /// <inheritdoc />
        public Task LinkAssetsAsync(IEnumerable<IAsset> assets, CancellationToken cancellationToken)
        {
            return m_DataSource.AddAssetsToCollectionAsync(Descriptor, assets.Select(SelectAssetId), cancellationToken);
        }

        /// <inheritdoc />
        public Task UnlinkAssetsAsync(IEnumerable<IAsset> assets, CancellationToken cancellationToken)
        {
            return m_DataSource.RemoveAssetsFromCollectionAsync(Descriptor, assets.Select(SelectAssetId), cancellationToken);
        }

        /// <inheritdoc />
        public async Task MoveToNewPathAsync(CollectionPath newCollectionPath, CancellationToken cancellationToken)
        {
            await m_DataSource.MoveCollectionToNewPathAsync(Descriptor, newCollectionPath, cancellationToken);
            ParentPath = newCollectionPath;
        }

        static AssetId SelectAssetId(IAsset asset)
        {
            return asset.Descriptor.AssetId;
        }
    }
}

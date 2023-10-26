using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This object contains the information pertaining to an asset collection stored on the cloud.
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
        /// Creates and initializes a <see cref="AssetCollection"/>.
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
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        /// <inheritdoc />
        public void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            Description = description;
        }

        /// <inheritdoc />
        public string GetFullCollectionPath()
        {
            return Descriptor.CollectionPath;
        }

        /// <inheritdoc />
        public Task UpdateAsync(CancellationToken cancellationToken)
        {
            return m_DataSource.UpdateCollectionAsync(Descriptor, this.From(), cancellationToken);
        }

        /// <inheritdoc />
        public Task AddAssetsAsync(IEnumerable<IAsset> assets, CancellationToken cancellationToken)
        {
            return m_DataSource.AddAssetsToCollectionAsync(Descriptor, assets.Select(SelectAssetId), cancellationToken);
        }

        /// <inheritdoc />
        public Task RemoveAssetsAsync(IEnumerable<IAsset> assets, CancellationToken cancellationToken)
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

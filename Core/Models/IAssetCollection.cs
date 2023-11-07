using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This object contains the information about an asset collection.
    /// </summary>
    public interface IAssetCollection
    {
        /// <summary>
        /// The descriptor of the collection.
        /// </summary>
        CollectionDescriptor Descriptor { get; }

        /// <summary>
        /// The name of the collection.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Describes the collection.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The path to the parent collection; can be empty.
        /// </summary>
        CollectionPath ParentPath { get; }

        /// <summary>
        /// Sets the <see cref="Name"/> of the collection.
        /// </summary>
        /// <param name="name">The name of the collection. </param>
        /// <exception cref="ArgumentNullException">This exception is thrown if the <paramref name="name"/> is null or empty. </exception>
        void SetName(string name);

        /// <summary>
        /// Sets the <see cref="Description"/> of the collection.
        /// </summary>
        /// <param name="description">The description of the collection. </param>
        /// <exception cref="ArgumentNullException">This exception is thrown if the <paramref name="description"/> is null or empty. </exception>
        void SetDescription(string description);

        /// <summary>
        /// Returns the full path to the collection.
        /// </summary>
        /// <returns>The path of the collection. </returns>
        string GetFullCollectionPath();

        /// <summary>
        /// Synchronizes the local changes to the collection with the data source.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task UpdateAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Adds a set of asset references to the collection.
        /// </summary>
        /// <param name="assets">The assets to link to the collection. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task AddAssetsAsync(IEnumerable<IAsset> assets, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a set of asset references from the collection.
        /// </summary>
        /// <param name="assets">The assets to unlink from the collection. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task RemoveAssetsAsync(IEnumerable<IAsset> assets, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new path for the collection.
        /// </summary>
        /// <param name="newCollectionPath">The new parent path. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task MoveToNewPathAsync(CollectionPath newCollectionPath, CancellationToken cancellationToken);
    }
}

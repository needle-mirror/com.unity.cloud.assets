using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An object that provides the methods to interact with an <see cref="IAssetCollection"/>.
    /// </summary>
    interface IAssetCollectionDataSource
    {
        /// <summary>
        /// Implement this method to get the collections in an <see cref="IProject"/>.
        /// </summary>
        /// <param name="organization">The organization the <paramref name="project"/> belongs to. </param>
        /// <param name="project">The project the collection belongs to. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is an array of <see cref="IAssetCollection"/>. </returns>
        Task<IAssetCollection[]> ListCollectionsAsync(IOrganization organization, IProject project, CancellationToken token);

        /// <summary>
        /// Implement this method to get the collection at the specified path.
        /// </summary>
        /// <param name="organization">The organization the <paramref name="project"/> belongs to. </param>
        /// <param name="project">The project the collection belongs to. </param>
        /// <param name="collectionPath">The path to a collection. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is an <see cref="IAssetCollection"/>. </returns>
        Task<IAssetCollection> GetCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, CancellationToken token);

        /// <summary>
        /// Implement this method to create a new collection within an <see cref="IProject"/>.
        /// </summary>
        /// <param name="organization">The organization the <paramref name="project"/> belongs to. </param>
        /// <param name="project">The project the collection belongs to. </param>
        /// <param name="assetCollection">The <see cref="IAssetCollection"/> to push to the cloud. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the path to the collection within the <paramref name="project"/>. </returns>
        Task<CollectionPath> CreateCollectionAsync(IOrganization organization, IProject project, IAssetCollection assetCollection, CancellationToken token);

        /// <summary>
        /// Implement this method to update a collection in an <see cref="IProject"/>.
        /// </summary>
        /// <param name="organization">The organization the <paramref name="project"/> belongs to. </param>
        /// <param name="project">The project the collection belongs to. </param>
        /// <param name="assetCollection">The <see cref="IAssetCollection"/> to push to the cloud. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result.</returns>
        Task UpdateCollectionAsync(IOrganization organization, IProject project, IAssetCollection assetCollection, CancellationToken token);

        /// <summary>
        /// Implement this method to delete a collection from an <see cref="IProject"/>.
        /// </summary>
        /// <param name="organization">The organization the <paramref name="project"/> belongs to. </param>
        /// <param name="project">The project the collection belongs to. </param>
        /// <param name="collectionPath">The path to a collection. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result.</returns>
        Task DeleteCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, CancellationToken token);

        /// <summary>
        /// Implement this method to insert assets into a collection in an <see cref="IProject"/>.
        /// </summary>
        /// <param name="organization">The organization the <paramref name="project"/> belongs to. </param>
        /// <param name="project">The project the collection belongs to. </param>
        /// <param name="collectionPath"></param>
        /// <param name="assets"></param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> InsertAssetsToCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, IEnumerable<IAsset> assets, CancellationToken token);

        /// <summary>
        /// Implement this method to remove assets from a collection in an <see cref="IProject"/>.
        /// </summary>
        /// <param name="organization">The organization the <paramref name="project"/> belongs to. </param>
        /// <param name="project">The project the collection belongs to. </param>
        /// <param name="collectionPath"></param>
        /// <param name="assets"></param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> RemoveAssetsFromCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, IEnumerable<IAsset> assets, CancellationToken token);

        /// <summary>
        /// Implement this method to move a collection in an <see cref="IProject"/> to a new path.
        /// </summary>
        /// <param name="organization">The organization the <paramref name="project"/> belongs to. </param>
        /// <param name="project">The project the collection belongs to. </param>
        /// <param name="collectionPath"></param>
        /// <param name="newCollectionPath"></param>
        /// <param name="token">The cancellation token</param>
        /// <returns></returns>
        Task<string> MoveCollectionToNewPathAsync(IOrganization organization, IProject project, CollectionPath collectionPath, CollectionPath newCollectionPath, CancellationToken token);
    }
}

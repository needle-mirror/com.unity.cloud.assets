using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An interface that provides the methods to interact with an <see cref="IAssetCollection"/>.
    /// </summary>
    public interface IAssetCollectionManager
    {
        /// <summary>
        /// Gets the collections in an <see cref="IProject"/>.
        /// </summary>
        /// <param name="organization"></param>
        /// <param name="project"></param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is an array of collections.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/CollectionManagerExample.cs" region="ListCollections"/>
        /// </example>
        Task<IAssetCollection[]> ListCollectionsAsync(IOrganization organization, IProject project, CancellationToken token);

        /// <summary>
        /// Gets an <see cref="IAssetCollection"/> in an <see cref="IProject"/>.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the collection resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="collectionPath">The path to a collection. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the <see cref="IAssetCollection"/> at path <paramref name="collectionPath"/>. </returns>
        /// <example>
        /// <code source="../Samples/Documentatino/Scripting/CollectionManagerExample.cs" region="GetCollection"/>
        /// </example>
        Task<IAssetCollection> GetCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, CancellationToken token);

        /// <summary>
        /// Creates a new <see cref="IAssetCollection"/> at the specified path. in an <see cref="IProject"/>.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the collection will reside. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetCollection">The collection to commit to the cloud. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the path to the new collection within the <paramref name="project"/>. </returns>
        /// <exception cref="ArgumentNullException">This exception is thrown if the <see cref="IAssetCollection"/> has invalid members. </exception>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/CollectionManagerExample.cs" region="CreateCollection"/>
        /// </example>
        Task<CollectionPath> CreateCollectionAsync(IOrganization organization, IProject project, IAssetCollection assetCollection, CancellationToken token);

        /// <summary>
        /// Updates an <see cref="IAssetCollection"/> in an <see cref="IProject"/>.
        /// </summary>
        /// <param name="assetCollection">The collection to commit to the cloud. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/CollectionManagerExample.cs" region="UpdateCollection"/>
        /// </example>
        Task UpdateCollectionAsync(IAssetCollection assetCollection, CancellationToken token);

        /// <summary>
        /// Deletes the <see cref="IAssetCollection"/> at the specified path from an <see cref="IProject"/>.
        /// </summary>
        /// <param name="assetCollection">The collection to remove from the cloud. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/CollectionManagerExample.cs" region="DeleteCollection"/>
        /// </example>
        Task DeleteCollectionAsync(IAssetCollection assetCollection, CancellationToken token);

        /// <summary>
        /// Implement this method to move a collection in an <see cref="IProject"/> to a new path.
        /// </summary>
        /// <param name="assetCollection"></param>
        /// <param name="newCollectionPath"></param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the new path to the collection. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/CollectionManagerExample.cs" region="MoveCollection"/>
        /// </example>
        Task<string> MoveCollectionToNewPathAsync(IAssetCollection assetCollection, CollectionPath newCollectionPath, CancellationToken token);

        /// <summary>
        /// Implement this method to insert assets into a collection in an <see cref="IProject"/>.
        /// </summary>
        /// <param name="organization">The organization the <paramref name="project"/> belongs to. </param>
        /// <param name="project">The project the collection belongs to. </param>
        /// <param name="collectionPath">The path to the collection to be modified. </param>
        /// <param name="assets">The assets to add. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/CollectionManagerExample.cs" region="CollectionInsert"/>
        /// </example>
        Task InsertAssetsToCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, IEnumerable<IAsset> assets, CancellationToken token);

        /// <summary>
        /// Implement this method to remove assets from a collection in an <see cref="IProject"/>.
        /// </summary>
        /// <param name="organization">The organization the <paramref name="project"/> belongs to. </param>
        /// <param name="project">The project the collection belongs to. </param>
        /// <param name="collectionPath">The path to the collection to modified. </param>
        /// <param name="assets">The assets to remove. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/CollectionManagerExample.cs" region="CollectionRemove"/>
        /// </example>
        Task RemoveAssetsFromCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, IEnumerable<IAsset> assets, CancellationToken token);
    }
}

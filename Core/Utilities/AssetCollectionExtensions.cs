using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    public static class AssetCollectionExtensions
    {
        /// <summary>
        /// Inserts assets into the asset collection of an <see cref="IProject"/>.
        /// </summary>
        /// <param name="collectionManager">The <see cref="IAssetCollectionManager"/> handling the call. </param>
        /// <param name="assetCollection">The collection to be modified. </param>
        /// <param name="assets">The assets to add. </param>
        /// <param name="token">The cancellation token. </param>
        /// <returns>A task with no result.</returns>
        public static Task InsertAssetsToCollectionAsync(this IAssetCollectionManager collectionManager, IAssetCollection assetCollection, IEnumerable<IAsset> assets, CancellationToken token)
        {
            return collectionManager.InsertAssetsToCollectionAsync(assetCollection.Project, assetCollection.GetFullCollectionPath(), assets, token);
        }

        /// <summary>
        /// Removes assets from the asset collection of an <see cref="IProject"/>.
        /// </summary>
        /// <param name="collectionManager">The <see cref="IAssetCollectionManager"/> handling the call. </param>
        /// <param name="assetCollection">The collection to be modified. </param>
        /// <param name="assets">The assets to remove. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result.</returns>
        public static Task RemoveAssetsFromCollectionAsync(this IAssetCollectionManager collectionManager, IAssetCollection assetCollection, IEnumerable<IAsset> assets, CancellationToken token)
        {
            return collectionManager.RemoveAssetsFromCollectionAsync(assetCollection.Project, assetCollection.GetFullCollectionPath(), assets, token);
        }
    }
}

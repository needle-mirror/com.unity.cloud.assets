using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Assets;

namespace Unity.Cloud.Documentation.Assets.Scripting
{
#pragma warning disable S1144 // Remove unused private method
    public class CollectionManagerExample
    {
        #region GetCollection

        async Task<IAssetCollection> GetCollection(IAssetProject project, CollectionPath collectionPath, CancellationToken cancellationToken)
        {
            var collection = await project.GetCollectionAsync(collectionPath, cancellationToken);
            return collection;
        }

        #endregion

        #region ListCollections

        async Task<IEnumerable<IAssetCollection>> ListCollections(IAssetProject project, CancellationToken cancellationToken)
        {
            var collections = await project.ListCollectionsAsync(cancellationToken);
            return collections;
        }

        #endregion

        #region CreateCollection

        async Task<IAssetCollection> CreateCollection(IAssetProject project, CancellationToken cancellationToken)
        {
            var collectionData = new AssetCollectionCreation("My Collection", "A description of my collection.");
            var newCollection = await project.CreateCollectionAsync(collectionData, cancellationToken);

            return newCollection;
        }

        #endregion

        #region UpdateCollection

        async Task UpdateCollection(IAssetCollection assetCollection, CancellationToken cancellationToken)
        {
            assetCollection.SetName("A new name");
            assetCollection.SetDescription("A new description");

            await assetCollection.UpdateAsync(cancellationToken);
        }

        #endregion

        #region DeleteCollection

        async Task DeleteCollection(IAssetProject project, IAssetCollection assetCollection, CancellationToken cancellationToken)
        {
            await project.DeleteCollectionAsync(assetCollection.GetFullCollectionPath(), cancellationToken);
        }

        #endregion

        #region MoveCollection

        async Task MoveCollection(IAssetCollection assetCollection, CollectionPath newCollectionPath, CancellationToken cancellationToken)
        {
            await assetCollection.MoveToNewPathAsync(newCollectionPath, cancellationToken);
        }

        #endregion

        #region CollectionInsert

        async Task CollectionInsert(IAssetCollection assetCollection, CancellationToken cancellationToken, params IAsset[] assets)
        {
            await assetCollection.AddAssetsAsync(assets, cancellationToken);
        }

        #endregion

        #region CollectionRemove

        async Task CollectionRemove(IAssetCollection assetCollection, CancellationToken cancellationToken, params IAsset[] assets)
        {
            await assetCollection.RemoveAssetsAsync(assets, cancellationToken);
        }

        #endregion
    }
#pragma warning restore S1144
}

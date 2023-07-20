using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity;
using Unity.Cloud.Identity.Runtime;

namespace Unity.Cloud.Assets.Documentation.Scripting
{
    public class CollectionManagerExample
    {
        IAssetCollectionManager m_AssetCollectionManager;

        void ConstructAssetCollectionManager()
        {
            #region ConstructAssetCollectionManager

            var httpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();

            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, serviceHostResolver)
                .AddDefaultPkceAuthenticator(playerSettings)
                .Build();

            var authenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

            var serviceHttpClient = new ServiceHttpClient(httpClient, authenticator, playerSettings);

            m_AssetCollectionManager = new CloudAssetCollectionManager(serviceHttpClient, serviceHostResolver);

            #endregion
        }

        #region GetCollection

        async Task<IAssetCollection> GetCollection(IOrganization organization, IProject project, CollectionPath collectionPath, CancellationToken cancellationToken)
        {
            var collection = await m_AssetCollectionManager.GetCollectionAsync(organization, project, collectionPath, cancellationToken);
            return collection;
        }

        #endregion

        #region ListCollections

        async Task<IAssetCollection[]> ListCollections(IOrganization organization, IProject project, CancellationToken cancellationToken)
        {
            var collections = await m_AssetCollectionManager.ListCollectionsAsync(organization, project, cancellationToken);
            return collections;
        }

        #endregion

        #region CreateCollection

        async Task<string> CreateCollection(IOrganization organization, IProject project, CancellationToken cancellationToken)
        {
            var assetCollection = new AssetCollection("My Collection", "A description of my collection.");
            var collectionPath = await m_AssetCollectionManager.CreateCollectionAsync(organization, project, assetCollection, cancellationToken);

            return collectionPath;
        }

        #endregion

        #region UpdateCollection

        async Task UpdateCollection(IAssetCollection assetCollection, CancellationToken cancellationToken)
        {
            assetCollection.SetName("A new name");
            assetCollection.SetDescription("A new description");

            await m_AssetCollectionManager.UpdateCollectionAsync(assetCollection, cancellationToken);
        }

        #endregion

        #region DeleteCollection

        async Task DeleteCollection(IAssetCollection assetCollection, CancellationToken cancellationToken)
        {
            await m_AssetCollectionManager.DeleteCollectionAsync(assetCollection, cancellationToken);
        }

        #endregion

        #region MoveCollection

        async Task MoveCollection(IAssetCollection assetCollection, CollectionPath newCollectionPath, CancellationToken cancellationToken)
        {
            await m_AssetCollectionManager.MoveCollectionToNewPathAsync(assetCollection, newCollectionPath, cancellationToken);
        }

        #endregion

        #region CollectionInsert

        async Task CollectionInsert(IAssetCollection assetCollection, CancellationToken cancellationToken, params IAsset[] assets)
        {
            await m_AssetCollectionManager.InsertAssetsToCollectionAsync(assetCollection, assets, cancellationToken);
        }

        #endregion

        #region CollectionRemove

        async Task CollectionRemove(IAssetCollection assetCollection, CancellationToken cancellationToken, params IAsset[] assets)
        {
            await m_AssetCollectionManager.RemoveAssetsFromCollectionAsync(assetCollection, assets, cancellationToken);
        }

        #endregion
    }
}

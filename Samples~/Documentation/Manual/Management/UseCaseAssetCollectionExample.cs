using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCaseAssetCollectionExample
    {
        readonly UseCaseAssetCollectionExampleBehaviour m_Behaviour = new();

        public void DisplayExample(IAsset asset)
        {
            m_Behaviour.Initialize(asset);
            AssetActions();
        }

        #region Example_UI

        protected virtual void AssetActions()
        {
            if (GUILayout.Button("Refresh asset collections"))
            {
                _ = m_Behaviour.RefreshAssetCollections();
            }

            GUILayout.Label("Collections:");
            if (m_Behaviour.CurrentAsset != null)
            {
                foreach (var collection in m_Behaviour.CurrentAsset.Collections)
                {
                    DisplayAssetCollections(collection);
                }
            }
            else
            {
                GUILayout.Label(" ! No asset selected !");
            }
        }

        void DisplayAssetCollections(string collectionName)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{collectionName}");

            if (GUILayout.Button("Remove asset"))
            {
                _ = m_Behaviour.RemoveAssetFromCollectionAsync(collectionName);
            }

            GUILayout.EndHorizontal();
        }

        #endregion
    }

    class UseCaseAssetCollectionExampleBehaviour
    {
        // Member names should match with the names of the get-started behaviour snippets.
        public IAsset CurrentAsset;

        public void Initialize(IAsset asset)
        {
            CurrentAsset = asset;
        }

        #region Example_Behaviour_RefreshCollections

        public async Task RefreshAssetCollections()
        {
            var cancellationTokenSrc = new CancellationTokenSource();
            await CurrentAsset.RefreshAssetCollectionsAsync(cancellationTokenSrc.Token);
        }

        #endregion

        #region Example_Behaviour_RemoveFromCollection

        public async Task RemoveAssetFromCollectionAsync(CollectionPath collectionPath)
        {
            var cancellationTokenSrc = new CancellationTokenSource();

            var collection = await CurrentAsset.GetCollectionAsync(collectionPath, cancellationTokenSrc.Token);
            if (collection == null)
            {
                Debug.LogError($"Collection {collectionPath} not found.");
                return;
            }

            await collection.RemoveAssetsAsync(new[] {CurrentAsset}, cancellationTokenSrc.Token);
            await RefreshAssetCollections();
            Debug.Log("Asset removed from collection.");
        }

        #endregion

    }
}

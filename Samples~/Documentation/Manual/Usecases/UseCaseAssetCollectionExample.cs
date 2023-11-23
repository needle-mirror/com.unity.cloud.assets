using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    public class UseCaseAssetCollectionExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseAssetCollectionExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseAssetCollectionExample : IAssetManagementUI
    {
        readonly UseCaseAssetCollectionExampleBehaviour m_Behaviour;

        public UseCaseAssetCollectionExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseAssetCollectionExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                _ = m_Behaviour.RefreshAssetCollections();
            }

            GUILayout.BeginVertical();

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

            GUILayout.EndVertical();
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
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseAssetCollectionExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
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

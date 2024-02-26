using System;
using System.Collections.Generic;

namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using UnityEngine;

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
                foreach (var collection in m_Behaviour.Collections)
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

        void DisplayAssetCollections(CollectionPath collectionPath)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{collectionPath}");

            if (GUILayout.Button("Remove asset"))
            {
                _ = m_Behaviour.RemoveAssetFromCollectionAsync(collectionPath);
            }

            GUILayout.EndHorizontal();
        }

        #endregion
    }

    class UseCaseAssetCollectionExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAssetProject CurrentProject => m_Behaviour.CurrentProject;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseAssetCollectionExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshCollections

        public IEnumerable<CollectionPath> Collections { get; private set; }

        public async Task RefreshAssetCollections()
        {
            var collectionsAsync = CurrentAsset.ListLinkedAssetCollectionsAsync(Range.All, CancellationToken.None);
            var collections = new List<CollectionPath>();
            await foreach(var collection in collectionsAsync)
            {
                collections.Add(collection.Path);
            }

            Collections = collections.ToArray();
        }

        #endregion

        #region Example_Behaviour_RemoveFromCollection

        public async Task RemoveAssetFromCollectionAsync(CollectionPath collectionPath)
        {
            var cancellationTokenSrc = new CancellationTokenSource();

            var collection = await CurrentProject.GetCollectionAsync(collectionPath, cancellationTokenSrc.Token);
            if (collection == null)
            {
                Debug.LogError($"Collection {collectionPath} not found.");
                return;
            }

            await collection.UnlinkAssetsAsync(new[] {CurrentAsset}, cancellationTokenSrc.Token);
            await RefreshAssetCollections();
            Debug.Log("Asset removed from collection.");
        }

        #endregion
    }
}

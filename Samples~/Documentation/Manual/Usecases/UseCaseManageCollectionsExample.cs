namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using UnityEngine;

    public class UseCaseManageCollectionsExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseManageCollectionsExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseManageCollectionsExample : IAssetManagementUI
    {
        readonly UseCaseManageCollectionsExampleBehaviour m_Behaviour;

        public UseCaseManageCollectionsExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseManageCollectionsExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAssetProject m_CurrentProject;

        Vector2 m_CollectionListScrollPosition;

        string m_NewCollectionName = "";
        AssetCollectionUpdate m_CollectionUpdate;
        string m_NewParentPath = "";

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (m_Behaviour.CurrentProject != m_CurrentProject)
            {
                m_CurrentProject = m_Behaviour.CurrentProject;
                _ = m_Behaviour.ListProjectAssetCollectionsAsync();
            }

            ListCollections();

            DrawCurrentCollection();

            DrawAssets();

            GUILayout.FlexibleSpace();
        }

        void ListCollections()
        {
            GUILayout.BeginVertical();

            CreateNewCollection();

            GUILayout.Space(15f);

            GUILayout.Label("Available Collections:");

            if (GUILayout.Button("Refresh collection list"))
            {
                _ = m_Behaviour.ListProjectAssetCollectionsAsync();
            }

            if (m_Behaviour.AssetCollections != null)
            {
                m_CollectionListScrollPosition = GUILayout.BeginScrollView(m_CollectionListScrollPosition);

                // Hold a local reference to the collections to avoid concurrent modification exceptions.
                var collections = m_Behaviour.AssetCollections.ToArray();
                foreach (var collection in collections)
                {
                    GUILayout.BeginHorizontal();

                    DrawCollection(collection);

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
        }

        void CreateNewCollection()
        {
            GUILayout.Label("Create New Collection");

            GUILayout.Label("Collection Path:");
            m_NewCollectionName = GUILayout.TextField(m_NewCollectionName);

            if (GUILayout.Button("Create"))
            {
                _ = m_Behaviour.CreateAssetCollectionAsync(m_NewCollectionName);
            }
        }

        void DrawCollection(IAssetCollection collection)
        {
            GUILayout.Label($"{collection.Name}", GUILayout.MaxWidth(Screen.width * 0.2f));

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                m_Behaviour.SetCurrentCollection(collection);
                m_CollectionUpdate = new AssetCollectionUpdate
                {
                    Name = collection.Name,
                    Description = collection.Description
                };
            }

            if (GUILayout.Button("Delete"))
            {
                _ = m_Behaviour.DeleteAssetCollectionAsync(collection);
            }
        }

        void DrawCurrentCollection()
        {
            if (m_Behaviour.CurrentCollection == null)
            {
                GUILayout.Label("! No collection selected !");
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Label($"{m_Behaviour.CurrentCollection.ParentPath}::{m_Behaviour.CurrentCollection.Name}");

            GUILayout.Label("Name: ");
            m_CollectionUpdate.Name = GUILayout.TextField(m_CollectionUpdate.Name);

            GUILayout.Label("Description: ");
            m_CollectionUpdate.Description = GUILayout.TextArea(m_CollectionUpdate.Description, GUILayout.MinHeight(60));

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateProjectAssetCollectionAsync(m_CollectionUpdate);
            }

            GUILayout.Space(10f);

            GUILayout.Label("New parent path: ");
            m_NewParentPath = GUILayout.TextField(m_NewParentPath);

            if (GUILayout.Button("Reparent Collection"))
            {
                _ = m_Behaviour.MoveProjectAssetCollectionAsync(m_NewParentPath);
            }

            GUILayout.EndVertical();
        }

        void DrawAssets()
        {
            GUILayout.BeginVertical();

            var assets = m_Behaviour.CurrentCollectionAssets;
            var currentAsset = m_Behaviour.CurrentAsset;

            GUILayout.Label($"Selected Asset: {currentAsset?.Name ?? "! No asset selected !"}");
            if (currentAsset != null && assets != null)
            {
                if (assets.Any(x => x.Descriptor.AssetId == currentAsset.Descriptor.AssetId))
                {
                    GUILayout.Label("Asset is in collection.");
                }
                else if (GUILayout.Button("Add asset to collection"))
                {
                    _ = m_Behaviour.LinkAssetToCollectionAsync(m_Behaviour.CurrentAsset);
                }
            }

            GUILayout.Space(15f);

            GUILayout.Label("Assets in collection:");
            if (assets != null)
            {
                foreach (var asset in assets)
                {
                    DrawAsset(asset);
                }
            }

            GUILayout.EndVertical();
        }

        void DrawAsset(IAsset asset)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{asset.Name}");
            if (GUILayout.Button($"Remove from collection"))
            {
                _ = m_Behaviour.UnlinkAssetFromCollectionAsync(asset);
            }

            GUILayout.EndHorizontal();
        }

        #endregion
    }

    class UseCaseManageCollectionsExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAssetProject CurrentProject => m_Behaviour.CurrentProject;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseManageCollectionsExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshCollections

        public IEnumerable<IAssetCollection> AssetCollections { get; private set; }
        public IAssetCollection CurrentCollection { get; private set; }
        public List<IAsset> CurrentCollectionAssets { get; } = new();

        public async Task ListProjectAssetCollectionsAsync()
        {
            CurrentCollection = null;

            var results = CurrentProject.ListCollectionsAsync(Range.All, CancellationToken.None);
            var collections = new List<IAssetCollection>();
            await foreach (var collection in results)
            {
                collections.Add(collection);
            }

            AssetCollections = collections;
        }

        public void SetCurrentCollection(IAssetCollection collection)
        {
            if (collection != CurrentCollection)
            {
                CurrentCollection = collection;
                if (CurrentCollection != null)
                {
                    _ = RefreshCollectionAssets();
                }
            }
        }

        async Task RefreshCollectionAssets()
        {
            CurrentCollectionAssets.Clear();

            var searchFilter = new AssetSearchFilter();
            searchFilter.Collections.WhereContains(CurrentCollection.Descriptor.Path);

            var assetList = CurrentProject.QueryAssets().SelectWhereMatchesFilter(searchFilter).ExecuteAsync(CancellationToken.None);
            await foreach (var asset in assetList)
            {
                CurrentCollectionAssets.Add(asset);
            }
        }

        #endregion

        #region Example_Behaviour_CreateCollection

        public async Task CreateAssetCollectionAsync(CollectionPath newPath)
        {
            var name = newPath.GetLastComponentOfPath();
            var newCollection = new AssetCollectionCreation(name, "A collection generated by the use-case example.")
            {
                ParentPath = newPath.GetParentPath()
            };

            try
            {
                await CurrentProject.CreateCollectionAsync(newCollection, CancellationToken.None);
                Debug.Log("Collection created at path: " + newPath);

                // Refresh the list of collections.
                await ListProjectAssetCollectionsAsync();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        #endregion

        #region Example_Behaviour_UpdateCollection

        public async Task UpdateProjectAssetCollectionAsync(IAssetCollectionUpdate update)
        {
            await CurrentCollection.UpdateAsync(update, default);
            Debug.Log("Collection updated.");
        }

        #endregion

        #region Example_Behaviour_DeleteCollection

        public async Task DeleteAssetCollectionAsync(IAssetCollection collection)
        {
            await CurrentProject.DeleteCollectionAsync(collection.Descriptor.Path, CancellationToken.None);

            // Refresh the list of collections.
            await ListProjectAssetCollectionsAsync();
            Debug.Log("Collection deleted.");
        }

        #endregion

        #region Example_Behaviour_MoveCollection

        public async Task MoveProjectAssetCollectionAsync(CollectionPath newPath)
        {
            await CurrentCollection.MoveToNewPathAsync(newPath, CancellationToken.None);
            await ListProjectAssetCollectionsAsync();
            Debug.Log("Collection successfully moved to new path: " + newPath);
        }

        #endregion

        #region Example_Behaviour_CollectionInsert

        public async Task LinkAssetToCollectionAsync(IAsset asset)
        {
            await CurrentCollection.LinkAssetsAsync(new[] {asset}, CancellationToken.None);
            Debug.Log("Asset added to collection.");

            await RefreshCollectionAssets();
        }

        #endregion

        #region Example_Behaviour_CollectionRemove

        public async Task UnlinkAssetFromCollectionAsync(IAsset asset)
        {
            await CurrentCollection.UnlinkAssetsAsync(new[] {asset}, CancellationToken.None);
            Debug.Log("Asset added to collection.");

            await RefreshCollectionAssets();
        }

        #endregion
    }
}

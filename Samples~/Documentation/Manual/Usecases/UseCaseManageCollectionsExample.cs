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
    using Unity.Cloud.Common;
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

            GUI.enabled = collection != m_Behaviour.CurrentCollection;

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                _ = m_Behaviour.SetCurrentCollection(collection);
            }

            GUI.enabled = true;

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

            if (m_Behaviour.CurrentCollectionUpdate == null)
            {
                GUILayout.Label("Loading collection...");
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Label($"{m_Behaviour.CurrentCollection.ParentPath}::{m_Behaviour.CurrentCollection.Name}");

            GUILayout.Label("Name: ");
            m_Behaviour.CurrentCollectionUpdate.Name = GUILayout.TextField(m_Behaviour.CurrentCollectionUpdate.Name);

            GUILayout.Label("Description: ");
            m_Behaviour.CurrentCollectionUpdate.Description = GUILayout.TextArea(m_Behaviour.CurrentCollectionUpdate.Description, GUILayout.MinHeight(60));

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateProjectAssetCollectionAsync(m_Behaviour.CurrentCollectionUpdate);
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

            var currentAssetId = m_Behaviour.CurrentAsset?.Descriptor.AssetId ?? AssetId.None;
            AssetProperties? currentAssetProperties = m_Behaviour.AssetProperties.TryGetValue(currentAssetId, out var properties) ? properties : null;

            GUILayout.Label($"Selected Asset: {currentAssetProperties?.Name ?? "! No asset selected !"}");

            var assetIds = m_Behaviour.CurrentCollectionAssetIds;

            if (currentAssetProperties.HasValue)
            {
                if (assetIds.Any(id => id == currentAssetId))
                {
                    GUILayout.Label("Asset is in collection.");
                }
                else if (GUILayout.Button("Add asset to collection"))
                {
                    _ = m_Behaviour.LinkAssetToCollectionAsync(currentAssetId);
                }
            }

            GUILayout.Space(15f);

            GUILayout.Label("Assets in collection:");
            foreach (var assetId in assetIds)
            {
                DrawAsset(assetId);
            }

            GUILayout.EndVertical();
        }

        void DrawAsset(AssetId assetId)
        {
            AssetProperties? properties = m_Behaviour.AssetProperties.TryGetValue(assetId, out var p) ? p : null;

            GUILayout.BeginHorizontal();

            GUILayout.Label($"{properties?.Name ?? assetId.ToString()}");
            if (GUILayout.Button($"Remove from collection"))
            {
                _ = m_Behaviour.UnlinkAssetFromCollectionAsync(assetId);
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
        public Dictionary<AssetId, AssetProperties> AssetProperties => m_Behaviour.AssetProperties;

        public UseCaseManageCollectionsExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshCollections

        public IEnumerable<IAssetCollection> AssetCollections { get; private set; }
        public IAssetCollection CurrentCollection { get; private set; }
        public AssetCollectionUpdate CurrentCollectionUpdate { get; private set; }
        public List<AssetId> CurrentCollectionAssetIds { get; } = new();

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

        public async Task SetCurrentCollection(IAssetCollection collection)
        {
            CurrentCollectionUpdate = null;
            CurrentCollection = collection;

            if (CurrentCollection != null)
            {
                var properties = await CurrentCollection.GetPropertiesAsync(CancellationToken.None);

                CurrentCollectionUpdate = new AssetCollectionUpdate
                {
                    Name = collection.Name,
                    Description = properties.Description
                };

                await RefreshCollectionAssets();
            }
        }

        async Task RefreshCollectionAssets()
        {
            CurrentCollectionAssetIds.Clear();

            var searchFilter = new AssetSearchFilter();
            searchFilter.Collections.WhereContains(CurrentCollection.Descriptor.Path);

            var assetList = CurrentProject.QueryAssets().SelectWhereMatchesFilter(searchFilter).ExecuteAsync(CancellationToken.None);
            await foreach (var asset in assetList)
            {
                if (!m_Behaviour.AssetProperties.ContainsKey(asset.Descriptor.AssetId))
                {
                    var properties = await asset.GetPropertiesAsync(CancellationToken.None);
                    m_Behaviour.AssetProperties.Add(asset.Descriptor.AssetId, properties);
                }

                CurrentCollectionAssetIds.Add(asset.Descriptor.AssetId);
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
                await CurrentProject.CreateCollectionLiteAsync(newCollection, CancellationToken.None);
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

        public async Task LinkAssetToCollectionAsync(AssetId assetId)
        {
            await CurrentCollection.LinkAssetsAsync(new[] {assetId}, CancellationToken.None);
            Debug.Log("Asset added to collection.");

            await RefreshCollectionAssets();
        }

        #endregion

        #region Example_Behaviour_CollectionRemove

        public async Task UnlinkAssetFromCollectionAsync(AssetId assetId)
        {
            await CurrentCollection.UnlinkAssetsAsync(new[] {assetId}, CancellationToken.None);
            Debug.Log("Asset added to collection.");

            await RefreshCollectionAssets();
        }

        #endregion
    }
}

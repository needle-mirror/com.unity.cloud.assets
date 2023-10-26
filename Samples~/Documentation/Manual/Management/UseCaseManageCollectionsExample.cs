using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCaseManageCollectionsExample
    {
        readonly UseCaseManageCollectionsExampleBehaviour m_Behaviour = new();

        public void DisplayExample(IAssetProject project)
        {
            m_Behaviour.Initialize(project);
            ProjectActions();
        }

        public void DisplayExample(IAsset asset)
        {
            m_Behaviour.CurrentAsset = asset;
            AssetActions();
        }

        #region Example_UIProject

        string m_NewCollectionName = "My Asset Collection";

        protected virtual void ProjectActions()
        {
            GUILayout.BeginVertical(GUI.skin.box);

            if (GUILayout.Button("Refresh collection list"))
            {
                _ = m_Behaviour.ListProjectAssetCollectionsAsync();
            }

            GUILayout.Label("Available Collections:");
            if (m_Behaviour.AssetCollections != null)
            {
                // Hold a local reference to the collections to avoid concurrent modification exceptions.
                var collections = m_Behaviour.AssetCollections.ToArray();
                foreach (var collection in collections)
                {
                    if (GUILayout.Button($"{collection.Name}"))
                    {
                        m_Behaviour.SetCurrentCollection(collection);
                    }
                }
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Create New Collection");

            m_NewCollectionName = TextField(m_NewCollectionName, "Collection Name:");

            if (GUILayout.Button("Create Collection"))
            {
                try
                {
                    _ = m_Behaviour.CreateAssetCollectionAsync(m_NewCollectionName);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    throw;
                }
            }

            GUILayout.EndVertical();
        }

        static string TextField(string value, string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            value = GUILayout.TextField(value);
            GUILayout.EndHorizontal();

            return value;
        }

        #endregion

        #region Example_UIActions

        string m_NewParentPath = "";

        protected virtual void AssetActions()
        {
            GUILayout.BeginHorizontal();

            var collection = m_Behaviour.CurrentCollection;
            if (collection == null)
            {
                GUILayout.Label("! No collection selected !");
            }
            else
            {
                DrawCollection(collection);
                GUILayout.Space(10f);
                DrawAssets();
            }

            GUILayout.EndHorizontal();
        }

        void DrawCollection(IAssetCollection collection)
        {
            GUILayout.BeginVertical();

            GUILayout.Label($"{collection.ParentPath}::{collection.Name}");
            GUILayout.Space(5f);
            GUILayout.Label($"{collection.Description}");
            GUILayout.Space(5f);

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateProjectAssetCollectionAsync();
            }

            if (GUILayout.Button("Delete"))
            {
                _ = m_Behaviour.DeleteProjectAssetCollectionAsync();
            }

            GUILayout.Space(10f);

            m_NewParentPath = TextField(m_NewParentPath, "New Parent Path:");

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
                    _ = m_Behaviour.AddAssetToCollectionAsync(m_Behaviour.CurrentAsset);
                }
            }

            GUILayout.Space(10f);

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
                _ = m_Behaviour.RemoveAssetFromCollectionAsync(asset);
            }
            GUILayout.EndHorizontal();
        }

        #endregion
    }

    class UseCaseManageCollectionsExampleBehaviour
    {
        // Member names should match with the names of the get-started behaviour snippets.
        IAssetProject m_CurrentProject;
        public IAsset CurrentAsset;

        public void Initialize(IAssetProject project)
        {
            if (m_CurrentProject != project || AssetCollections == null)
            {
                m_CurrentProject = project;

                if (m_CurrentProject == null)
                {
                    AssetCollections = Array.Empty<IAssetCollection>();
                }
                else
                {
                    _ = ListProjectAssetCollectionsAsync();
                }
            }
        }

        #region Example_Behaviour_RefreshCollections

        public IEnumerable<IAssetCollection> AssetCollections { get; private set; }
        public IAssetCollection CurrentCollection { get; private set; }
        public List<IAsset> CurrentCollectionAssets { get; private set; } = new();

        public async Task ListProjectAssetCollectionsAsync()
        {
            CurrentCollection = null;

            var cancellationTokenSrc = new CancellationTokenSource();
            AssetCollections = await m_CurrentProject.ListCollectionsAsync(cancellationTokenSrc.Token);
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

        public async Task RefreshCollectionAssets()
        {
            CurrentCollectionAssets.Clear();

            var searchFilter = new AssetSearchFilter();
            searchFilter.Collections.Add(CurrentCollection.GetFullCollectionPath());

            var pagination = new Pagination(Range.All);

            var cancellationTokenSrc = new CancellationTokenSource();
            var assetList = m_CurrentProject.SearchAssetsAsync(searchFilter, pagination, cancellationTokenSrc.Token);
            await foreach(var asset in assetList)
            {
                CurrentCollectionAssets.Add(asset);
            }
        }

        #endregion

        #region Example_Behaviour_CreateCollection

        public async Task CreateAssetCollectionAsync(CollectionPath newPath)
        {
            var name = newPath.GetLastComponentOfPath();
            var newCollection = new AssetCollectionCreation(name, "A collection generated by the use-case example.\nUpdate count: 0")
            {
                ParentPath = newPath.GetParentPath()
            };

            var cancellationTokenSrc = new CancellationTokenSource();
            await m_CurrentProject.CreateCollectionAsync(newCollection, cancellationTokenSrc.Token);

            // Refresh the list of collections.
            await ListProjectAssetCollectionsAsync();
            Debug.Log("Collection created at path: " + newPath);
        }

        #endregion

        #region Example_Behaviour_UpdateCollection

        public async Task UpdateProjectAssetCollectionAsync()
        {
            var description = CurrentCollection.Description.Split(' ').ToList();
            if (int.TryParse(description[^1], out var updateCount))
            {
                description[^1] = (updateCount + 1).ToString();
            }
            else
            {
                description.Add("Update count: 1");
            }

            var strBuilder = new StringBuilder();
            strBuilder.AppendJoin(' ', description);
            CurrentCollection.SetDescription(strBuilder.ToString());

            var cancellationTokenSrc = new CancellationTokenSource();
            await CurrentCollection.UpdateAsync(cancellationTokenSrc.Token);
            Debug.Log("Collection updated.");
        }

        #endregion

        #region Example_Behaviour_DeleteCollection

        public async Task DeleteProjectAssetCollectionAsync()
        {
            var cancellationTokenSrc = new CancellationTokenSource();
            await m_CurrentProject.DeleteCollectionAsync(CurrentCollection.GetFullCollectionPath(), cancellationTokenSrc.Token);

            // Refresh the list of collections.
            await ListProjectAssetCollectionsAsync();
            Debug.Log("Collection deleted.");
        }

        #endregion

        #region Example_Behaviour_MoveCollection

        public async Task MoveProjectAssetCollectionAsync(CollectionPath newPath)
        {
            var cancellationTokenSrc = new CancellationTokenSource();
            await CurrentCollection.MoveToNewPathAsync(newPath, cancellationTokenSrc.Token);
            await ListProjectAssetCollectionsAsync();
            Debug.Log("Collection successfully moved to new path: " + newPath);
        }

        #endregion

        #region Example_Behaviour_CollectionInsert

        public async Task AddAssetToCollectionAsync(IAsset asset)
        {
            var cancellationTokenSrc = new CancellationTokenSource();
            await CurrentCollection.AddAssetsAsync(new[] {asset}, cancellationTokenSrc.Token);
            Debug.Log("Asset added to collection.");

            await RefreshCollectionAssets();
        }

        #endregion

        #region Example_Behaviour_CollectionRemove

        public async Task RemoveAssetFromCollectionAsync(IAsset asset)
        {
            var cancellationTokenSrc = new CancellationTokenSource();
            await CurrentCollection.RemoveAssetsAsync(new[] {asset}, cancellationTokenSrc.Token);
            Debug.Log("Asset added to collection.");

            await RefreshCollectionAssets();
        }

        #endregion
    }
}

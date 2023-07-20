using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCaseManageCollectionsExample
    {
        readonly UseCaseManageCollectionsExampleBehaviour m_Behaviour = new();

        public void DisplayExample(IOrganization organization, IProject project)
        {
            m_Behaviour.Initialize(organization, project);
            ProjectActions();
        }

        public void DisplayExample(IAsset asset)
        {
            m_Behaviour.CurrentAsset = asset;
            AssetActions();
        }

        #region Example_UIProject

        string m_NewCollectionName = "My Asset Collection";
        string m_ParentCollectionPath = "";

        protected virtual void ProjectActions()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

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

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Create New Collection");

            m_NewCollectionName = TextField(m_NewCollectionName, "Collection Name:");
            m_ParentCollectionPath = TextField(m_ParentCollectionPath, "Parent Collection Path:");

            if (GUILayout.Button("Create Collection"))
            {
                try
                {
                    _ = m_Behaviour.CreateAssetCollectionAsync(m_NewCollectionName, m_ParentCollectionPath);
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
            var collection = m_Behaviour.CurrentCollection;
            if (collection == null)
            {
                GUILayout.Label("! No collection selected !");
            }
            else
            {
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

                GUILayout.Space(10f);

                GUILayout.Label($"Selected Asset: {m_Behaviour.CurrentAsset?.Name ?? "! No asset selected !"}");
                if (GUILayout.Button("Add asset to collection"))
                {
                    _ = m_Behaviour.AddAssetToCollectionAsync();
                }
            }
        }

        #endregion
    }

    class UseCaseManageCollectionsExampleBehaviour
    {
        // Member names should match with the names of the get-started behaviour snippets.
        IOrganization m_CurrentOrganization;
        IProject m_CurrentProject;
        public IAsset CurrentAsset;

        public void Initialize(IOrganization organization, IProject project)
        {
            m_CurrentOrganization = organization;
            m_CurrentProject = project;

            if (m_CurrentOrganization != organization || m_CurrentProject != project || AssetCollections == null)
            {
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

        public async Task ListProjectAssetCollectionsAsync()
        {
            CurrentCollection = null;

            var cancellationTokenSrc = new CancellationTokenSource();
            AssetCollections = await PlatformServices.AssetCollectionManager.ListCollectionsAsync(m_CurrentOrganization, m_CurrentProject, cancellationTokenSrc.Token);
        }

        public void SetCurrentCollection(IAssetCollection collection)
        {
            CurrentCollection = collection;
        }

        #endregion

        #region Example_Behaviour_CreateCollection

        public async Task CreateAssetCollectionAsync(string name, string parentCollectionPath)
        {
            var newCollection = new AssetCollection(name, "A collection generated by the use-case example.\nUpdate count: 0", parentCollectionPath);

            var cancellationTokenSrc = new CancellationTokenSource();
            var path = await PlatformServices.AssetCollectionManager.CreateCollectionAsync(m_CurrentOrganization, m_CurrentProject, newCollection, cancellationTokenSrc.Token);

            // Refresh the list of collections.
            await ListProjectAssetCollectionsAsync();
            Debug.Log("Collection created at path: " + path);
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
            await PlatformServices.AssetCollectionManager.UpdateCollectionAsync(CurrentCollection, cancellationTokenSrc.Token);
            Debug.Log("Collection updated.");
        }

        #endregion

        #region Example_Behaviour_DeleteCollection

        public async Task DeleteProjectAssetCollectionAsync()
        {
            var cancellationTokenSrc = new CancellationTokenSource();
            await PlatformServices.AssetCollectionManager.DeleteCollectionAsync(CurrentCollection, cancellationTokenSrc.Token);

            // Refresh the list of collections.
            await ListProjectAssetCollectionsAsync();
            Debug.Log("Collection deleted.");
        }

        #endregion

        #region Example_Behaviour_MoveCollection

        public async Task MoveProjectAssetCollectionAsync(string newPath)
        {
            var cancellationTokenSrc = new CancellationTokenSource();
            var result = await PlatformServices.AssetCollectionManager.MoveCollectionToNewPathAsync(CurrentCollection, newPath, cancellationTokenSrc.Token);
            await ListProjectAssetCollectionsAsync();
            Debug.Log("Collection successfully moved to new path: " + result);
        }

        #endregion

        #region Example_Behaviour_CollectionInsert

        public async Task AddAssetToCollectionAsync()
        {
            if (CurrentAsset == null) return;

            var cancellationTokenSrc = new CancellationTokenSource();
            await PlatformServices.AssetCollectionManager.InsertAssetsToCollectionAsync(CurrentCollection, new[] {CurrentAsset}, cancellationTokenSrc.Token);
            Debug.Log("Asset added to collection.");
        }

        #endregion
    }
}

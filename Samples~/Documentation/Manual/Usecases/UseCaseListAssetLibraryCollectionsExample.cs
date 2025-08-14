namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;
    using UnityEngine;

    public class UseCaseListAssetLibraryCollectionsExampleUI : IAssetManagementUI
    {
        readonly AssetLibrariesBehaviour m_Behaviour;

        public UseCaseListAssetLibraryCollectionsExampleUI(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseListAssetLibraryCollectionsExample : IAssetManagementUI
    {
        readonly UseCaseListAssetLibraryCollectionsExampleBehaviour m_Behaviour;

        public UseCaseListAssetLibraryCollectionsExample(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = new UseCaseListAssetLibraryCollectionsExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAssetLibrary m_CurrentLibrary;

        Vector2 m_ListScrollPosition;

        public void OnGUI()
        {
            if (!m_Behaviour.IsAssetLibrarySelected) return;

            if (m_Behaviour.CurrentAssetLibrary != m_CurrentLibrary)
            {
                m_CurrentLibrary = m_Behaviour.CurrentAssetLibrary;
                _ = m_Behaviour.ListAssetCollectionsAsync();
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Label($"Library: {m_Behaviour.GetAssetLibraryName(m_Behaviour.CurrentAssetLibrary.Id)}");

            // Go back to select a different library.
            if (GUILayout.Button("Back"))
            {
                m_Behaviour.SetSelectedAssetLibrary(null);
                return;
            }

            GUILayout.Space(15f);

            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = m_Behaviour.ListAssetCollectionsAsync();
                return;
            }

            ListCollections();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            DrawCurrentCollection();
            GUILayout.EndVertical();
        }

        void ListCollections()
        {
            GUILayout.BeginVertical();

            GUILayout.Space(15f);

            GUILayout.Label("Available Collections:");

            if (GUILayout.Button("Refresh collection list"))
            {
                _ = m_Behaviour.ListAssetCollectionsAsync();
            }

            m_ListScrollPosition = GUILayout.BeginScrollView(m_ListScrollPosition);

            // Hold a local reference to the collections to avoid concurrent modification exceptions.
            foreach (var kvp in m_Behaviour.AssetCollectionProperties)
            {
                GUILayout.BeginHorizontal();

                DrawCollection(kvp.Key, kvp.Value);

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
        }

        void DrawCollection(CollectionPath collectionPath, AssetCollectionProperties properties)
        {
            GUILayout.Label($"{collectionPath.GetLastComponentOfPath()}", GUILayout.MaxWidth(Screen.width * 0.2f));

            GUI.enabled = collectionPath != m_Behaviour.CurrentCollection;

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                m_Behaviour.CurrentCollection = collectionPath;
            }

            GUI.enabled = true;
        }

        void DrawCurrentCollection()
        {
            if (!m_Behaviour.CurrentCollection.HasValue) return;

            if (!m_Behaviour.AssetCollectionProperties.TryGetValue(m_Behaviour.CurrentCollection.Value, out var properties))
            {
                GUILayout.Label(" ! Collection properties not loaded !");
                return;
            }

            var collectionPath = m_Behaviour.CurrentCollection.Value;

            GUILayout.BeginVertical();

            GUILayout.Label($"{collectionPath.GetParentPath()}::{collectionPath.GetLastComponentOfPath()}");

            GUILayout.Label("Name: ");
            GUILayout.Label(collectionPath.GetLastComponentOfPath());

            GUILayout.Label("Description: ");
            GUILayout.Label(properties.Description, GUILayout.MinHeight(60));

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseListAssetLibraryCollectionsExampleBehaviour
    {
        readonly AssetLibrariesBehaviour m_Behaviour;

        public bool IsAssetLibrarySelected => m_Behaviour.IsAssetLibrarySelected;
        public IAssetLibrary CurrentAssetLibrary => m_Behaviour.CurrentAssetLibrary;
        public string GetAssetLibraryName(AssetLibraryId libraryId) => m_Behaviour.GetAssetLibraryName(libraryId);
        public void SetSelectedAssetLibrary(IAssetLibrary assetLibrary) => m_Behaviour.SetSelectedAssetLibrary(assetLibrary);

        public UseCaseListAssetLibraryCollectionsExampleBehaviour(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshCollections

        public Dictionary<CollectionPath, AssetCollectionProperties> AssetCollectionProperties { get; } = new();
        public CollectionPath? CurrentCollection { get; set; }

        public async Task ListAssetCollectionsAsync()
        {
            var selectedCollection = CurrentCollection;
            CurrentCollection = null;
            AssetCollectionProperties.Clear();

            var results = CurrentAssetLibrary.QueryCollections().ExecuteAsync(CancellationToken.None);
            await foreach (var collection in results)
            {
                var properties = await collection.GetPropertiesAsync(CancellationToken.None);
                AssetCollectionProperties.Add(collection.Descriptor.Path, properties);

                if (collection.Descriptor.Path == selectedCollection)
                {
                    CurrentCollection = collection.Descriptor.Path;
                }
            }
        }

        #endregion
    }
}

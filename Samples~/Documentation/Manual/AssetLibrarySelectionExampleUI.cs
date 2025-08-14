namespace Unity.Cloud.Documentation.Assets
{
    #region Example

    using UnityEngine;

    public class AssetLibrarySelectionExampleUI : IAssetManagementUI
    {
        readonly AssetLibrariesBehaviour m_Behaviour;

        Vector2 m_ListsScrollPosition;

        public AssetLibrarySelectionExampleUI(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI()
        {
            if (m_Behaviour.IsAssetLibrarySelected) return;

            // Refresh the library list
            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = m_Behaviour.GetAssetLibrariesAsync();
                return;
            }

            GUILayout.Space(15f);

            GUILayout.BeginVertical();

            GUILayout.Label("Available Libraries:");
            GUILayout.Space(5f);
            ListAssetLibraries();

            GUILayout.EndVertical();
        }

        void ListAssetLibraries()
        {
            var libraries = m_Behaviour.AvailableLibraries.ToArray();
            if (libraries.Length == 0)
            {
                GUILayout.Label("No libraries found.");
                return;
            }

            m_ListsScrollPosition = GUILayout.BeginScrollView(m_ListsScrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width(250));

            foreach (var library in libraries)
            {
                GUI.enabled = library.Id != m_Behaviour.CurrentAssetLibrary?.Id;

                if (GUILayout.Button(m_Behaviour.GetAssetLibraryName(library.Id)))
                {
                    m_Behaviour.SetSelectedAssetLibrary(library);
                }

                GUI.enabled = true;
            }

            GUILayout.EndScrollView();
        }
    }

    #endregion
}

namespace Unity.Cloud.Assets.Documentation.Discovery
{
    #region Example

    using System;
    using Unity.Cloud.Identity;
    using UnityEditor;
    using UnityEngine;

    public class AssetDiscoveryUI : MonoBehaviour
    {
        protected readonly AssetDiscoveryBehaviour m_Behaviour = new();
        IAuthenticationStateProvider m_AuthenticationStateProvider;

        bool IsLoggedIn => m_AuthenticationStateProvider?.AuthenticationState == AuthenticationState.LoggedIn;

        void Start()
        {
            m_AuthenticationStateProvider = PlatformServices.AuthenticationStateProvider;
            m_AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        void OnDestroy()
        {
            if (m_AuthenticationStateProvider != null)
            {
                m_AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            }
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal();

            UpdateAuthenticationUI(m_AuthenticationStateProvider.AuthenticationState);

            if (!IsLoggedIn)
            {
                GUILayout.EndHorizontal();
                return;
            }

            if (!m_Behaviour.IsOrganizationSelected)
            {
                // Refresh the org list
                if (GUILayout.Button("Refresh"))
                {
                    _ = m_Behaviour.GetOrganizationsAsync();
                    return;
                }

                GUILayout.Space(50);

                // If an organization is not selected, list those available.
                SelectAnOrganization();
            }
            else if (!m_Behaviour.IsProjectSelected)
            {
                GUILayout.BeginVertical();

                // Go back to select a different scene.
                if (GUILayout.Button("Back"))
                {
                    m_Behaviour.SetSelectedOrganization(null);
                    return;
                }

                // Refresh the org list
                if (GUILayout.Button("Refresh"))
                {
                    _ = m_Behaviour.GetProjectsAsync();
                    return;
                }

                GUILayout.EndVertical();

                GUILayout.Space(50);

                SelectAProject();
            }
            else
            {
                // Go back to select a different scene.
                if (GUILayout.Button("Back"))
                {
                    m_Behaviour.SetSelectedProject(null);
                    return;
                }

                GUILayout.Space(50);

                SelectAnAsset();

                GUILayout.Space(50);

                AssetActions();
            }

            GUILayout.EndHorizontal();
        }

        protected virtual void AssetActions()
        {
            // Add additional actions here.
        }

        void SelectAnOrganization()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Available Organizations:");
            GUILayout.Space(10);

            var availableOrganizations = m_Behaviour.AvailableOrganizations;
            if (availableOrganizations != null)
            {
                for (var i = 0; i < availableOrganizations.Length; ++i)
                {
                    if (GUILayout.Button(availableOrganizations[i].Name))
                    {
                        m_Behaviour.SetSelectedOrganization(availableOrganizations[i]);
                    }
                }
            }
            else
            {
                GUILayout.Label("Loading...");
            }

            GUILayout.EndVertical();
        }

        void SelectAProject()
        {
            GUILayout.BeginVertical();

            GUILayout.Label($"{m_Behaviour.CurrentOrganization.Name}");
            GUILayout.Space(10);

            GUILayout.Label("Available Projects:");
            GUILayout.Space(10);

            var projectPage = m_Behaviour.AvailableProjects;
            if (projectPage != null)
            {
                var projects = projectPage.Elements;

                for (var i = 0; i < projects.Length; ++i)
                {
                    if (GUILayout.Button(projects[i].Name))
                    {
                        m_Behaviour.SetSelectedProject(projects[i]);
                    }
                }

                GUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(projectPage.PreviousPage == null);

                // Go back to select a different scene.
                if (GUILayout.Button("Previous Page"))
                {
                    m_Behaviour.GetPreviousProjects();
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(projectPage.NextPageToken));

                // Go back to select a different scene.
                if (GUILayout.Button("Next Page"))
                {
                    _ = m_Behaviour.GetNextAvailableProjectsAsync();
                }

                EditorGUI.EndDisabledGroup();

                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Loading...");
            }

            GUILayout.EndVertical();
        }

        void SelectAnAsset()
        {
            GUILayout.BeginVertical();

            GUILayout.Label($"{m_Behaviour.CurrentOrganization.Name} >> {m_Behaviour.CurrentProject.Name}");
            GUILayout.Space(10);

            GUILayout.Label("Available Assets:");
            GUILayout.Space(10);

            var assetPage = m_Behaviour.AvailableAssets;
            if (assetPage != null)
            {
                var assets = assetPage.Elements;

                for (var i = 0; i < assets.Length; ++i)
                {
                    if (GUILayout.Button(assets[i].Name))
                    {
                        m_Behaviour.CurrentAsset = assets[i];
                        Debug.Log($"Selected: {assets[i].Name}");
                    }
                }

                GUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(assetPage.PreviousPage == null);

                // Go back to select a different scene.
                if (GUILayout.Button("Previous Page"))
                {
                    m_Behaviour.GetPreviousAssets();
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(assetPage.NextPageToken));

                // Go back to select a different scene.
                if (GUILayout.Button("Next Page"))
                {
                    _ = m_Behaviour.GetNextAvailableAssetsAsync();
                }

                EditorGUI.EndDisabledGroup();

                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Loading...");
            }

            GUILayout.EndVertical();
        }

        static void UpdateAuthenticationUI(AuthenticationState state)
        {
            GUILayout.BeginVertical();

            switch (state)
            {
                case AuthenticationState.AwaitingInitialization:
                    GUILayout.Label("Initializing Service...");
                    break;

                case AuthenticationState.AwaitingLogout:
                    GUILayout.Label("Logging out...");
                    break;

                case AuthenticationState.LoggedOut:
                    if (GUILayout.Button("Login"))
                    {
                        _ = PlatformServices.Authenticator.LoginAsync();
                    }

                    break;

                case AuthenticationState.AwaitingLogin:
                    GUILayout.Label("Logging in...");
                    if (GUILayout.Button("Cancel"))
                    {
                        PlatformServices.Authenticator.CancelLogin();
                    }

                    break;

                case AuthenticationState.LoggedIn:
                    if (GUILayout.Button("Logout"))
                    {
                        _ = PlatformServices.Authenticator.LogoutAsync();
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            GUILayout.EndVertical();
        }

        void OnAuthenticationStateChanged(AuthenticationState obj)
        {
            if (obj == AuthenticationState.LoggedIn)
            {
                _ = m_Behaviour.GetOrganizationsAsync();
            }
        }
    }

    #endregion
}

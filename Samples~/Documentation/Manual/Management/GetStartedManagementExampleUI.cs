namespace Unity.Cloud.Assets.Documentation.Management
{
    #region Example

    using System;
    using Unity.Cloud.Identity;
    using UnityEditor;
    using UnityEngine;

    public class AssetManagementUI : MonoBehaviour
    {
        protected readonly AssetManagementBehaviour m_Behaviour = new();
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

                GUILayout.BeginVertical();

                GUILayout.Label($"{m_Behaviour.CurrentOrganization.Name} >> {m_Behaviour.CurrentProject.Name}");
                GUILayout.Space(10f);

                SelectAnAsset();

                GUILayout.Space(5f);

                ProjectActions();

                GUILayout.EndVertical();

                GUILayout.Space(50);

                AssetActions();
            }

            GUILayout.EndHorizontal();
        }

        protected virtual void ProjectActions()
        {
            // Add additional project related actions here.
        }

        protected virtual void AssetActions()
        {
            ManageAnAsset();

            // Add additional asset related actions here.
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
            GUILayout.BeginVertical(EditorStyles.helpBox);

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

        void ManageAnAsset()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Manage:");
            GUILayout.Space(10);

            if (GUILayout.Button("Create new asset", GUILayout.Width(150f)))
            {
                _ = m_Behaviour.CreateAssetAsync();
            }
            GUILayout.Space(5f);

            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
            }
            else
            {
                GUILayout.Label("Asset selected:");
                GUILayout.Space(5f);

                DisplayAsset(m_Behaviour.CurrentAsset);
            }

            GUILayout.EndVertical();
        }

        protected virtual void DisplayAsset(IAsset asset)
        {
            GUILayout.BeginHorizontal();

            var nameValue = GUILayout.TextField(asset.Name, GUILayout.Width(100f));
            if(nameValue != asset.Name)
            {
                asset.Name = nameValue;
            }
            GUILayout.Space(5f);

            var versionNameValue = GUILayout.TextField(asset.VersionName, GUILayout.Width(50f));
            if(versionNameValue != asset.VersionName)
            {
                asset.VersionName = versionNameValue;
            }
            GUILayout.Space(5f);

            GUILayout.Label(asset.Status);
            GUILayout.Space(5f);

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateAssetAsync(asset);
            }
            GUILayout.Space(5f);

            if (GUILayout.Button("Delete"))
            {
                _ = m_Behaviour.DeleteAssetAsync(asset);
            }
            GUILayout.Space(5f);

            GUILayout.EndHorizontal();
        }
    }

    #endregion
}

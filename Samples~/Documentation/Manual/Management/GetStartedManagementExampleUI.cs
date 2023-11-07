using System.Linq;

namespace Unity.Cloud.Assets.Documentation.Management
{
    #region Example

    using System;
    using Unity.Cloud.Identity;
    using UnityEngine;

    public class AssetManagementUI : MonoBehaviour
    {
        protected readonly AssetManagementBehaviour m_Behaviour = new();
        IAuthenticationStateProvider m_AuthenticationStateProvider;

        Vector2 m_ProjectListScrollPosition;
        Vector2 m_AssetListScrollPosition;

        AssetUpdate m_AssetUpdate;

        string[] m_AssetTypeList = Array.Empty<string>();

        bool IsLoggedIn => m_AuthenticationStateProvider?.AuthenticationState == AuthenticationState.LoggedIn;

        void Start()
        {
            m_AuthenticationStateProvider = PlatformServices.AuthenticationStateProvider;
            m_AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;

            m_AssetTypeList = AssetTypeExtensions.AssetTypeList().ToArray();
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
            GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));

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
                    m_Behaviour.GetProjects();
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

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }

        protected virtual void ProjectActions()
        {
            // Add additional project related actions here.
        }

        protected virtual void AssetActions()
        {
            CreateAnAsset();

            DisplaySelectedAsset();

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

            var projects = m_Behaviour.AvailableProjects;
            if (projects.Count > 0)
            {
                m_ProjectListScrollPosition = GUILayout.BeginScrollView(m_ProjectListScrollPosition, GUILayout.Height(Screen.height * 0.8f));

                for (var i = 0; i < projects.Count; ++i)
                {
                    if (GUILayout.Button(projects[i].Name))
                    {
                        m_Behaviour.SetSelectedProject(projects[i]);
                    }
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("No projects found.");
            }

            GUILayout.EndVertical();
        }

        void SelectAnAsset()
        {
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Available Assets:");
            GUILayout.Space(10);

            var assets = m_Behaviour.AvailableAssets;
            if (assets.Count > 0)
            {
                m_AssetListScrollPosition = GUILayout.BeginScrollView(m_AssetListScrollPosition, GUILayout.Height(Screen.height * 0.3f));

                for (var i = 0; i < assets.Count; ++i)
                {
                    if (GUILayout.Button(assets[i].Name))
                    {
                        m_Behaviour.CurrentAsset = assets[i];
                        m_AssetUpdate = new AssetUpdate(m_Behaviour.CurrentAsset);
                        Debug.Log($"Selected: {assets[i].Descriptor.AssetId}");
                    }
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("No assets found.");
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

        protected void CreateAnAsset()
        {
            if (GUILayout.Button("Create new asset", GUILayout.Width(150f)))
            {
                _ = m_Behaviour.CreateAssetAsync();
            }
        }

        protected void DisplaySelectedAsset()
        {
            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
            }
            else
            {
                GUILayout.Label("Asset selected:");
                GUILayout.Space(5f);

                DisplayAsset(m_Behaviour.CurrentAsset, m_AssetUpdate);
            }
        }

        void DisplayAsset(IAsset asset, IAssetUpdate assetUpdate)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Name:");

            assetUpdate.Name = GUILayout.TextField(assetUpdate.Name, GUILayout.Width(100f));

            GUILayout.Space(5f);

            GUILayout.Label(asset.Status);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Type: ");

            var type = (int)assetUpdate.Type;
            type = GUILayout.SelectionGrid(type, m_AssetTypeList, 4);
            assetUpdate.Type = (AssetType)type;

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            var tags = string.Join(',', assetUpdate.Tags);
            tags = GUILayout.TextField(tags);
            assetUpdate.Tags = tags.Split(',').ToList();

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateAssetAsync(asset, assetUpdate);
            }
        }
    }

    #endregion
}

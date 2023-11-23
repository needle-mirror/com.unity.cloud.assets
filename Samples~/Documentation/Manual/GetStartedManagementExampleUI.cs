namespace Unity.Cloud.Assets.Documentation
{
    #region Example

    using System;
    using System.Collections.Generic;
    using Unity.Cloud.Identity;
    using UnityEngine;

    public class AssetManagementUI : MonoBehaviour
    {
        protected readonly AssetManagementBehaviour m_Behaviour = new();
        protected readonly List<IAssetManagementUI> m_UI = new();

        IAuthenticationStateProvider m_AuthenticationStateProvider;

        bool IsLoggedIn => m_AuthenticationStateProvider?.AuthenticationState == AuthenticationState.LoggedIn;

        protected virtual void Awake()
        {
            m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
            m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
            m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseCreateAssetExampleUI(m_Behaviour));
            m_UI.Add(new UseCaseManageAssetExampleUI(m_Behaviour));
        }

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

            m_Behaviour.Clear();
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

            foreach (var ui in m_UI)
            {
                ui.OnGUI();
            }

            AdditionalGUI();

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }

        protected virtual void AdditionalGUI()
        {
            // Do more stuff here.
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

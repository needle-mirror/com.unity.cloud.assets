using System;
using Unity.Cloud.Common;
using Unity.Cloud.Identity;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class LoginController : MonoBehaviour
    {
        const string k_AwaitingLoginText = "Logging in...";

        ICompositeAuthenticator m_Authenticator;
        IAuthenticatedUserInfoProvider m_UserInfoProvider;

        [SerializeField] UIDocument m_LoginUiDocument;

        VisualElement m_LoginUiDocumentRoot;
        VisualElement m_LoginBarContainer;
        VisualElement m_UserInfo;
        Button m_LoginButton;
        Button m_LogoutButton;
        Label m_StatusLabel;

        void Start()
        {
            if (m_LoginUiDocument)
                m_LoginUiDocumentRoot = m_LoginUiDocument.rootVisualElement;

            m_LoginBarContainer = m_LoginUiDocumentRoot.Q<VisualElement>("LoginBarContainer");
            m_UserInfo = m_LoginUiDocumentRoot.Q<VisualElement>("UserInfo");
            m_LoginButton = m_LoginUiDocumentRoot.Q<Button>("LoginButton");
            m_LogoutButton = m_LoginUiDocumentRoot.Q<Button>("LogoutButton");
            m_StatusLabel = m_LoginUiDocumentRoot.Q<Label>("StatusLabel");

            RegisterButtons();

            m_Authenticator = PlatformServices.Authenticator;
            m_UserInfoProvider = PlatformServices.AuthenticatedUserInfoProvider;

            if (m_Authenticator.RequiresGUI)
            {
                PlatformServices.AuthenticationStateProvider.AuthenticationStateChanged += OnAuthenticationChanged;
            }
            else
            {
                m_LoginBarContainer.style.display = DisplayStyle.None;
            }
        }

        void OnDestroy()
        {
            if (PlatformServices.AuthenticationStateProvider != null)
            {
                PlatformServices.AuthenticationStateProvider.AuthenticationStateChanged -= OnAuthenticationChanged;
            }

            UnregisterButtons();
        }

        void Login()
        {
            try
            {
                m_Authenticator?.LoginAsync();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException
                    or AuthenticationFailedException)
                {
                    Debug.LogError(ex.Message);
                }
                throw;
            }
        }

        void Logout()
        {
            try
            {
                m_Authenticator?.LogoutAsync();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException
                    or AuthenticationFailedException)
                {
                    Debug.LogError(ex.Message);
                }
                throw;
            }
        }

        void OnAuthenticationChanged(AuthenticationState newAuthenticationState)
        {
            ApplyAuthenticationState(newAuthenticationState);
        }

        void ApplyAuthenticationState(AuthenticationState state)
        {
            switch (state)
            {
                case AuthenticationState.AwaitingInitialization:
                case AuthenticationState.AwaitingLogin:
                    UpdateStatus();
                    break;
                case AuthenticationState.AwaitingLogout:
                    UpdateLogout();
                    break;
                case AuthenticationState.LoggedIn:
                    m_LoginUiDocumentRoot.Q<Label>("UserName").text = GetUserName();
                    UpdateLogin();
                    break;
                case AuthenticationState.LoggedOut:
                    UpdateLogout();
                    break;
            }
        }

        void UpdateStatus()
        {
            m_LoginBarContainer.style.justifyContent = Justify.Center;
            m_LoginButton.style.display = DisplayStyle.None;
            m_StatusLabel.style.display = DisplayStyle.Flex;
            m_StatusLabel.text = k_AwaitingLoginText;
        }

        void UpdateLogin()
        {
            m_StatusLabel.style.display = DisplayStyle.None;
            m_LoginButton.style.display = DisplayStyle.None;
            m_LogoutButton.style.display = DisplayStyle.Flex;
            m_LoginBarContainer.style.justifyContent = Justify.SpaceBetween;
            m_UserInfo.style.display = DisplayStyle.Flex;
        }

        void UpdateLogout()
        {
            m_LogoutButton.style.display = DisplayStyle.None;
            m_UserInfo.style.display = DisplayStyle.None;
            m_LoginButton.style.display = DisplayStyle.Flex;
            m_LoginBarContainer.style.justifyContent = Justify.FlexEnd;
        }

        string GetUserName()
        {
            return m_UserInfoProvider.GetUserInfo(AuthenticatedUserInfoClaims.Name);
        }

        void RegisterButtons()
        {
            m_LoginButton.clickable.clickedWithEventInfo +=
                (evt => Login());
            m_LogoutButton.clickable.clickedWithEventInfo +=
                (evt => Logout());
        }

        void UnregisterButtons()
        {
            m_LoginButton.clickable.clickedWithEventInfo -=
                (evt => Login());
            m_LogoutButton.clickable.clickedWithEventInfo -=
                (evt => Logout());
        }
    }
}

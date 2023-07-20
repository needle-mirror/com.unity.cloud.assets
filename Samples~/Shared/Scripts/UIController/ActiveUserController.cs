#if !UC_EXCLUDE_SAMPLES
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Identity;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    public class ActiveUserController : MonoBehaviour
    {
        const string k_AwaitingLoginText = "Logging in...";

        ICompositeAuthenticator m_Authenticator;
        IUserInfoProvider m_UserInfoProvider;

        [SerializeField] UIDocument m_UiDocument;

        VisualElement m_UiDocumentRoot;
        VisualElement m_LoginBarContainer;
        VisualElement m_UserInfo;
        Button m_LoginButton;
        Button m_LogoutButton;
        Label m_StatusLabel;

        void Start()
        {
            if (m_UiDocument)
                m_UiDocumentRoot = m_UiDocument.rootVisualElement;

            m_LoginBarContainer = m_UiDocumentRoot.Q<VisualElement>("LoginBarContainer");
            m_UserInfo = m_UiDocumentRoot.Q<VisualElement>("UserInfo");
            m_LoginButton = m_UiDocumentRoot.Q<Button>("LoginButton");
            m_LogoutButton = m_UiDocumentRoot.Q<Button>("LogoutButton");
            m_StatusLabel = m_UiDocumentRoot.Q<Label>("StatusLabel");

            RegisterButtons();

            m_Authenticator = PlatformServices.Authenticator;
            m_UserInfoProvider = PlatformServices.UserInfoProvider;

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
            _ = ApplyAuthenticationState(newAuthenticationState);
        }

        async Task ApplyAuthenticationState(AuthenticationState state)
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
                    m_UiDocumentRoot.Q<Label>("UserName").text = await GetUserInfo();
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

        async Task<string> GetUserInfo()
        {
            try
            {
                var userInfo = await m_UserInfoProvider.GetUserInfoAsync();
                var userName = userInfo != null ? userInfo.Name : "No User";
                return userName;
            }

            catch (Exception ex)
            {
                if (ex is HttpRequestException
                    or UnauthorizedAccessException
                    or ConnectionException
                    or ForbiddenException)
                {
                    Debug.LogError(ex.Message);
                }

                throw;
            }
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
#endif

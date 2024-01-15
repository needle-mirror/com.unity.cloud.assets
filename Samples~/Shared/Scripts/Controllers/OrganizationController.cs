using System;
using Unity.Cloud.Common;
using Unity.Cloud.Identity;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class OrganizationController : MonoBehaviour
    {
        [SerializeField]
        UIDocument m_OrganizationListUiDocument;

        readonly OrganizationListUi m_OrganizationListUi = new();

        VisualElement m_RootVisualElement;
        IAssetRepository m_AssetRepository;
        ICompositeAuthenticator m_Authenticator;
        IOrganizationRepository m_OrganizationRepository;

        public VisualElement RootVisualElement => m_RootVisualElement ??= m_OrganizationListUiDocument.rootVisualElement;
        public IAssetRepository AssetRepository => m_AssetRepository;
        public OrganizationId SelectedOrganizationId => m_OrganizationListUi.SelectedOrganization.Id;

        public event Action ShowContent;
        public event Action HideContent;
        public event Action<OrganizationId> OrganizationSelected;

        protected virtual void Start()
        {
            ProcessAuthenticator();

            m_OrganizationListUi.Initialize(RootVisualElement);
            m_OrganizationListUi.OrganizationSelected += OnOrganizationSelected;
            m_OrganizationListUi.Hide();

            var popupContainer = RootVisualElement.Q("PopupContainer");
            DialogService.Initialize(popupContainer);
        }

        protected virtual void OnDestroy()
        {
            if (m_Authenticator != null)
            {
                m_Authenticator.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            }

            m_OrganizationListUi.OrganizationSelected -= OnOrganizationSelected;
        }

        public void SetServices(ICompositeAuthenticator authenticator, IAssetRepository assetRepository, IOrganizationRepository organizationRepository)
        {
            m_Authenticator = authenticator;
            m_AssetRepository = assetRepository;
            m_OrganizationRepository = organizationRepository;
        }

        void ProcessAuthenticator()
        {
            if (m_Authenticator.RequiresGUI)
            {
                m_Authenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
            }
            else
            {
                OnAuthenticationStateChanged(AuthenticationState.LoggedIn);
            }
        }

        async void OnAuthenticationStateChanged(AuthenticationState newAuthenticationState)
        {
            switch (newAuthenticationState)
            {
                case AuthenticationState.LoggedIn:
                    await m_OrganizationListUi.PopulateOrganizations(m_OrganizationRepository);
                    ShowContent?.Invoke();
                    break;
                case AuthenticationState.LoggedOut:
                    Hide();
                    break;
            }
        }

        protected virtual void Hide()
        {
            HideContent?.Invoke();

            m_OrganizationListUi.Hide();
        }

        void OnOrganizationSelected()
        {
            OrganizationSelected?.Invoke(m_OrganizationListUi.SelectedOrganization.Id);
        }
    }
}

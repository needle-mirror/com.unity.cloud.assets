using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Identity;
using UnityEngine;
using UnityEngine.UIElements;
using UserId = Unity.Cloud.Common.UserId;

namespace Unity.Cloud.Assets.Samples
{
    public class OrganizationController : MonoBehaviour
    {
        [SerializeField]
        UIDocument m_OrganizationListUiDocument;

        readonly OrganizationListUi m_OrganizationListUi = new();

        VisualElement m_RootVisualElement;
        static ICompositeAuthenticator Authenticator => PlatformServices.Authenticator;
        static IOrganizationRepository OrganizationRepository => PlatformServices.OrganizationRepository;

        public Dictionary<UserId, IMemberInfo> OrganizationMembersInfo { get; } = new();

        public VisualElement RootVisualElement => m_RootVisualElement ??= m_OrganizationListUiDocument.rootVisualElement;
        public IAssetRepository AssetRepository => PlatformServices.AssetRepository;
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

            DialogService.Initialize(RootVisualElement);
        }

        protected virtual void OnDestroy()
        {
            if (Authenticator != null)
            {
                Authenticator.AuthenticationStateChanged -= OnAuthenticationStateChanged;
            }

            m_OrganizationListUi.OrganizationSelected -= OnOrganizationSelected;
        }

        void ProcessAuthenticator()
        {
            if (Authenticator.RequiresGUI)
            {
                Authenticator.AuthenticationStateChanged += OnAuthenticationStateChanged;
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
                    await m_OrganizationListUi.PopulateOrganizations(OrganizationRepository);
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
            _ = GetOrganizationMembersInfo(m_OrganizationListUi.SelectedOrganization.Id);
        }

        async Task GetOrganizationMembersInfo(OrganizationId organizationId)
        {
            OrganizationMembersInfo.Clear();
            var datasetOrganization = await OrganizationRepository.GetOrganizationAsync(organizationId);
            var organizationMembersInfo = datasetOrganization.ListMembersAsync(Range.All);
            await foreach (var memberInfo in organizationMembersInfo)
            {
                OrganizationMembersInfo.Add(memberInfo.UserId, memberInfo);
            }
        }
    }
}

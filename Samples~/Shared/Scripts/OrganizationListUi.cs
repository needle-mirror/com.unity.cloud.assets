using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Cloud.Identity;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class OrganizationListUi
    {
        public event Action OrganizationSelected;

        VisualElement m_OrganizationsContainer;
        DropdownField m_OrganizationsDropdown;

        IOrganization m_SelectedOrganization;

        public IOrganization SelectedOrganization
        {
            get => m_SelectedOrganization;
            private set
            {
                m_SelectedOrganization = value;
                Debug.Log($"Organization Selected: {SelectedOrganization.Name}");
                OrganizationSelected?.Invoke();
            }
        }

        public void Initialize(VisualElement uiDocumentRoot)
        {
            m_OrganizationsContainer = uiDocumentRoot.Q<VisualElement>("OrganizationsContainer");
            m_OrganizationsDropdown = uiDocumentRoot.Q<DropdownField>("OrganizationsDropdown");
        }

        public void Hide()
        {
            m_OrganizationsContainer.style.display = DisplayStyle.None;
        }

        public async Task PopulateOrganizations(IOrganizationRepository organizationRepository)
        {
            var organizations = new List<IOrganization>();
            var organizationsAsyncEnumerable = organizationRepository.ListOrganizationsAsync(Range.All);
            await foreach (var organization in organizationsAsyncEnumerable)
            {
                organizations.Add(organization);
            }

            m_OrganizationsDropdown.choices = GetOrganizationsList(organizations);

            Show();

            m_OrganizationsDropdown.value = organizations.First().Name;
            SelectedOrganization = organizations.First();

            m_OrganizationsDropdown.RegisterValueChangedCallback(evt =>
            {
                SelectedOrganization = organizations.First(info => info.Name == evt.newValue);
            });
        }

        static List<string> GetOrganizationsList(IEnumerable<IOrganization> organization)
        {
            return organization.Select(info => info.Name).ToList();
        }

        void Show()
        {
            m_OrganizationsContainer.style.display = DisplayStyle.Flex;
        }
    }
}

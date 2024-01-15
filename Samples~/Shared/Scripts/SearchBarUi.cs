using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class SearchBarUi : MonoBehaviour
    {
        [SerializeField]
        VisualTreeAsset m_SearchBarTemplate;
        [SerializeField]
        VisualTreeAsset m_SearchBarChipTemplate;

        readonly SearchBarController m_SearchBarController = new();

        public FieldsFilter FieldsToInclude
        {
            get => m_SearchBarController.FieldsToInclude;
            set => m_SearchBarController.FieldsToInclude = value;
        }

        public event Action<IAsyncEnumerable<IAsset>> AddSearchQuery
        {
            add => m_SearchBarController.addSearchQuery += value;
            remove => m_SearchBarController.addSearchQuery -= value;
        }

        public event Action<IAsyncEnumerable<IAsset>> DeleteSearchQuery
        {
            add => m_SearchBarController.deleteSearchQuery += value;
            remove => m_SearchBarController.deleteSearchQuery -= value;
        }

        public event Action ClearSearchQuery
        {
            add => m_SearchBarController.clearSearchQuery += value;
            remove => m_SearchBarController.clearSearchQuery -= value;
        }

        public void Initialize(VisualElement root, VisualElement parentElement)
        {
            var searchBar = m_SearchBarTemplate.Instantiate();
            parentElement.Add(searchBar);

            m_SearchBarController.Init(root, m_SearchBarChipTemplate);
        }

        public void DisplaySearchBar(IAssetProject project)
        {
            m_SearchBarController.DisplaySearchBar();
            m_SearchBarController.UpdateSearchBarProjectsLabel(project);

            if (project != null)
            {
                UpdateSearchBarValues(project);
            }
        }

        public void DisplaySearchBar(IAssetRepository assetRepository, OrganizationId organizationId, IEnumerable<IAssetProject> projects)
        {
            var projectIds = projects.Select(p => p.Descriptor.ProjectId).ToArray();

            m_SearchBarController.DisplaySearchBar();
            m_SearchBarController.UpdateSearchBarProjectsLabel(assetRepository, organizationId, projectIds);

            UpdateSearchBarValues(assetRepository, organizationId, projectIds);
        }

        public void UpdateSearchBarValues(IAssetProject project)
        {
            _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Type, project);
            _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Name, project);
            _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Tags, project);
            _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Status, project);
        }

        public void UpdateSearchBarValues(IAssetRepository assetRepository, OrganizationId organizationId, IEnumerable<ProjectId> projectIds)
        {
            _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Type, assetRepository, organizationId, projectIds);
            _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Name, assetRepository, organizationId, projectIds);
            _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Tags, assetRepository, organizationId, projectIds);
            _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Status, assetRepository, organizationId, projectIds);
        }

        async Task UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion criterion, IAssetProject project)
        {
            try
            {
                var parameters = new AggregationParameters(GetCriterionSearchKey(criterion), 10000);
                var  aggregation = await project.CountAssetsAsync(new AssetSearchFilter(), parameters, CancellationToken.None);

                m_SearchBarController.UpdateSearchValues(criterion, aggregation.Values.ToArray());
            }
            catch (Exception e)
            {
                e.LogException();
            }
        }

        async Task UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion criterion, IAssetRepository assetRepository, OrganizationId organizationId, IEnumerable<ProjectId> projects)
        {
            try
            {
                var parameters = new AggregationParameters(GetCriterionSearchKey(criterion), 10000);
                var aggregation = await assetRepository.CountAssetsAsync(organizationId, projects, new AssetSearchFilter(), parameters, CancellationToken.None);

                m_SearchBarController.UpdateSearchValues(criterion, aggregation.Values.ToArray());
            }
            catch (Exception e)
            {
                e.LogException();
            }
        }

        static string GetCriterionSearchKey(SearchBarController.SearchCriterion criterion)
        {
            return criterion switch
            {
                SearchBarController.SearchCriterion.Type => AssetTypeSearchCriteria.SearchKey,
                SearchBarController.SearchCriterion.Name => "name",
                SearchBarController.SearchCriterion.Tags => "tags",
                SearchBarController.SearchCriterion.Status => "status",
                _ => throw new ArgumentOutOfRangeException(nameof(criterion), criterion, null)
            };
        }
    }
}

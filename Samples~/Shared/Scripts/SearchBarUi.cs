#if !UC_EXCLUDE_SAMPLES
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
            var searchBarContainer = m_SearchBarTemplate.Instantiate();
            searchBarContainer.name = "SearchBar";
            parentElement.Add(searchBarContainer);

            m_SearchBarController.Init(root, m_SearchBarChipTemplate);
        }

        public void DisplaySearchBar(IAssetProject project)
        {
            m_SearchBarController.DisplaySearchBar();
            m_SearchBarController.UpdateSearchBarProjectsLabel(project);

            if (project != null)
            {
                _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Type, project);
                _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Name, project);
                _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Tags, project);
            }
        }

        public void DisplaySearchBar(IAssetRepository assetRepository, OrganizationId organizationId, IEnumerable<IAssetProject> projects)
        {
            var projectIds = projects.Select(p => p.Descriptor.ProjectId).ToArray();

            m_SearchBarController.DisplaySearchBar();
            m_SearchBarController.UpdateSearchBarProjectsLabel(assetRepository, organizationId, projectIds);

            _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Type, assetRepository, organizationId, projectIds);
            _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Name, assetRepository, organizationId, projectIds);
            _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Tags, assetRepository, organizationId, projectIds);
        }

        async Task UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion criterion, IAssetProject project)
        {
            try
            {
                var parameters = new AggregationParameters(GetCriterionSearchKey(criterion));
                var  aggregation = await project.CountAssetsAsync(new AssetSearchFilter(), parameters, CancellationToken.None);

                m_SearchBarController.UpdateSearchValues(criterion, aggregation.Values.ToArray());
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);
                throw;
            }
            catch (AggregateException e)
            {
                Debug.LogException(e.InnerException);
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        async Task UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion criterion, IAssetRepository assetRepository, OrganizationId organizationId, IEnumerable<ProjectId> projects)
        {
            try
            {
                var parameters = new AggregationParameters(GetCriterionSearchKey(criterion));
                var aggregation = await assetRepository.CountAssetsAsync(organizationId, projects, new AssetSearchFilter(), parameters, CancellationToken.None);

                m_SearchBarController.UpdateSearchValues(criterion, aggregation.Values.ToArray());
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);
                throw;
            }
            catch (AggregateException e)
            {
                Debug.LogException(e.InnerException);
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        static string GetCriterionSearchKey(SearchBarController.SearchCriterion criterion)
        {
            return criterion switch
            {
                SearchBarController.SearchCriterion.Type => AssetTypeSearchCriteria.SearchKey,
                SearchBarController.SearchCriterion.Name => "name",
                SearchBarController.SearchCriterion.Tags => "tags",
                _ => throw new ArgumentOutOfRangeException(nameof(criterion), criterion, null)
            };
        }
    }
}
#endif

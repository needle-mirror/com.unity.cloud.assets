#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public void DisplaySearchBar(IProject project)
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

        public void DisplaySearchBar(IProject project, IOrganization organization, IEnumerable<IProject> projects)
        {
            m_SearchBarController.DisplaySearchBar();
            m_SearchBarController.UpdateSearchBarProjectsLabel(project, organization, projects);

            if (project != null)
            {
                _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Type, organization, project, projects);
                _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Name, organization, project, projects);
                _ = UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion.Tags, organization, project, projects);
            }
            else
            {
                m_SearchBarController.UpdateSearchValues(SearchBarController.SearchCriterion.Type, Array.Empty<string>());
                m_SearchBarController.UpdateSearchValues(SearchBarController.SearchCriterion.Name, Array.Empty<string>());
                m_SearchBarController.UpdateSearchValues(SearchBarController.SearchCriterion.Tags, Array.Empty<string>());
            }
        }

        async Task UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion criterion, IProject project)
        {
            try
            {
                var parameters = new AggregationParameters(criterion.ToString());
                var  aggregation = await PlatformServices.AssetProvider.AggregateAsync(new AssetSearchFilter(project), parameters, CancellationToken.None);

                m_SearchBarController.UpdateSearchValues(criterion, aggregation.Values.Keys.ToArray());
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

        async Task UpdateSearchBarValuesAsync(SearchBarController.SearchCriterion criterion, IOrganization organization, IProject project, IEnumerable<IProject> projects)
        {
            try
            {
                var parameters = new AggregationParameters(criterion.ToString());
                var aggregation = await PlatformServices.AssetProvider.AggregateAsync(organization, projects, new AssetSearchFilter(project), parameters, CancellationToken.None);

                m_SearchBarController.UpdateSearchValues(criterion, aggregation.Values.Keys.ToArray());
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
    }
}
#endif

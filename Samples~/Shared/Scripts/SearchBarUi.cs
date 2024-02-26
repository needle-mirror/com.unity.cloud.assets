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

        CancellationTokenSource m_AggregationCancellationToken;

        public event Action<IAsyncEnumerable<IAsset>, CancellationToken> AddSearchQuery
        {
            add => m_SearchBarController.AddSearchQuery += value;
            remove => m_SearchBarController.AddSearchQuery -= value;
        }

        public event Action<IAsyncEnumerable<IAsset>, CancellationToken> DeleteSearchQuery
        {
            add => m_SearchBarController.DeleteSearchQuery += value;
            remove => m_SearchBarController.DeleteSearchQuery -= value;
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

        public CancellationToken GetSearchCancellationToken() => m_SearchBarController.GetSearchCancellationToken();

        public void DisplaySearchBar(IAssetProject project)
        {
            m_SearchBarController.DisplaySearchBar();
            m_SearchBarController.UpdateSearchBarProjectsLabel(project);

            if (project != null)
            {
                UpdateSearchBarValues(project);
            }
        }

        public void DisplaySearchBar(IAssetRepository assetRepository, IEnumerable<IAssetProject> projects)
        {
            var projectDescriptors = projects.Select(p => p.Descriptor).ToArray();

            m_SearchBarController.DisplaySearchBar();
            m_SearchBarController.UpdateSearchBarProjectsLabel(assetRepository, projectDescriptors);

            UpdateSearchBarValues(assetRepository, projectDescriptors);
        }

        public void UpdateSearchBarValues(IAssetProject project)
        {
            var cancellationToken = GetAggregationCancellationToken();

            _ = UpdateSearchBarValuesAsync(GroupableField.Type, project, cancellationToken);
            _ = UpdateSearchBarValuesAsync(GroupableField.Name, project, cancellationToken);
            _ = UpdateSearchBarValuesAsync(GroupableField.Tags, project, cancellationToken);
            _ = UpdateSearchBarValuesAsync(GroupableField.Status, project, cancellationToken);
        }

        public void UpdateSearchBarValues(IAssetRepository assetRepository, IEnumerable<ProjectDescriptor> projectDescriptors)
        {
            var cancellationToken = GetAggregationCancellationToken();

            var enumerable = projectDescriptors.ToArray();
            _ = UpdateSearchBarValuesAsync(GroupableField.Type, assetRepository, enumerable, cancellationToken);
            _ = UpdateSearchBarValuesAsync(GroupableField.Name, assetRepository, enumerable, cancellationToken);
            _ = UpdateSearchBarValuesAsync(GroupableField.Tags, assetRepository, enumerable, cancellationToken);
            _ = UpdateSearchBarValuesAsync(GroupableField.Status, assetRepository, enumerable, cancellationToken);
        }

        async Task UpdateSearchBarValuesAsync(GroupableField criterion, IAssetProject project, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            try
            {
                var aggregation = await project.GroupAndCountAssets()
                    .LimitTo(102)
                    .ExecuteAsync(criterion, cancellationToken);

                m_SearchBarController.UpdateSearchValues(criterion, aggregation.ToArray());
            }
            catch (Exception e)
            {
                e.LogException();
            }
        }

        async Task UpdateSearchBarValuesAsync(GroupableField criterion, IAssetRepository assetRepository, IEnumerable<ProjectDescriptor> projectDescriptors, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            try
            {
                var aggregation = await assetRepository.GroupAndCountAssets(projectDescriptors)
                    .LimitTo(102)
                    .ExecuteAsync(criterion, cancellationToken);

                m_SearchBarController.UpdateSearchValues(criterion, aggregation.ToArray());
            }
            catch (Exception e)
            {
                e.LogException();
            }
        }

        CancellationToken GetAggregationCancellationToken()
        {
            m_AggregationCancellationToken?.Cancel();
            m_AggregationCancellationToken?.Dispose();
            m_AggregationCancellationToken = new CancellationTokenSource();
            return m_AggregationCancellationToken.Token;
        }
    }
}

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
            add => m_SearchBarController.ClearSearchQuery += value;
            remove => m_SearchBarController.ClearSearchQuery -= value;
        }

        public void Initialize(VisualElement root, VisualElement parentElement, IAssetRepository assetRepository)
        {
            var searchBar = m_SearchBarTemplate.Instantiate();
            parentElement.Add(searchBar);

            m_SearchBarController.Init(root, m_SearchBarChipTemplate, assetRepository);
        }

        public CancellationToken GetSearchCancellationToken() => m_SearchBarController.GetSearchCancellationToken();

        public void DisplaySearchBar(IAssetProject project)
        {
            m_SearchBarController.UpdateSearchBar(project);
            m_SearchBarController.DisplaySearchBar();
            UpdateSearchBarValues();
        }

        public void DisplaySearchBar(IEnumerable<IAssetProject> projects)
        {
            var projectDescriptors = projects.Select(p => p.Descriptor).ToArray();

            m_SearchBarController.UpdateSearchBar(projectDescriptors);
            m_SearchBarController.DisplaySearchBar();
            UpdateSearchBarValues();
        }

        public void UpdateSearchBarValues()
        {
            _ = m_SearchBarController.UpdateSearchBarValuesAsync();
        }
    }
}

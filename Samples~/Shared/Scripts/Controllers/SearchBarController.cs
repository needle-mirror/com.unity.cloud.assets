#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class SearchBarController
    {
        public enum SearchCriterion
        {
            Name,
            Tags,
            Type
        }

        const string k_SearchBarPlaceholder = "Search by name, type or tag...";
        static readonly Pagination m_DefaultPagination = new(nameof(IAsset.Name), Range.All);

        AssetSearchFilter m_AssetSearchFilter;

        VisualElement m_Root;
        VisualElement m_SearchBar;
        VisualElement m_SearchBarChipsContainer;
        VisualElement m_SearchBarContainer;

        TextField m_SearchBarField;
        Label m_SearchBarProjectLabel;
        VisualTreeAsset m_SearchBarChipTemplate;
        ListView m_SearchValuesContainer;

        Button m_SearchBarClearButton;

        List<string> m_QueryList;
        IOrganization m_Organization;
        IEnumerable<IProject> m_Projects;
        bool AcrossProjectMode => m_Organization != null && m_Projects != null;

        readonly Dictionary<SearchCriterion, string[]> m_SearchValuesByCategory = new();
        readonly List<string> m_SearchValues = new();

        public event Action<IAsyncEnumerable<IAsset>> addSearchQuery;
        public event Action<IAsyncEnumerable<IAsset>> deleteSearchQuery;
        public event Action clearSearchQuery;

        ListView SearchValuesContainer
        {
            get
            {
                if (m_SearchValuesContainer == null)
                {
                    m_SearchValuesContainer = m_Root.Q<ListView>("SearchValues");
                    SetupSearchValueList();
                }

                return m_SearchValuesContainer;
            }
        }

        public void Init(VisualElement root, VisualTreeAsset chipsTemplate)
        {
            m_AssetSearchFilter = new AssetSearchFilter(null);
            m_QueryList = new List<string>();

            m_Root = root;
            m_SearchBarChipTemplate = chipsTemplate;
            m_SearchBar = m_Root.Q<VisualElement>("SearchBar");
            m_SearchBarChipsContainer = m_Root.Q<VisualElement>("SearchBarChipsContainer");
            m_SearchBarField = m_Root.Q<TextField>("SearchBarField");
            m_SearchBarField.value = k_SearchBarPlaceholder;
            m_SearchBarProjectLabel = m_Root.Q<Label>("SearchBarProjectLabel");
            m_SearchBarContainer = m_Root.Q<VisualElement>("SearchBarContainer");
            m_SearchBarClearButton = m_Root.Q<Button>("SearchBarClearButton");
            var searchBarButton = m_Root.Q<Button>("SearchBarButton");

            searchBarButton.clickable.clicked += AddChipAsync;
            m_SearchBarClearButton.clickable.clicked += HideAndClearSearchBar;

            m_SearchBarField.RegisterCallback<FocusInEvent>(_ =>
            {
                OnSearchFieldIn();
            });
            m_SearchBarField.RegisterCallback<FocusOutEvent>(_ =>
            {
                OnSearchFieldOut();
            });
            m_SearchBarField.RegisterCallback<InputEvent>(evt =>
            {
                OnSearchFieldChange(evt.newData);
            });
        }

        async void AddChipAsync()
        {
            var searchString = m_SearchBarField.value;

            // Clear text field
            m_SearchBarField.value = k_SearchBarPlaceholder;

            if (string.IsNullOrWhiteSpace(searchString) || searchString == k_SearchBarPlaceholder || m_QueryList.Contains(searchString))
            {
                return;
            }

            var assetCount = await CountAssetsAsync(searchString);

            var chip = m_SearchBarChipTemplate.Instantiate();
            chip.Q<Label>("Label").viewDataKey = searchString;
            chip.Q<Label>("Label").text = $"{searchString} ({assetCount})";
            chip.Q<Button>("DeleteButton").clickable.clickedWithEventInfo += DeleteChip;

            m_SearchBarChipsContainer.Add(chip);
            m_QueryList.Add(searchString);

            m_SearchBarClearButton.style.display = DisplayStyle.Flex;

            addSearchQuery?.Invoke(UpdateAssetsListAsync());
        }

        void DeleteChip(EventBase obj)
        {
            var target = obj.currentTarget as Button;

            // SearchBarChipDeleteButton(Button) -> DeleteButton(VisualElement) -> SearchBarChip(VisualElement) -> SearchBarChipTemplate(uxml)
            var targetGrandparent = target?.hierarchy.parent.parent.parent;
            var targetName = targetGrandparent.Q<Label>("Label").viewDataKey;

            m_QueryList.Remove(targetName);
            m_SearchBarChipsContainer.Remove(targetGrandparent);

            if (m_QueryList.Count == 0)
            {
                HideAndClearSearchBar();
            }
            else
            {
                deleteSearchQuery?.Invoke(UpdateAssetsListAsync());
            }
        }

        public void UpdateSearchBarProjectsLabel(IProject project)
        {
            m_Organization = null;
            m_Projects = null;

            if (project != null)
            {
                m_AssetSearchFilter.Project.Include(project);
                m_SearchBarProjectLabel.text = $"In: {project.Name}";
            }

            HideAndClearSearchBar();
        }

        public void UpdateSearchBarProjectsLabel(IProject project, IOrganization organization, IEnumerable<IProject> projects)
        {
            m_Organization = organization;
            m_Projects = projects;

            if (project != null)
                m_SearchBarProjectLabel.text = $"In: {project.Name}";

            HideAndClearSearchBar();
        }

        public void UpdateSearchValues(SearchCriterion criterion, string[] names)
        {
            if (SearchValuesContainer != null) m_SearchValuesContainer.style.display = DisplayStyle.None;
            m_SearchValuesByCategory[criterion] = names;
        }

        public void DisplaySearchBar()
        {
            m_SearchBar.style.display = DisplayStyle.Flex;
        }

        void HideAndClearSearchBar()
        {
            m_SearchBarClearButton.style.display = DisplayStyle.None;
            ClearSearchBar();
        }

        void ClearSearchBar()
        {
            m_SearchBarChipsContainer?.Clear();
            m_QueryList?.Clear();
            clearSearchQuery?.Invoke();
        }

        IAsyncEnumerable<IAsset> UpdateAssetsListAsync()
        {
            var values = new HashSet<string>();
            foreach (var kvp in m_SearchValuesByCategory)
            {
                values.UnionWith(kvp.Value);
            }

            UpdateSearchCriterionString(SearchCriterion.Name, values, m_QueryList.ToArray());
            UpdateSearchCriterionString(SearchCriterion.Type, values, m_QueryList.ToArray());
            UpdateSearchCriterionList(SearchCriterion.Tags, values, m_QueryList.ToArray());

            try
            {
                if(AcrossProjectMode)
                    return PlatformServices.AssetProvider.SearchAsync(m_Organization, m_Projects, m_AssetSearchFilter, m_DefaultPagination, CancellationToken.None);

                return PlatformServices.AssetProvider.SearchAsync(m_AssetSearchFilter, m_DefaultPagination, CancellationToken.None);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        void UpdateSearchCriterionString(SearchCriterion criterion, ICollection<string> allValues, params string[] queries)
        {
            var filter = GetFilterAndValues(criterion, out var filterValues);
            if (filter == null) return;

            var stringBuilder = new StringBuilder();

            for (var i = 0; i < queries.Length; ++i)
            {
                var query = queries[i];

                if (filterValues.Contains(query) || allValues == null || !allValues.Contains(query))
                {
                    stringBuilder.Append(query);
                    stringBuilder.Append(' ');
                }
            }

            if (stringBuilder.Length > 0)
            {
                filter.ForAny(stringBuilder.ToString());
            }
        }

        void UpdateSearchCriterionList(SearchCriterion criterion, ICollection<string> allValues, params string[] queries)
        {
            var filter = GetFilterAndValues(criterion, out var filterValues);
            if (filter == null) return;

            List<string> anyList = new();

            for (var i = 0; i < queries.Length; ++i)
            {
                var query = queries[i];

                if (filterValues.Contains(query) || allValues == null || !allValues.Contains(query))
                {
                    anyList.Add(query);
                }
            }

            filter.ForAny(anyList.ToArray());
        }

        ISearchCriteria GetFilterAndValues(SearchCriterion criterion, out HashSet<string> filterValues)
        {
            var filter = m_AssetSearchFilter.AllCriteria.FirstOrDefault(x => x.SearchKey == criterion.ToString());
            if (filter == null)
            {
                filterValues = null;
                return null;
            }

            filter.Clear();
            filterValues = new HashSet<string>();
            if (m_SearchValuesByCategory.TryGetValue(criterion, out var values))
            {
                filterValues.UnionWith(values);
            }

            return filter;
        }

        async Task<int> CountAssetsAsync(string query)
        {
            UpdateSearchCriterionString(SearchCriterion.Name, null, query);
            UpdateSearchCriterionString(SearchCriterion.Type, null, query);
            UpdateSearchCriterionList(SearchCriterion.Tags, null, query);

            var parameters = new AggregationParameters(nameof(IAsset.Type));

            try
            {
                Aggregation aggregation;
                if (AcrossProjectMode)
                {
                    aggregation = await PlatformServices.AssetProvider.AggregateAsync(m_Organization, m_Projects, m_AssetSearchFilter, parameters, CancellationToken.None);
                }
                else
                {
                    aggregation = await PlatformServices.AssetProvider.AggregateAsync(m_AssetSearchFilter, parameters, CancellationToken.None);
                }

                return aggregation.Total;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        void OnSearchFieldIn()
        {
            m_SearchBarContainer.style.borderTopWidth = 1;
            m_SearchBarContainer.style.borderBottomWidth = 1;
            m_SearchBarContainer.style.borderLeftWidth = 1;
            m_SearchBarContainer.style.borderRightWidth = 1;
            m_SearchBarContainer.style.borderTopColor = new Color(0.16f, 0.63f, 1f, 1f);
            m_SearchBarContainer.style.borderBottomColor = new Color(0.16f, 0.63f, 1f, 1f);
            m_SearchBarContainer.style.borderLeftColor = new Color(0.16f, 0.63f, 1f, 1f);
            m_SearchBarContainer.style.borderRightColor = new Color(0.16f, 0.63f, 1f, 1f);

            OnSearchFieldChange("");

            if (SearchValuesContainer != null)
                m_SearchValuesContainer.style.display = DisplayStyle.Flex;
        }

        void OnSearchFieldOut()
        {
            m_SearchBarContainer.style.borderTopWidth = 0;
            m_SearchBarContainer.style.borderBottomWidth = 0;
            m_SearchBarContainer.style.borderLeftWidth = 0;
            m_SearchBarContainer.style.borderRightWidth = 0;
            m_SearchBarContainer.style.borderTopColor = new Color(0f, 0f, 0f, 0f);
            m_SearchBarContainer.style.borderBottomColor = new Color(0f, 0f, 0f, 0f);
            m_SearchBarContainer.style.borderLeftColor = new Color(0f, 0f, 0f, 0f);
            m_SearchBarContainer.style.borderRightColor = new Color(0f, 0f, 0f, 0f);

            if (SearchValuesContainer != null)
                m_SearchValuesContainer.style.display = DisplayStyle.None;
        }

        void OnSearchFieldChange(string searchString)
        {
            m_SearchValues.Clear();

            foreach (var kvp in m_SearchValuesByCategory)
            {
                var searchValues = new List<string>();

                foreach (var value in kvp.Value)
                {
                    if (!value.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) continue;

                    searchValues.Add($"{value}");
                }

                if (searchValues.Count > 0)
                {
                    m_SearchValues.Add($"<< {kvp.Key.ToString()} >>");
                    m_SearchValues.AddRange(searchValues);
                }
            }

            if (SearchValuesContainer != null)
                m_SearchValuesContainer.RefreshItems();
        }

        void SetupSearchValueList()
        {
            if (m_SearchValuesContainer == null) return;

            m_SearchValuesContainer.itemsSource = m_SearchValues;
            m_SearchValuesContainer.makeItem = () => new Label();
            m_SearchValuesContainer.bindItem = (element, i) =>
            {
                var label = element.Q<Label>();
                label.focusable = true;
                label.text = m_SearchValues[i];

                label.style.unityFontStyleAndWeight =
                    label.text.StartsWith("<<")
                        ? new StyleEnum<FontStyle>(FontStyle.Bold)
                        : new StyleEnum<FontStyle>(FontStyle.Normal);
            };

            m_SearchValuesContainer.onSelectionChange += enumerable =>
            {
                var selection = enumerable?.OfType<string>().ToList();
                if (selection == null || selection.Count == 0) return;

                var value = selection[0];

                m_SearchValuesContainer.ClearSelection();

                if (!string.IsNullOrWhiteSpace(value) && !value.StartsWith("<<"))
                {
                    m_SearchBarField.Blur();
                    m_SearchBarField.value = value;
                    AddChipAsync();
                }
            };
        }
    }
}
#endif

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
    public class SearchBarController
    {
        struct SearchValue
        {
            public string Name { get; }
            public SearchCriterion Type { get; }
            public int Count { get; }

            public SearchValue(string name, SearchCriterion type, int count)
            {
                Name = name;
                Type = type;
                Count = count;
            }
        }

        public enum SearchCriterion
        {
            None = -1,
            Name,
            Tags,
            Type,
            Status,
        }

        const string k_SearchBarPlaceholder = "Search by name, type or tag...";
        static readonly Pagination m_DefaultPagination = new(nameof(IAsset.Name), Range.All);

        AssetSearchFilter m_AssetSearchFilter;

        VisualElement m_Root;
        VisualElement m_SearchBar;
        VisualElement m_SearchBarChipsContainer;

        TextField m_SearchBarField;
        Label m_SearchBarProjectLabel;
        VisualTreeAsset m_SearchBarChipTemplate;
        ListView m_SearchValuesList;
        VisualElement m_SearchValuesCloseButton;

        Button m_SearchBarClearButton;

        List<string> m_QueryList;
        OrganizationId m_OrganizationId = OrganizationId.None;
        IEnumerable<ProjectId> m_Projects;
        IAssetProject m_CurrentProject;
        bool AcrossProjectMode => m_OrganizationId != OrganizationId.None && m_Projects != null;

        readonly Dictionary<SearchCriterion, KeyValuePair<string,int>[]> m_SearchValuesByCategory = new();
        readonly HashSet<string> m_AllSearchValues = new();
        readonly List<SearchValue> m_SearchValues = new();

        public FieldsFilter FieldsToInclude { get; set; }

        public event Action<IAsyncEnumerable<IAsset>> addSearchQuery;
        public event Action<IAsyncEnumerable<IAsset>> deleteSearchQuery;
        public event Action clearSearchQuery;

        IAssetRepository m_AssetRepository;

        ListView searchValuesList
        {
            get
            {
                if (m_SearchValuesList == null)
                {
                    m_SearchValuesList = m_Root.Q<ListView>("SearchValues");
                    SetupSearchValueList();
                }

                return m_SearchValuesList;
            }
        }

        public void Init(VisualElement root, VisualTreeAsset chipsTemplate)
        {
            m_AssetSearchFilter = new AssetSearchFilter
            {
                IncludedFields = FieldsToInclude
            };
            m_QueryList = new List<string>();

            m_Root = root;
            m_SearchBarChipTemplate = chipsTemplate;
            m_SearchBar = m_Root.Q<VisualElement>("SearchBar");
            m_SearchBarChipsContainer = m_Root.Q<VisualElement>("SearchBarChipsContainer");
            m_SearchBarField = m_Root.Q<TextField>("SearchBarField");
            m_SearchBarField.value = k_SearchBarPlaceholder;
            m_SearchBarProjectLabel = m_Root.Q<Label>("SearchBarProjectLabel");
            m_SearchBarClearButton = m_Root.Q<Button>("SearchBarClearButton");
            var searchBarButton = m_Root.Q<Button>("SearchBarButton");

            searchBarButton.clickable.clicked += AddChipAsync;
            m_SearchBarClearButton.clickable.clicked += HideAndClearSearchBar;
            m_SearchBarClearButton.style.display = DisplayStyle.None;

            m_SearchBarField.RegisterCallback<ClickEvent>(_ =>
            {
                ToggleSearchValuesContainer();
            });

            m_SearchBarField.RegisterCallback<FocusInEvent>(_ =>
            {
                EnableSearchBarHighlight(true);
                OnSearchFieldChange("");
            });
            m_SearchBarField.RegisterCallback<FocusOutEvent>(_ =>
            {
                EnableSearchBarHighlight(false);
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
            var overflowString = assetCount > 100 ? "+" : "";
            assetCount = Math.Min(assetCount, 100);

            var chip = m_SearchBarChipTemplate.Instantiate();
            chip.Q<Label>("Label").viewDataKey = searchString;
            chip.Q<Label>("Label").text = $"{searchString} ({assetCount}{overflowString})";
            chip.Q<Button>("DeleteButton").clickable.clickedWithEventInfo += DeleteChip;

            m_SearchBarChipsContainer.Add(chip);
            m_QueryList.Add(searchString);

            m_SearchBarClearButton.style.display = DisplayStyle.Flex;

            addSearchQuery?.Invoke(UpdateAssetsListAsync());

            HideSearchValuesContainer();
        }

        void DeleteChip(EventBase obj)
        {
            var target = obj.currentTarget as Button;

            // SearchBarChipDeleteButton(Button) -> SearchBarChip(VisualElement) -> SearchBarChipTemplate(uxml)
            var targetGrandparent = target?.hierarchy.parent.parent;
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

        public void UpdateSearchBarProjectsLabel(IAssetProject project)
        {
            m_OrganizationId = OrganizationId.None;
            m_Projects = null;
            m_CurrentProject = project;
            m_AssetRepository = null;

            if (project != null)
            {
                m_SearchBarProjectLabel.text = $"In: {project.Name}";
            }

            HideAndClearSearchBar();
        }

        public void UpdateSearchBarProjectsLabel(IAssetRepository assetRepository, OrganizationId organizationId, IEnumerable<ProjectId> projects)
        {
            m_OrganizationId = organizationId;
            m_Projects = projects;
            m_CurrentProject = null;
            m_AssetRepository = assetRepository;

            m_SearchBarProjectLabel.text = $"In: All Projects";

            HideAndClearSearchBar();
        }

        public void UpdateSearchValues(SearchCriterion criterion, KeyValuePair<string, int>[] names)
        {
            HideSearchValuesContainer();

            m_SearchValuesByCategory[criterion] = names;

            m_AllSearchValues.Clear();
            foreach (var kvp in m_SearchValuesByCategory)
            {
                foreach (var aggregation in kvp.Value)
                {
                    m_AllSearchValues.Add(aggregation.Key);
                }
            }
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
            UpdateSearchCriterionString(SearchCriterion.Name, m_AllSearchValues, m_QueryList.ToArray());
            UpdateSearchCriterionString(SearchCriterion.Type, m_AllSearchValues, m_QueryList.ToArray());
            UpdateSearchCriterionList(SearchCriterion.Tags, m_AllSearchValues, m_QueryList.ToArray());
            UpdateSearchCriterionString(SearchCriterion.Status, m_AllSearchValues, m_QueryList.ToArray());

            try
            {
                if (AcrossProjectMode)
                {
                    return m_AssetRepository.SearchAssetsAsync(m_OrganizationId, m_Projects, m_AssetSearchFilter, m_DefaultPagination, CancellationToken.None);
                }

                if (m_CurrentProject != null)
                {
                    return m_CurrentProject.SearchAssetsAsync(m_AssetSearchFilter, m_DefaultPagination, CancellationToken.None);
                }
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception e)
            {
                e.LogException();
                throw;
            }

            return Empty();

            static async IAsyncEnumerable<IAsset> Empty()
            {
                await Task.CompletedTask;
                yield break;
            }
        }

        static (SearchCriterion type, string queryValue) ParseQuery(string query)
        {
            var split = query.Split(':');

            if (split.Length != 2) return (SearchCriterion.None, query.Trim());

            return Enum.TryParse<SearchCriterion>(split[0].Trim(), out var type) ? (type, split[1].Trim()) : (SearchCriterion.None, string.Empty);
        }

        void UpdateSearchCriterionString(SearchCriterion criterion, ICollection<string> allValues, params string[] queries)
        {
            var filter = GetFilterAndValues(criterion, out var filterValues);
            if (filter == null) return;

            var forAnyList = new List<string>();
            var includeList = new List<string>();

            for (var i = 0; i < queries.Length; ++i)
            {
                var (type, queryValue) = ParseQuery(queries[i]);

                if (type == SearchCriterion.None)
                {
                    // If this criterion contains this value, consider it a match,
                    // otherwise, only consider the value if it's not a value of other criteria (or the global list is null)
                    if (filterValues.Contains(queryValue) || allValues == null || !allValues.Contains(queryValue))
                    {
                        forAnyList.Add(queryValue);
                    }
                }
                else if (type == criterion)
                {
                    if (filterValues.Contains(queryValue))
                    {
                        includeList.Add(queryValue);
                    }
                    else
                    {
                        forAnyList.Add(queryValue);
                    }
                }
            }

            if (forAnyList.Count > 0)
            {
                filter.ForAny(string.Join(' ', forAnyList));
            }

            if (includeList.Count > 0)
            {
                filter.Include(string.Join(' ', includeList));
            }
        }

        void UpdateSearchCriterionList(SearchCriterion criterion, ICollection<string> allValues, params string[] queries)
        {
            var filter = GetFilterAndValues(criterion, out var filterValues);
            if (filter == null) return;

            var forAnyList = new List<string>();
            var includeList = new List<string>();

            for (var i = 0; i < queries.Length; ++i)
            {
                var query = queries[i];

                var (type, queryValue) = ParseQuery(query);

                if (type == SearchCriterion.None)
                {
                    // If this criterion contains this value, consider it a match,
                    // otherwise, only consider the value if it's not a value of other criteria (or the global list is null)
                    if (filterValues.Contains(queryValue) || allValues == null || !allValues.Contains(queryValue))
                    {
                        forAnyList.Add(queryValue);
                    }
                }
                else if (type == criterion && filterValues.Contains(queryValue))
                {
                    if (filterValues.Contains(queryValue))
                    {
                        includeList.Add(queryValue);
                    }
                    else
                    {
                        forAnyList.Add(queryValue);
                    }
                }
            }

            filter.ForAny(forAnyList.ToArray());
            filter.Include(includeList.ToArray());
        }

        ISearchCriteria GetFilterAndValues(SearchCriterion criterion, out HashSet<string> filterValues)
        {
            var filter = m_AssetSearchFilter.AllCriteria.FirstOrDefault(x => x.PropertyName == criterion.ToString());
            if (filter == null)
            {
                filterValues = null;
                return null;
            }

            filter.Clear();
            filterValues = new HashSet<string>();
            if (m_SearchValuesByCategory.TryGetValue(criterion, out var values))
            {
                foreach (var aggregationResult in values)
                {
                    filterValues.Add(aggregationResult.Key);
                }
            }

            return filter;
        }

        async Task<int> CountAssetsAsync(string query)
        {
            UpdateSearchCriterionString(SearchCriterion.Name, m_AllSearchValues, query);
            UpdateSearchCriterionString(SearchCriterion.Type, m_AllSearchValues, query);
            UpdateSearchCriterionList(SearchCriterion.Tags, m_AllSearchValues, query);
            UpdateSearchCriterionString(SearchCriterion.Status, m_AllSearchValues, query);

            var parameters = new AggregationParameters(AssetTypeSearchCriteria.SearchKey, 101);

            try
            {
                Aggregation aggregation = default;
                if (AcrossProjectMode)
                {
                    aggregation = await m_AssetRepository.CountAssetsAsync(m_OrganizationId, m_Projects, m_AssetSearchFilter, parameters, CancellationToken.None);
                }
                else if (m_CurrentProject != null)
                {
                    aggregation = await m_CurrentProject.CountAssetsAsync(m_AssetSearchFilter, parameters, CancellationToken.None);
                }

                return aggregation?.Total ?? 0;
            }
            catch (OperationCanceledException)
            {
                return 0;
            }
            catch (Exception e)
            {
                e.LogException();
                throw;
            }
        }

        void ToggleSearchValuesContainer()
        {
            var isVisible = m_SearchValuesList.style.display == DisplayStyle.Flex;
            isVisible = !isVisible;
            m_SearchValuesList.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
            m_SearchValuesCloseButton.pickingMode = isVisible ? PickingMode.Position : PickingMode.Ignore;
        }

        void HideSearchValuesContainer()
        {
            if (m_SearchValuesList != null)
            {
                m_SearchValuesList.style.display = DisplayStyle.None;
            }

            if (m_SearchValuesCloseButton != null)
            {
                m_SearchValuesCloseButton.pickingMode = PickingMode.Ignore;
            }
        }

        void EnableSearchBarHighlight(bool isEnabled)
        {
            var width = isEnabled ? 1f : 0f;
            m_SearchBar.style.borderTopWidth = width;
            m_SearchBar.style.borderBottomWidth = width;
            m_SearchBar.style.borderLeftWidth = width;
            m_SearchBar.style.borderRightWidth = width;
        }

        void OnSearchFieldChange(string searchString)
        {
            m_SearchValues.Clear();

            foreach (var criterionAggregation in m_SearchValuesByCategory)
            {
                var searchValues = new List<SearchValue>();
                var count = 0;
                foreach (var criterionValue in criterionAggregation.Value)
                {
                    if (!criterionValue.Key.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) continue;

                    searchValues.Add(new SearchValue(criterionValue.Key, criterionAggregation.Key, criterionValue.Value));
                    count += criterionValue.Value;
                }

                if (searchValues.Count > 0)
                {
                    m_SearchValues.Add(new SearchValue($"=== {criterionAggregation.Key.ToString()} ({criterionAggregation.Value.Length}) ===", criterionAggregation.Key, count));
                    m_SearchValues.AddRange(searchValues);
                }
            }

            if (searchValuesList != null)
                m_SearchValuesList.RefreshItems();
        }

        void SetupSearchValueList()
        {
            if (m_SearchValuesList == null) return;

            m_SearchValuesList.itemsSource = m_SearchValues;
            m_SearchValuesList.makeItem = () => new Label();
            m_SearchValuesList.bindItem = (element, i) =>
            {
                var label = element.Q<Label>();
                label.text = $"{m_SearchValues[i].Name}";

                label.style.unityFontStyleAndWeight =
                    label.text.StartsWith("===")
                        ? new StyleEnum<FontStyle>(FontStyle.Bold)
                        : new StyleEnum<FontStyle>(FontStyle.Normal);
            };

#if UNITY_2022_3_OR_NEWER
            m_SearchValuesList.selectionChanged += OnSelectionChanged;
#else
            m_SearchValuesList.onSelectionChange += OnSelectionChanged;
#endif

            m_SearchValuesCloseButton = m_Root.Q("SearchValuesCloseButton");
            m_SearchValuesCloseButton.RegisterCallback<ClickEvent>(_ =>
            {
                HideSearchValuesContainer();
            });
        }

        void OnSelectionChanged(IEnumerable<object> enumerable)
        {
            var selection = enumerable?.OfType<SearchValue>().ToList();
            if (selection == null || selection.Count == 0) return;

            var searchValue = selection[0];

            m_SearchValuesList.ClearSelection();

            if (!string.IsNullOrWhiteSpace(searchValue.Name) && !searchValue.Name.StartsWith("==="))
            {
                m_SearchBarField.Blur();
                m_SearchBarField.value = $"{searchValue.Type}:{searchValue.Name}";
                AddChipAsync();
            }
        }
    }
}

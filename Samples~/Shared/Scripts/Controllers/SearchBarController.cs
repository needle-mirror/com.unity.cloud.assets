#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
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
        OrganizationId m_OrganizationId = OrganizationId.None;
        IEnumerable<ProjectId> m_Projects;
        IAssetProject m_CurrentProject;
        bool AcrossProjectMode => m_OrganizationId != OrganizationId.None && m_Projects != null;

        readonly Dictionary<SearchCriterion, KeyValuePair<string,int>[]> m_SearchValuesByCategory = new();
        readonly List<KeyValuePair<string,int>> m_SearchValues = new();

        public event Action<IAsyncEnumerable<IAsset>> addSearchQuery;
        public event Action<IAsyncEnumerable<IAsset>> deleteSearchQuery;
        public event Action clearSearchQuery;

        IAssetRepository m_AssetRepository;

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
            m_AssetSearchFilter = new AssetSearchFilter
            {
                IncludedFields = new FieldsFilter
                {
                    AssetFields = AssetFields.all,
                    FileFields = FileFields.downloadUrl
                }
            };
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

            m_SearchBarField.RegisterCallback<ClickEvent>(_ =>
            {
                ShowSearchValuesContainer();
            });

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

            // Hack
            HideSearchValuesContainer();
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

        public void UpdateSearchValues(SearchCriterion criterion, KeyValuePair<string,int>[] names)
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
                foreach (var aggregation in kvp.Value)
                {
                    values.Add(aggregation.Key);
                }
            }

            UpdateSearchCriterionString(SearchCriterion.Name, values, m_QueryList.ToArray());
            UpdateSearchCriterionString(SearchCriterion.Type, values, FilterTypes(m_QueryList.ToArray()));
            UpdateSearchCriterionList(SearchCriterion.Tags, values, m_QueryList.ToArray());

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
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }

            return Empty();

            static async IAsyncEnumerable<IAsset> Empty()
            {
                await Task.CompletedTask;
                yield break;
            }
        }

        static string[] FilterTypes(string[] typeValues)
        {
            var validTypes = new List<string>();
            foreach (var value in typeValues)
            {
                if ("Other".Equals(value) || value.GetAssetTypeFromString() != AssetType.Other)
                {
                    validTypes.Add(value);
                }
            }
            return validTypes.ToArray();
        }

        void UpdateSearchCriterionString(SearchCriterion criterion, ICollection<string> allValues, params string[] queries)
        {
            var filter = GetFilterAndValues(criterion, out var filterValues);
            if (filter == null) return;

            var stringBuilder = new StringBuilder();

            for (var i = 0; i < queries.Length; ++i)
            {
                var query = queries[i];
                if(criterion == SearchCriterion.Type && !"Other".Equals(query) && query.GetAssetTypeFromString() == AssetType.Other)
                    continue;

                if (filterValues.Contains(query) || allValues == null || !allValues.Contains(query))
                {
                    stringBuilder.Append(query);

                    if(queries.Length > 1)
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
            UpdateSearchCriterionString(SearchCriterion.Name, null, query);
            UpdateSearchCriterionString(SearchCriterion.Type, null, query);
            UpdateSearchCriterionList(SearchCriterion.Tags, null, query);

            var parameters = new AggregationParameters(AssetTypeSearchCriteria.SearchKey);

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
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        void ShowSearchValuesContainer()
        {
            m_SearchValuesContainer.style.display = DisplayStyle.Flex;
        }

        void HideSearchValuesContainer()
        {
            m_SearchValuesContainer.style.display = DisplayStyle.None;
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
        }

        void OnSearchFieldChange(string searchString)
        {
            m_SearchValues.Clear();

            foreach (var kvp in m_SearchValuesByCategory)
            {
                var searchValues = new List<KeyValuePair<string,int>>();
                int count = 0;
                foreach (var value in kvp.Value)
                {
                    if (!value.Key.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) continue;

                    searchValues.Add(value);
                    count += value.Value;
                }

                if (searchValues.Count > 0)
                {
                    m_SearchValues.Add(new KeyValuePair<string,int>($"<< {kvp.Key.ToString()} >>", count));
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
                label.text = $"{m_SearchValues[i].Key} ({m_SearchValues[i].Value.ToString()})";

                label.style.unityFontStyleAndWeight =
                    label.text.StartsWith("<<")
                        ? new StyleEnum<FontStyle>(FontStyle.Bold)
                        : new StyleEnum<FontStyle>(FontStyle.Normal);
            };

#if UNITY_2022_3_OR_NEWER
            m_SearchValuesContainer.selectionChanged += OnSelectionChanged;
#else
            m_SearchValuesContainer.onSelectionChange += OnSelectionChanged;
#endif
            m_SearchValuesContainer.RegisterCallback<FocusOutEvent>(_ =>
            {
                HideSearchValuesContainer();
            });
        }

        void OnSelectionChanged(IEnumerable<object> enumerable)
        {
            var selection = enumerable?.OfType<KeyValuePair<string,int>>().ToList();
            if (selection == null || selection.Count == 0) return;

            var value = selection[0];

            m_SearchValuesContainer.ClearSelection();

            if (!string.IsNullOrWhiteSpace(value.Key) && !value.Key.StartsWith("<<"))
            {
                m_SearchBarField.Blur();
                m_SearchBarField.value = value.Key;
                AddChipAsync();
            }
        }
    }
}
#endif

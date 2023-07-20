#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    class SearchBarController
    {
        public enum SearchCriterion
        {
            Name,
            Tags
        }

        const string k_SearchBarPlaceholder = "Search by name or tag...";
        static readonly Pagination m_DefaultPagination = AssetDiscoverySample.m_DefaultPagination;

        AssetSearchFilter m_AssetSearchFilter;

        VisualElement m_Root;
        VisualElement m_SearchBar;
        VisualElement m_SearchBarChipsContainer;
        TextField m_SearchBarField;
        Label m_SearchBarProjectLabel;
        VisualTreeAsset m_SearchBarChipTemplate;
        ListView m_SearchValuesContainer;

        List<string> m_QueryList;

        Dictionary<SearchCriterion, string[]> m_SearchValuesByCategory = new();
        List<string> m_SearchValues = new();

        public List<string> QueryList => m_QueryList;
        public event Action deleteQuery;

        public void Init(VisualElement root, VisualTreeAsset chipsTemplate, Action onAddedSearchQuery)
        {
            m_AssetSearchFilter = new AssetSearchFilter(null, null);
            m_QueryList = new List<string>();

            m_Root = root;
            m_SearchBarChipTemplate = chipsTemplate;
            m_SearchBar = m_Root.Q<VisualElement>("SearchBar");
            m_SearchBarChipsContainer = m_Root.Q<VisualElement>("SearchBarChipsContainer");
            m_SearchBarField = m_Root.Q<TextField>("SearchBarField");
            m_SearchBarField.value = k_SearchBarPlaceholder;
            m_SearchBarProjectLabel = m_Root.Q<Label>("SearchBarProjectLabel");
            m_SearchValuesContainer = m_Root.Q<ListView>("SearchValues");

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

            SetupSearchValueList(onAddedSearchQuery);
        }

        public async Task<List<IAsset>> AddChipAsync(List<IAsset> currentAssetList)
        {
            var searchString = m_SearchBarField.value;

            // Clear text field
            m_SearchBarField.value = k_SearchBarPlaceholder;

            if (string.IsNullOrWhiteSpace(searchString) || searchString == k_SearchBarPlaceholder || m_QueryList.Contains(searchString))
            {
                return currentAssetList;
            }

            var assetCount = await CountAssetsAsync(searchString);

                var chip = m_SearchBarChipTemplate.Instantiate();
                chip.Q<Label>("SearchBarChipLabel").viewDataKey = searchString;
                chip.Q<Label>("SearchBarChipLabel").text = $"{searchString} ({assetCount})";
                chip.Q<Button>("SearchBarChipDeleteButton").clickable.clickedWithEventInfo += DeleteChip;

                m_SearchBarChipsContainer.Add(chip);
                m_QueryList.Add(searchString);

                return await UpdateAssetsListAsync();
            }

        void DeleteChip(EventBase obj)
        {
            var target = obj.currentTarget as Button;
            // SearchBarChipDeleteButton(Button) -> DeleteButton(VisualElement) -> SearchBarChip(VisualElement) -> SearchBarChipTemplate(uxml)
            var targetGrandparent = target?.hierarchy.parent.parent.parent;
            var targetName = targetGrandparent.Q<Label>("SearchBarChipLabel").viewDataKey;

            m_QueryList.Remove(targetName);
            m_SearchBarChipsContainer.Remove(targetGrandparent);

            deleteQuery?.Invoke();
        }

        public async Task<List<IAsset>> UpdateAssetsListAsync()
        {
            var values = new HashSet<string>();
            foreach (var kvp in m_SearchValuesByCategory)
            {
                values.UnionWith(kvp.Value);
            }

            UpdateSearchCriterionName(values);
            UpdateSearchCriterionTags(values);

            try
            {
                var assetPage = await PlatformServices.AssetProvider.SearchAsync(m_AssetSearchFilter, m_DefaultPagination, CancellationToken.None);
                return assetPage.Elements.ToList();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        void UpdateSearchCriterionName(ICollection<string> allValues)
        {
            m_AssetSearchFilter.Name.Clear();
            var values = new HashSet<string>();
            if (m_SearchValuesByCategory.TryGetValue(SearchCriterion.Name, out var names))
            {
                values.UnionWith(names);
            }

            var stringBuilder = new StringBuilder();

            for (var i = m_QueryList.Count - 1; i >= 0; --i)
            {
                var query = m_QueryList[i];

                if (values.Contains(query))
                {
                    m_AssetSearchFilter.Name.Include(query);
                }
                else if (!allValues.Contains(query))
                {
                    stringBuilder.Append(query);
                    stringBuilder.Append(' ');
                }
            }

            if (stringBuilder.Length > 0)
            {
                m_AssetSearchFilter.Name.ForAny(stringBuilder.ToString());
            }
        }

        void UpdateSearchCriterionTags(ICollection<string> allValues)
        {
            m_AssetSearchFilter.Tags.Clear();
            var values = new HashSet<string>();
            if (m_SearchValuesByCategory.TryGetValue(SearchCriterion.Tags, out var tags))
            {
                values.UnionWith(tags);
            }

            List<string> includedTagList = new();
            List<string> anyTagList = new();
            for (var i = 0; i < m_QueryList.Count; ++i)
            {
                var query = m_QueryList[i];

                if (values.Contains(query))
                {
                    includedTagList.Add(query);
                }
                else if (!allValues.Contains(query))
                {
                    anyTagList.Add(query);
                }
            }

            m_AssetSearchFilter.Tags.Include(includedTagList.ToArray());
            m_AssetSearchFilter.Tags.ForAny(anyTagList.ToArray());
        }

        async Task<int> CountAssetsAsync(string query)
        {
            // By Name
            m_AssetSearchFilter.Name.Clear();
            m_AssetSearchFilter.Name.ForAny(query);

            m_AssetSearchFilter.Tags.Clear();
            m_AssetSearchFilter.Tags.ForAny(query);

            var parameters = new AggregationParameters(nameof(IAsset.Name));

            try
            {
                var aggregation = await PlatformServices.AssetProvider.AggregateAsync(m_AssetSearchFilter, parameters, CancellationToken.None);
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
            m_SearchBar.style.borderTopWidth = 1;
            m_SearchBar.style.borderBottomWidth = 1;
            m_SearchBar.style.borderLeftWidth = 1;
            m_SearchBar.style.borderRightWidth = 1;
            m_SearchBar.style.borderTopColor = new Color(0.16f, 0.63f, 1f, 1f);
            m_SearchBar.style.borderBottomColor = new Color(0.16f, 0.63f, 1f, 1f);
            m_SearchBar.style.borderLeftColor = new Color(0.16f, 0.63f, 1f, 1f);
            m_SearchBar.style.borderRightColor = new Color(0.16f, 0.63f, 1f, 1f);

            OnSearchFieldChange("");
            m_SearchValuesContainer.style.display = DisplayStyle.Flex;
        }

        void OnSearchFieldOut()
        {
            m_SearchBar.style.borderTopWidth = 0;
            m_SearchBar.style.borderBottomWidth = 0;
            m_SearchBar.style.borderLeftWidth = 0;
            m_SearchBar.style.borderRightWidth = 0;
            m_SearchBar.style.borderTopColor = new Color(0f, 0f, 0f, 0f);
            m_SearchBar.style.borderBottomColor = new Color(0f, 0f, 0f, 0f);
            m_SearchBar.style.borderLeftColor = new Color(0f, 0f, 0f, 0f);
            m_SearchBar.style.borderRightColor = new Color(0f, 0f, 0f, 0f);

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

            m_SearchValuesContainer.RefreshItems();
        }

        public void UpdateSearchBarProjectsLabel(IOrganization organization, IProject project)
        {
            m_AssetSearchFilter.Organization.Include(organization);
            m_AssetSearchFilter.Project.Include(project);

            m_SearchBarProjectLabel.text = $"In: {project.Name}";
        }

        public void UpdateSearchValues(SearchCriterion criterion, string[] names)
        {
            m_SearchValuesByCategory[criterion] = names;
        }

        public void ClearSearchBar()
        {
            m_SearchBarChipsContainer?.Clear();
            QueryList?.Clear();
        }

        void SetupSearchValueList(Action onAddedSearchQuery)
        {
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
                    onAddedSearchQuery();
                }
            };
        }
    }
}
#endif

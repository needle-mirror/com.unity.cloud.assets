using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class SearchBarController
    {
        enum SearchFilter
        {
            Name,
            Type,
            Status,
            Tags,
            SystemTags,
            PreviewFile,
            FileName,
            FileSize
        }

        struct SearchValue
        {
            public string Name { get; }
            public int Count { get; }

            public SearchValue(string name, int count)
            {
                Name = name;
                Count = count;
            }
        }

        static readonly Dictionary<SearchFilter, Groupable[]> k_SearchFilterToGroupableField = new()
        {
            {SearchFilter.Name, new Groupable[] {GroupableField.Name}},
            {SearchFilter.Type, new Groupable[] {GroupableField.Type}},
            {SearchFilter.Status, new Groupable[] {GroupableField.Status}},
            {SearchFilter.Tags, new Groupable[] {GroupableField.Tags, GroupableField.DatasetTags, GroupableField.FileTags}},
            {SearchFilter.SystemTags, new Groupable[] {GroupableField.SystemTags, GroupableField.DatasetSystemTags, GroupableField.FileSystemTags}},
            {SearchFilter.PreviewFile, new Groupable[] {GroupableField.PreviewFile}},
            {SearchFilter.FileName, new Groupable[] {GroupableField.FilePath}},
        };

        static readonly HashSet<SearchFilter> k_UniqueSearchFilters = new()
        {
            SearchFilter.Name,
            SearchFilter.Type,
            SearchFilter.Status,
            SearchFilter.PreviewFile,
            SearchFilter.FileName,
            SearchFilter.FileSize
        };

        const string k_SearchBarPlaceholder = "Search by ";

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
        IAssetRepository m_AssetRepository;
        OrganizationId? m_OrganizationId;
        IAssetProject m_CurrentProject;

        readonly Dictionary<SearchFilter, KeyValuePair<string, int>[]> m_SearchValuesByCategory = new();
        readonly List<SearchValue> m_SearchValues = new();

        SearchFilter m_CurrentSearchFilter = SearchFilter.Name;

        CancellationTokenSource m_SearchCancellationToken;
        CancellationTokenSource m_AggregationCancellationToken;

        public event Action<IAsyncEnumerable<IAsset>, CancellationToken> AddSearchQuery;
        public event Action<IAsyncEnumerable<IAsset>, CancellationToken> DeleteSearchQuery;
        public event Action ClearSearchQuery;

        ListView SearchValuesList
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

        public void Init(VisualElement root, VisualTreeAsset chipsTemplate, IAssetRepository assetRepository)
        {
            m_AssetRepository = assetRepository;

            m_QueryList = new List<string>();

            m_Root = root;
            m_SearchBarChipTemplate = chipsTemplate;
            m_SearchBar = m_Root.Q<VisualElement>("SearchBar");

            m_SearchBarChipsContainer = m_Root.Q<VisualElement>("SearchBarChipsContainer");

            var filter = m_Root.Q<EnumField>();
            filter.Init(m_CurrentSearchFilter);
            filter.RegisterValueChangedCallback(evt =>
            {
                m_CurrentSearchFilter = (SearchFilter) evt.newValue;
                SetDefaultSearchBarText();
                OnSearchFieldChange("");
            });

            m_SearchBarProjectLabel = m_Root.Q<Label>("SearchBarProjectLabel");

            m_SearchBarClearButton = m_Root.Q<Button>("SearchBarClearButton");
            m_SearchBarClearButton.clickable.clicked += ClearSearchBar;
            m_SearchBarClearButton.style.display = DisplayStyle.None;

            var searchBarButton = m_Root.Q<Button>("SearchBarButton");
            searchBarButton.clickable.clicked += AddChip;

            m_SearchBarField = m_Root.Q<TextField>("SearchBarField");
            SetDefaultSearchBarText();

            m_SearchBarField.RegisterCallback<ClickEvent>(ToggleSearchValuesContainer);

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

        void AddChip()
        {
            _ = AddChipAsync(m_SearchBarField.value);
        }

        async Task AddChipAsync(string searchString)
        {
            // Clear text field
            SetDefaultSearchBarText();

            if (string.IsNullOrWhiteSpace(searchString) || searchString.StartsWith(k_SearchBarPlaceholder)) return;

            // Remove existing chips if the current search filter is unique and a chip already exists for it
            if (k_UniqueSearchFilters.Contains(m_CurrentSearchFilter) && m_QueryList.Any(q => q.StartsWith($"{m_CurrentSearchFilter}:")))
            {
                var chipsToRemove = new List<VisualElement>();

                m_QueryList.RemoveAll(q => q.StartsWith($"{m_CurrentSearchFilter}:"));

                foreach (var child in m_SearchBarChipsContainer.Children())
                {
                    var button = child.Q<Button>();

                    var targetGrandparent = button?.hierarchy.parent.parent;
                    var targetName = targetGrandparent.Q<Label>("Label").viewDataKey;

                    if (targetName.StartsWith($"{m_CurrentSearchFilter}:"))
                    {
                        chipsToRemove.Add(targetGrandparent);
                    }
                }

                foreach (var chipToRemove in chipsToRemove)
                {
                    m_SearchBarChipsContainer.Remove(chipToRemove);
                }
            }

            searchString = $"{m_CurrentSearchFilter}:{searchString}";

            if (m_QueryList.Contains(searchString)) return;

            var cancellationToken = GetSearchCancellationToken();

            var assetCount = await CountAssetsAsync(searchString, cancellationToken);

            if (cancellationToken.IsCancellationRequested) return;

            var overflowString = assetCount > 100 ? "+" : "";
            assetCount = Math.Min(assetCount, 100);

            var chip = m_SearchBarChipTemplate.Instantiate();
            chip.Q<Label>("Label").viewDataKey = searchString;
            chip.Q<Label>("Label").text = $"{searchString} ({assetCount}{overflowString})";
            chip.Q<Button>("DeleteButton").clickable.clickedWithEventInfo += DeleteChip;

            m_SearchBarChipsContainer.Add(chip);
            m_QueryList.Add(searchString);

            m_SearchBarClearButton.style.display = DisplayStyle.Flex;

            AddSearchQuery?.Invoke(UpdateAssetsListAsync(cancellationToken), cancellationToken);

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
                ClearSearchBar();
            }
            else
            {
                var cancellationToken = GetSearchCancellationToken();
                DeleteSearchQuery?.Invoke(UpdateAssetsListAsync(cancellationToken), cancellationToken);
            }
        }

        public void UpdateSearchBar(IAssetProject project)
        {
            m_OrganizationId = null;
            m_CurrentProject = project;
            if (project == null)
            {
                m_SearchBarProjectLabel.text = "";
            }
            else
            {
                var propertiesAsync = project.GetPropertiesAsync(default);
                propertiesAsync.Wait();
                m_SearchBarProjectLabel.text = $"In: {propertiesAsync.Result.Name}";
            }

            ClearSearchBar();
        }

        public void UpdateSearchBar(OrganizationId organizationId)
        {
            m_OrganizationId = organizationId;
            m_CurrentProject = null;

            m_SearchBarProjectLabel.text = $"In: All Projects";

            ClearSearchBar();
        }

        public async Task UpdateSearchBarValuesAsync()
        {
            HideSearchValuesContainer();

            var cancellationToken = GetAggregationCancellationToken();

            m_SearchValuesByCategory.Clear();

            try
            {
                var tasks = new List<Task>();
                foreach (var searchFilter in Enum.GetValues(typeof(SearchFilter)))
                {
                    tasks.Add(GetValuesForFilter((SearchFilter) searchFilter, cancellationToken));
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                e.LogException();
            }
        }

        async Task GetValuesForFilter(SearchFilter searchFilter, CancellationToken cancellationToken)
        {
            if (!k_SearchFilterToGroupableField.TryGetValue(searchFilter, out var criteria)) return;

            IAsyncEnumerable<KeyValuePair<GroupableFieldValue, int>> aggregation = null;
            var searchValues = new List<KeyValuePair<string, int>>();

            foreach (var criterion in criteria)
            {
                if (criteria.Length > 1)
                {
                    var displayName = criterion.ToString();
                    if (displayName.Contains('.'))
                    {
                        displayName = displayName.Replace('.', ' ');
                    }
                    else
                    {
                        displayName = "assets " + displayName;
                    }
                    searchValues.Add(new KeyValuePair<string, int>($"=== {displayName} ===", 0));
                }

                if (m_OrganizationId.HasValue)
                {
                    aggregation = m_AssetRepository.GroupAndCountAssets(m_OrganizationId.Value)
                        .LimitTo(int.MaxValue)
                        .ExecuteAsync(criterion, cancellationToken);
                }
                else if (m_CurrentProject != null)
                {
                    aggregation = m_CurrentProject.GroupAndCountAssets()
                        .LimitTo(int.MaxValue)
                        .ExecuteAsync(criterion, cancellationToken);
                }

                if (aggregation == null) return;

                if (cancellationToken.IsCancellationRequested) return;

                await foreach (var aggregationValue in aggregation)
                {
                    searchValues.Add(new KeyValuePair<string, int>(aggregationValue.Key.AsString(), aggregationValue.Value));
                }
            }

            if (searchValues.Count > 0)
            {
                m_SearchValuesByCategory[searchFilter] = searchValues.ToArray();
            }
        }

        public void DisplaySearchBar()
        {
            m_SearchBar.style.display = DisplayStyle.Flex;
        }

        public CancellationToken GetSearchCancellationToken()
        {
            m_SearchCancellationToken?.Cancel();
            m_SearchCancellationToken?.Dispose();
            m_SearchCancellationToken = new CancellationTokenSource();

            return m_SearchCancellationToken.Token;
        }

        CancellationToken GetAggregationCancellationToken()
        {
            m_AggregationCancellationToken?.Cancel();
            m_AggregationCancellationToken?.Dispose();
            m_AggregationCancellationToken = new CancellationTokenSource();
            return m_AggregationCancellationToken.Token;
        }

        void ClearSearchBar()
        {
            m_SearchBarClearButton.style.display = DisplayStyle.None;
            m_SearchBarChipsContainer?.Clear();
            m_QueryList?.Clear();
            ClearSearchQuery?.Invoke();
        }

        IAsyncEnumerable<IAsset> UpdateAssetsListAsync(CancellationToken cancellationToken)
        {
            var assetSearchFilter = BuildSearchFilterFromQueries(m_QueryList.ToArray());

            try
            {
                if (m_OrganizationId.HasValue)
                {
                    return m_AssetRepository.QueryAssets(m_OrganizationId.Value).SelectWhereMatchesFilter(assetSearchFilter).ExecuteAsync(cancellationToken);
                }

                if (m_CurrentProject != null)
                {
                    return m_CurrentProject.QueryAssets().SelectWhereMatchesFilter(assetSearchFilter).ExecuteAsync(cancellationToken);
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

        static AssetSearchFilter BuildSearchFilterFromQueries(params string[] queries)
        {
            var assetSearchFilter = new AssetSearchFilter();

            List<string> tags = new();
            List<string> systemTags = new();

            assetSearchFilter.Any().Tags.WithValue(tags);
            assetSearchFilter.Any().SystemTags.WithValue(systemTags);
            assetSearchFilter.Any().Datasets.Tags.WithValue(tags);
            assetSearchFilter.Any().Datasets.SystemTags.WithValue(systemTags);
            assetSearchFilter.Any().Files.Tags.WithValue(tags);
            assetSearchFilter.Any().Files.SystemTags.WithValue(systemTags);

            var minimumMatch = 0;

            for (var i = 0; i < queries.Length; ++i)
            {
                var (type, queryValue) = ParseQuery(queries[i]);

                if (type == null) continue;

                if (queryValue.StartsWith("/"))
                {
                    PopulateSearchFilter(assetSearchFilter, type.Value, new Regex(queryValue[1..]));
                }
                else
                {
                    PopulateSearchFilter(assetSearchFilter, type.Value, queryValue, tags, systemTags, ref minimumMatch);
                }
            }

            assetSearchFilter.Any().WhereMinimumMatchEquals(minimumMatch);
            return assetSearchFilter;
        }

        static void PopulateSearchFilter(AssetSearchFilter assetSearchFilter, SearchFilter type, string queryValue, ICollection<string> tags, ICollection<string> systemTags, ref int mimimumMatch)
        {
            switch (type)
            {
                case SearchFilter.Name:
                    assetSearchFilter.Include().Name.WithValue(queryValue);
                    break;
                case SearchFilter.Type:
                    assetSearchFilter.Include().Type.WithValue(queryValue);
                    break;
                case SearchFilter.Status:
                    assetSearchFilter.Include().Status.WithValue(queryValue);
                    break;
                case SearchFilter.Tags:
                    tags.Add(queryValue);
                    ++mimimumMatch;
                    break;
                case SearchFilter.SystemTags:
                    systemTags.Add(queryValue);
                    ++mimimumMatch;
                    break;
                case SearchFilter.PreviewFile:
                    assetSearchFilter.Include().PreviewFile.WithValue(queryValue);
                    break;
                case SearchFilter.FileName:
                    assetSearchFilter.Include().Files.Path.WithValue(queryValue);
                    break;
                case SearchFilter.FileSize:
                    var split = queryValue.Split('&');
                    if (split.Length == 2)
                    {
                        if (TryParseNumericCondition(split[0], out var range1)
                            && TryParseNumericCondition(split[1], out var range2))
                        {
                            assetSearchFilter.Include().Files.Size.WithValue(range1.And(range2));
                        }
                    }
                    else if (TryParseNumericCondition(queryValue, out var range))
                    {
                        assetSearchFilter.Include().Files.Size.WithValue(range);
                    }
                    else if (long.TryParse(queryValue, out var fileSizeLong))
                    {
                        assetSearchFilter.Include().Files.Size.WithValue(fileSizeLong);
                    }

                    break;
            }
        }

        static bool TryParseNumericCondition(string str, out NumericRange numericRange)
        {
            str = str.Trim();
            if (str.StartsWith("<="))
            {
                numericRange = NumericRange.LessThanOrEqual(double.Parse(str[2..]));
                return true;
            }
            if (str.StartsWith("<"))
            {
                numericRange = NumericRange.LessThan(double.Parse(str[1..]));
                return true;
            }
            if (str.StartsWith(">="))
            {
                numericRange = NumericRange.GreaterThanOrEqual(double.Parse(str[2..]));
                return true;
            }
            if (str.StartsWith(">"))
            {
                numericRange = NumericRange.GreaterThan(double.Parse(str[1..]));
                return true;
            }

            numericRange = default;
            return false;
        }

        static void PopulateSearchFilter(AssetSearchFilter assetSearchFilter, SearchFilter type, Regex regex)
        {
            switch (type)
            {
                case SearchFilter.Name:
                    assetSearchFilter.Include().Name.WithValue(regex);
                    break;
                case SearchFilter.PreviewFile:
                    assetSearchFilter.Include().PreviewFile.WithValue(regex);
                    break;
                case SearchFilter.FileName:
                    assetSearchFilter.Include().Files.Path.WithValue(regex);
                    break;
            }
        }

        static (SearchFilter? type, string queryValue) ParseQuery(string query)
        {
            var split = query.Split(':');

            if (split.Length != 2) return (null, query.Trim());

            return Enum.TryParse<SearchFilter>(split[0].Trim(), out var type) ? (type, split[1].Trim()) : (null, string.Empty);
        }

        async Task<int> CountAssetsAsync(string query, CancellationToken cancellationToken)
        {
            var assetSearchFilter = BuildSearchFilterFromQueries(query);

            try
            {
                var count = 0;
                if (m_OrganizationId.HasValue)
                {
                    count = await m_AssetRepository.CountAssetsAsync(m_OrganizationId.Value, assetSearchFilter, cancellationToken);
                }
                else if (m_CurrentProject != null)
                {
                    count = await m_CurrentProject.CountAssetsAsync(assetSearchFilter, cancellationToken);
                }

                return count;
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

        void ToggleSearchValuesContainer(ClickEvent evt)
        {
            var isVisible = m_SearchValuesList.style.display == DisplayStyle.Flex;
            isVisible = !isVisible && m_SearchValuesByCategory.ContainsKey(m_CurrentSearchFilter);
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
            if (searchString.StartsWith("/"))
            {
                HideSearchValuesContainer();
                return;
            }

            // Ignore wildcard in search string to properly filter values
            searchString = searchString.Replace("*", "");

            m_SearchValues.Clear();

            if (m_SearchValuesByCategory.TryGetValue(m_CurrentSearchFilter, out var searchValues))
            {
                var filteredSearchValues = new List<SearchValue>();
                var count = 0;
                foreach (var criterionValue in searchValues)
                {
                    if (!criterionValue.Key.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) continue;

                    filteredSearchValues.Add(new SearchValue(criterionValue.Key, criterionValue.Value));
                    count += criterionValue.Value;
                }

                if (filteredSearchValues.Count > 0)
                {
                    m_SearchValues.Add(new SearchValue($"=== {count} Results ===", count));
                    m_SearchValues.AddRange(filteredSearchValues);
                }
            }

            if (SearchValuesList != null)
            {
                m_SearchValuesList.RefreshItems();
            }
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
                _ = AddChipAsync(searchValue.Name);
            }
        }

        void SetDefaultSearchBarText()
        {
            var searchHint = string.Empty;
            switch (m_CurrentSearchFilter)
            {
                case SearchFilter.Name:
                case SearchFilter.PreviewFile:
                case SearchFilter.FileName:
                    searchHint = " (begin query with '/' for regex)";
                    break;
                case SearchFilter.FileSize:
                    searchHint = "in bytes (e.g. '1000', '>1000', '<=1000&>500')";
                    break;
            }

            m_SearchBarField.SetValueWithoutNotify($"{k_SearchBarPlaceholder}{m_CurrentSearchFilter}{searchHint}");
        }
    }
}

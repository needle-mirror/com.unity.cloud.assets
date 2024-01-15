using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    public interface IAssetInformationPanelController
    {
        void Init(VisualElement root, VisualTreeAsset informationItemTemplate, VisualTreeAsset tagsTemplate, VisualTreeAsset dataSetItemTemplate, MonoBehaviour coroutineHandler);
        void PopulateAssetPanel(IAsset asset);
        Task PopulateDatasetsPanel(IAsyncEnumerable<IDataset> datasets);
        void DisplayInformationPanel();
        void HideInformationPanel();
    }

    class AssetInformationPanelController : IAssetInformationPanelController
    {
        static readonly Type k_AssetType = typeof(IAsset);
        static readonly HashSet<string> k_AssetPropertiesToHide = new()
        {
            nameof(IAsset.SourceProject),
            nameof(IAsset.LinkedProjects),
            nameof(IAsset.Name),
            nameof(IAsset.PreviewFileUrl),
            nameof(IAsset.Metadata),
            nameof(IAsset.SystemMetadata)
        };

        static readonly Type k_DatasetType = typeof(IDataset);
        static readonly HashSet<string> k_DatasetPropertiesToHide = new()
        {
            nameof(IDataset.Name),
            nameof(IDataset.FileOrder),
            nameof(IDataset.Metadata),
            nameof(IDataset.SystemMetadata)
        };

        const string k_NoneLabel = "None";
        const string k_NoCollectionsLabel = "No Collections Found";
        const string k_CurrentlySelectedTabClassName = "currentlySelectedTab";

        IAsset m_SelectedAsset;

        MonoBehaviour m_CoroutineHandler;

        VisualElement m_RootPanel;
        ScrollView m_AssetInformationPanelScrollView;
        VisualElement m_AssetInfoTab;
        VisualElement m_DatasetInfoTab;
        VisualElement m_AssetInformationContainer;
        VisualElement m_AssetInformationDownloadSuccessful;
        VisualElement m_DatasetInformationContainer;
        VisualTreeAsset m_InformationItemTemplate;
        VisualTreeAsset m_InformationTagsTemplate;
        VisualTreeAsset m_DatasetInformationItemTemplate;
        ScrollView m_DatasetScrollView;
        Button m_AssetDownloadButton;

        readonly HashSet<string> m_InProgressDownloads = new();

        CancellationTokenSource m_CancelPopulateAsset;

        public void Init(VisualElement root, VisualTreeAsset informationItemTemplate, VisualTreeAsset tagsTemplate, VisualTreeAsset dataSetItemTemplate, MonoBehaviour coroutineHandler)
        {
            m_InformationItemTemplate = informationItemTemplate;
            m_InformationTagsTemplate = tagsTemplate;
            m_DatasetInformationItemTemplate = dataSetItemTemplate;
            m_CoroutineHandler = coroutineHandler;

            root.style.minWidth = new StyleLength { value = new Length(40.0f, LengthUnit.Percent) };
            m_RootPanel = root.Q<VisualElement>("RightPanel");

            m_AssetInformationPanelScrollView = root.Q<ScrollView>("AssetInformationScrollView");

            m_AssetInfoTab = root.Q<Label>("AssetInfo");
            m_AssetInfoTab.RegisterCallback<ClickEvent>(OnAssetInfoTabClicked);
            m_AssetInformationContainer = root.Q<VisualElement>("AssetInformationContainer");
            m_AssetInformationDownloadSuccessful = root.Q<VisualElement>("AssetDownloadSuccessful");
            m_AssetDownloadButton = root.Q<Button>("AssetDownloadButton");
            m_AssetDownloadButton.clickable.clicked += OnAssetDownloadButtonClicked;

            m_DatasetInfoTab = root.Q<Label>("DatasetsInfo");
            m_DatasetInfoTab.RegisterCallback<ClickEvent>(OnDatasetInfoTabClicked);
            m_DatasetInformationContainer = root.Q<VisualElement>("DatasetInformationContainer");
            m_DatasetScrollView = root.Q<ScrollView>("DatasetsScrollView");
        }

        void OnAssetInfoTabClicked(ClickEvent evt)
        {
            m_AssetInfoTab.AddToClassList(k_CurrentlySelectedTabClassName);
            m_DatasetInfoTab.RemoveFromClassList(k_CurrentlySelectedTabClassName);
            m_DatasetInformationContainer.style.display = DisplayStyle.None;
            m_AssetInformationContainer.style.display = DisplayStyle.Flex;
        }

        void OnDatasetInfoTabClicked(ClickEvent evt)
        {
            m_DatasetInfoTab.AddToClassList(k_CurrentlySelectedTabClassName);
            m_AssetInfoTab.RemoveFromClassList(k_CurrentlySelectedTabClassName);
            m_DatasetInformationContainer.style.display = DisplayStyle.Flex;
            m_AssetInformationContainer.style.display = DisplayStyle.None;
        }

        public void PopulateAssetPanel(IAsset asset)
        {
            if (m_CancelPopulateAsset != null)
            {
                m_CancelPopulateAsset.Cancel();
                m_CancelPopulateAsset.Dispose();
            }

            m_CancelPopulateAsset = new CancellationTokenSource();

            m_AssetInformationContainer.Q<Label>("AssetInformationLabel").text = asset.Name;

            m_AssetInformationPanelScrollView.Clear();

            m_SelectedAsset = asset;

            var propertyNames = m_SelectedAsset.GetType().GetProperties().Select(property => property.Name)
                .Where(name => !k_AssetPropertiesToHide.Contains(name));

            foreach (var propertyName in propertyNames)
            {
                var propertyValue = k_AssetType.GetProperty(propertyName)?.GetValue(m_SelectedAsset);

                if (string.IsNullOrEmpty(propertyValue?.ToString())) continue;

                var propertyInformation = CreatePropertyInformation
                (
                    propertyName,
                    propertyValue
                );

                foreach (var property in propertyInformation)
                {
                    m_AssetInformationPanelScrollView.Add(property);
                }
            }

            _ = PopulateMetadata(asset.Metadata, asset.SystemMetadata, m_CancelPopulateAsset.Token);

            m_AssetDownloadButton.tooltip = "";

            UpdateDownloadButton(m_AssetDownloadButton, !m_InProgressDownloads.Contains(m_SelectedAsset.Descriptor.AssetId.ToString()));
        }

        public async Task PopulateDatasetsPanel(IAsyncEnumerable<IDataset> datasets)
        {
            m_DatasetScrollView.Clear();

            var dataSetsList = new List<IDataset>();
            await foreach (var dataset in datasets)
                dataSetsList.Add(dataset);

            if (dataSetsList.Count == 0)
                return;

            var dataSetPropertyNames = typeof(IDataset).GetProperties().Select(property => property.Name)
                .Where(name => !k_DatasetPropertiesToHide.Contains(name));

            foreach (var dataset in dataSetsList)
            {
                var item = m_DatasetInformationItemTemplate.Instantiate();
                item.Q<Foldout>("DataSetFoldout").text = dataset.Name;

                var dataSetInformationScrollView = item.Q<ScrollView>("DataSetInformationScrollView");
                foreach (var propertyName in dataSetPropertyNames)
                {
                    var propertyValue = k_DatasetType.GetProperty(propertyName)?.GetValue(dataset);

                    if (string.IsNullOrEmpty(propertyValue?.ToString())) continue;

                    var propertyInformation = CreatePropertyInformation
                    (
                        propertyName,
                        propertyValue
                    );

                    foreach (var property in propertyInformation)
                    {
                        dataSetInformationScrollView.Add(property);
                    }
                }

                _ = PopulateMetadata(dataset.Metadata, dataset.SystemMetadata, m_CancelPopulateAsset.Token);

                var datasetDownloadButton = item.Q<Button>("DatasetDownloadButton");
                datasetDownloadButton.clickable.clicked += () =>
                {
                    OnDatasetDownloadButtonClicked(dataset, datasetDownloadButton);
                };

                m_DatasetScrollView.Add(item);
            }
        }

        IEnumerable<TemplateContainer> CreatePropertyInformation(string propertyName, object propertyValue)
        {
            var items = new List<TemplateContainer>();

            switch (propertyValue)
            {
                case AssetDescriptor assetDescriptor:
                    AddItem(items,"Id", assetDescriptor.AssetId.ToString());
                    break;
                case DatasetDescriptor datasetDescriptor:
                    AddItem(items,"Id", datasetDescriptor.DatasetId.ToString());
                    break;
                case AuthoringInfo authoringInfo:
                    AddItem(items,"Created On", authoringInfo.Created.ToString("MM/dd/yyyy"));
                    AddItem(items,"Updated On", authoringInfo.Updated.ToString("MM/dd/yyyy"));
                    break;
                case int propertyValueInt:
                    AddItem(items, propertyName, propertyValueInt.ToString());
                    break;
                case IEnumerable<CollectionPath>:
                    var collectionEntry = AddItem(items, propertyName, string.Empty);
                    _ = ListAssetCollections(collectionEntry);
                    break;
                case IEnumerable<string> enumerable:
                    AddItemList(items, propertyName, enumerable);
                    break;
                default:
                    AddItem(items, propertyName, propertyValue.ToString());
                    break;
            }

            return items;
        }

        async Task PopulateMetadata(IMetadataContainer metadataContainer, IMetadataContainer systemMetadataContainer, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var metadata = await metadataContainer.Query().ExecuteAsync(cancellationToken);

            PopulateMetadata(metadata, "Metadata");

            if (cancellationToken.IsCancellationRequested) return;

            var systemMetadata = await systemMetadataContainer.Query().ExecuteAsync(cancellationToken);

            PopulateMetadata(systemMetadata, "System Metadata");
        }

        void PopulateMetadata(IReadOnlyDictionary<string, IMetadataValue> metadata, string sectionTitle)
        {
            var properties = new List<TemplateContainer>();

            foreach (var kvp in metadata)
            {
                switch (kvp.Value.ValueType)
                {
                    case MetadataValueType.MultiSelection:
                        var multiSelectionEntry = kvp.Value.AsMultiSelection();
                        AddItemList(properties, kvp.Key, multiSelectionEntry.SelectedValues);
                        break;
                    case MetadataValueType.Url:
                        var urlEntry = kvp.Value.AsUrl();
                        var label = string.IsNullOrEmpty(urlEntry.Label) ? urlEntry.Uri.ToString() : urlEntry.Label;
                        var urlString = $"<a href=\"{urlEntry.Uri}\">{label}</a>";
                        AddItem(properties, kvp.Key, urlString);
                        break;
                    default:
                        AddItem(properties, kvp.Key, kvp.Value.ToString());
                        break;
                }
            }

            if (properties.Any())
            {
                m_AssetInformationPanelScrollView.Add(CreateLabel(sectionTitle));
            }

            foreach (var property in properties)
            {
                m_AssetInformationPanelScrollView.Add(property);
            }
        }

        TemplateContainer CreateLabel(string key)
        {
            var item = m_InformationItemTemplate.Instantiate();
            var label = item.Q<Label>("InformationPropertyLabel");
            label.parent.style.flexGrow = 1;
            label.text = key;
            item.Q("InformationValueLabel").style.display = DisplayStyle.None;
            return item;
        }

        TemplateContainer AddItem(ICollection<TemplateContainer> items, string key, string value)
        {
            var item = m_InformationItemTemplate.Instantiate();
            items.Add(item);
            item.Q<Label>("InformationPropertyLabel").text = key;
            item.Q<Label>("InformationValueLabel").text = value;
            return item;
        }

        void AddItemList(ICollection<TemplateContainer> items, string key, IEnumerable<string> values)
        {
            var item = AddItem(items, key, string.Empty);

            var label = item.Q<Label>("InformationValueLabel");
            var valueList = values.ToList();
            if (valueList.Count != 0)
            {
                label.style.display = DisplayStyle.None;
                foreach (var value in valueList)
                {
                    var tagItem = m_InformationTagsTemplate.Instantiate();
                    tagItem.Q<Button>("TagContainer").text = value;
                    item.Q<VisualElement>("InformationValue").Add(tagItem);
                }
            }
            else
            {
                label.text = k_NoneLabel;
            }
        }

        public void DisplayInformationPanel()
        {
            if (m_RootPanel == null)
                return;

            m_RootPanel.style.display = DisplayStyle.Flex;
            OnAssetInfoTabClicked(null);
        }

        public void HideInformationPanel()
        {
            if(m_RootPanel != null) m_RootPanel.style.display = DisplayStyle.None;
        }

        async void OnAssetDownloadButtonClicked()
        {
            UpdateDownloadButton(m_AssetDownloadButton, false);

            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var assetToDownload = m_SelectedAsset;
            m_InProgressDownloads.Add(assetToDownload.Descriptor.AssetId.ToString());

            try
            {
                await assetToDownload.GetAssetDownloadUrlsAsync(CancellationToken.None);

                await foreach (var file in assetToDownload.ListFilesAsync(Range.All, CancellationToken.None))
                {
                    await using var destination = File.OpenWrite(Path.Combine(path, file.Descriptor.Path));

                    // Evaluate the need of having a UI progress bar corresponding to the download progress.
                    await file.DownloadAsync(destination, null, CancellationToken.None);
                }

                m_CoroutineHandler.StartCoroutine(ShowSuccessfulDownload());
            }
            catch (Exception e)
            {
                e.LogException();
            }
            finally
            {
                m_InProgressDownloads.Remove(assetToDownload.Descriptor.AssetId.ToString());
                if (m_SelectedAsset == assetToDownload)
                {
                    UpdateDownloadButton(m_AssetDownloadButton, true);
                }
            }
        }

        async void OnDatasetDownloadButtonClicked(IDataset dataset, Button button)
        {
            UpdateDownloadButton(button, false);

            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            m_InProgressDownloads.Add(dataset.Descriptor.DatasetId.ToString());

            try
            {
                await foreach (var file in dataset.ListFilesAsync(Range.All, CancellationToken.None))
                {
                    await using var destination = File.OpenWrite(Path.Combine(path, file.Descriptor.Path));

                    // Evaluate the need of having a UI progress bar corresponding to the download progress.
                    await file.DownloadAsync(destination, null, CancellationToken.None);
                }

                m_CoroutineHandler.StartCoroutine(ShowSuccessfulDownload());
            }
            catch (Exception e)
            {
                e.LogException();
            }
            finally
            {
                m_InProgressDownloads.Remove(dataset.Descriptor.DatasetId.ToString());
                UpdateDownloadButton(button, true);
            }
        }

        static void UpdateDownloadButton(Button button, bool enable)
        {
            button.SetEnabled(enable);
            button.text = enable ? "Download" : "Downloading...";
        }

        IEnumerator ShowSuccessfulDownload()
        {
            m_AssetInformationDownloadSuccessful.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(3f);
            m_AssetInformationDownloadSuccessful.style.display = DisplayStyle.None;
        }

        async Task ListAssetCollections(TemplateContainer item)
        {
            var label = item.Q<Label>("InformationValueLabel");
            label.style.display = DisplayStyle.None;
            var container = item.Q<VisualElement>("InformationValue");

            try
            {
                await m_SelectedAsset.RefreshAssetCollectionsAsync(CancellationToken.None);
            }
            catch (Exception e)
            {
                e.LogException();
            }
            finally
            {
                var collections = m_SelectedAsset.Collections.ToArray();
                if (collections.Length != 0)
                {
                    foreach (var collection in collections)
                    {
                        var collectionItem = m_InformationTagsTemplate.Instantiate();
                        collectionItem.Q<Button>("TagContainer").text = collection.GetLastComponentOfPath();
                        container.Add(collectionItem);
                    }
                }
                else
                {
                    label.text = k_NoCollectionsLabel;
                    label.style.display = DisplayStyle.Flex;
                }
            }
        }
    }
}

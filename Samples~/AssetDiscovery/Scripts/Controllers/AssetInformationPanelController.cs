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
            nameof(IAsset.Metadata),
            nameof(IAsset.SystemMetadata),
        };

        static readonly Type k_DatasetType = typeof(IDataset);
        static readonly HashSet<string> k_DatasetPropertiesToHide = new()
        {
            nameof(IDataset.Name),
            nameof(IDataset.FileOrder),
            nameof(IDataset.Metadata),
        };

        const string k_NoneLabel = "None";
        const string k_NoCollectionsLabel = "No Collections Found";
        const string k_CurrentlySelectedTabClassName = "currentlySelectedTab";

        readonly OrganizationController m_OrganizationController;
        IAsset m_SelectedAsset;
        OrganizationId m_OrganizationId;
        Dictionary<string, string> m_FieldToName;
        Dictionary<string, string> m_StatusFlowToName;

        MonoBehaviour m_CoroutineHandler;

        VisualElement m_RootPanel;
        ScrollView m_AssetInformationScrollView;
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

        string m_DownloadFolder;
        readonly HashSet<string> m_InProgressDownloads = new();

        CancellationTokenSource m_CancelPopulateAsset;

        public AssetInformationPanelController(OrganizationController organizationController)
        {
            m_OrganizationController = organizationController;
        }

        public void Init(VisualElement root, VisualTreeAsset informationItemTemplate, VisualTreeAsset tagsTemplate, VisualTreeAsset dataSetItemTemplate, MonoBehaviour coroutineHandler)
        {
            m_InformationItemTemplate = informationItemTemplate;
            m_InformationTagsTemplate = tagsTemplate;
            m_DatasetInformationItemTemplate = dataSetItemTemplate;
            m_CoroutineHandler = coroutineHandler;

            root.style.minWidth = new StyleLength {value = new Length(40.0f, LengthUnit.Percent)};
            m_RootPanel = root.Q<VisualElement>("RightPanel");

            m_AssetInformationScrollView = root.Q<ScrollView>("AssetInformationScrollView");

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

            m_AssetInformationContainer.Q<Label>("Name").text = asset.Name;
            m_AssetInformationContainer.Q<Label>("Version").text = asset.IsFrozen ? $"Ver. {asset.FrozenSequenceNumber}" : "Pending";

            m_AssetInformationScrollView.Clear();

            m_SelectedAsset = asset;

            PopulateOrganizationFields(asset.Descriptor.OrganizationId);

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
                    m_AssetInformationScrollView.Add(property);
                }
            }

            _ = PopulateMetadata(asset.SystemMetadata, m_AssetInformationScrollView, "SystemMetadata", m_CancelPopulateAsset.Token);
            _ = PopulateMetadata(asset.Metadata as IReadOnlyMetadataContainer, m_AssetInformationScrollView, "Metadata", m_CancelPopulateAsset.Token);

            m_AssetDownloadButton.tooltip = "";

            UpdateDownloadButton(m_AssetDownloadButton, !m_InProgressDownloads.Contains(m_SelectedAsset.Descriptor.AssetId.ToString()));
        }

        public async Task PopulateDatasetsPanel(IAsyncEnumerable<IDataset> datasets)
        {
            m_DatasetScrollView.Clear();

            var dataSetPropertyNames = typeof(IDataset).GetProperties()
                .Select(property => property.Name)
                .Where(name => !k_DatasetPropertiesToHide.Contains(name))
                .ToList();

            await foreach (var dataset in datasets)
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

                _ = PopulateMetadata(dataset.SystemMetadata, dataSetInformationScrollView, "System Metadata", m_CancelPopulateAsset.Token);
                _ = PopulateMetadata(dataset.Metadata as IReadOnlyMetadataContainer, dataSetInformationScrollView, "Metadata", m_CancelPopulateAsset.Token);

                var datasetDownloadButton = item.Q<Button>("DatasetDownloadButton");
                datasetDownloadButton.clickable.clicked += () =>
                {
                    OnDatasetDownloadButtonClicked(dataset, datasetDownloadButton);
                };

                m_DatasetScrollView.Add(item);
            }
        }

        void PopulateOrganizationFields(OrganizationId organizationId)
        {
            if (m_OrganizationId == organizationId) return;
            m_OrganizationId = organizationId;

            _ = PopulateFieldToName();
            _ = PopluateStatusFlowToName();
        }

        async Task PopulateFieldToName()
        {
            m_FieldToName = null;

            var dictionary = new Dictionary<string, string>();
            await foreach (var fieldDefinition in PlatformServices.AssetRepository.ListFieldDefinitionsAsync(m_OrganizationId, Range.All, default))
            {
                dictionary[fieldDefinition.Descriptor.FieldKey] = fieldDefinition.DisplayName;
            }

            m_FieldToName = dictionary;
        }

        async Task PopluateStatusFlowToName()
        {
            m_StatusFlowToName = null;

            var dictionary = new Dictionary<string, string>();
            await foreach (var statusFlow in PlatformServices.AssetRepository.ListStatusFlowsAsync(m_OrganizationId, Range.All, default))
            {
                dictionary[statusFlow.Descriptor.StatusFlowId] = statusFlow.Name;
            }

            m_StatusFlowToName = dictionary;
        }

        IEnumerable<TemplateContainer> CreatePropertyInformation(string propertyName, object propertyValue)
        {
            var items = new List<TemplateContainer>();

            switch (propertyValue)
            {
                case AssetDescriptor assetDescriptor:
                    AddItem(items, "Id", assetDescriptor.AssetId.ToString());
                    break;
                case DatasetDescriptor datasetDescriptor:
                    AddItem(items, "Id", datasetDescriptor.DatasetId.ToString());
                    break;
                case StatusFlowDescriptor statusFlowDescriptor:
                    AddItem(items, "Status Flow", statusFlowDescriptor.StatusFlowId, valueSetter: (l, s) => _ = TrySetStatusFlowNameAsync(l, s));
                    break;
                case AuthoringInfo authoringInfo:
                    AddItem(items, "Created On", authoringInfo.Created.ToString("MM/dd/yyyy"));
                    if (m_OrganizationController.OrganizationMembersInfo.TryGetValue(authoringInfo.CreatedBy, out var value)) AddItem(items, "Created By", value.Name);
                    AddItem(items, "Updated On", authoringInfo.Updated.ToString("MM/dd/yyyy"));
                    if (m_OrganizationController.OrganizationMembersInfo.TryGetValue(authoringInfo.UpdatedBy, out value)) AddItem(items, "Updated By", value.Name);
                    break;
                case int propertyValueInt:
                    AddItem(items, propertyName, propertyValueInt.ToString());
                    break;
                case IEnumerable<CollectionPath>:
                    var collectionEntry = AddItem(items, propertyName, string.Empty);
                    _ = ListAssetCollections(collectionEntry);
                    break;
                case IEnumerable<LabelDescriptor> labelDescriptors:
                    AddItemList(items, propertyName, labelDescriptors.Select(label => label.LabelName));
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

        async Task PopulateMetadata(IReadOnlyMetadataContainer metadataContainer, VisualElement scrollView, string sectionTitle, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var result = metadataContainer.Query().ExecuteAsync(cancellationToken);
            await PopulateMetadata(result, sectionTitle, scrollView);
        }

        async Task PopulateMetadata(IAsyncEnumerable<KeyValuePair<string, MetadataValue>> metadata, string sectionTitle, VisualElement scrollView)
        {
            var properties = new List<TemplateContainer>();

            Action<Label, string> getFieldName = (l, s) => _ = TrySetFieldNameAsync(l, s);

            await foreach (var kvp in metadata)
            {
                switch (kvp.Value.ValueType)
                {
                    case MetadataValueType.MultiSelection:
                        var multiSelectionEntry = kvp.Value.AsMultiSelection();
                        AddItemList(properties, kvp.Key, multiSelectionEntry.SelectedValues, getFieldName);
                        break;
                    case MetadataValueType.Url:
                        var urlEntry = kvp.Value.AsUrl();
                        var label = string.IsNullOrEmpty(urlEntry.Label) ? urlEntry.Uri.ToString() : urlEntry.Label;
                        var urlString = $"<a href=\"{urlEntry.Uri}\">{label}</a>";
                        AddItem(properties, kvp.Key, urlString, getFieldName);
                        break;
                    default:
                        AddItem(properties, kvp.Key, kvp.Value.ToString(), getFieldName);
                        break;
                }
            }

            if (properties.Any())
            {
                scrollView.Add(CreateLabel(sectionTitle));
            }

            foreach (var property in properties)
            {
                scrollView.Add(property);
            }
        }

        TemplateContainer CreateLabel(string key)
        {
            var item = m_InformationItemTemplate.Instantiate();
            item.Q("InformationPropertyLabel").style.display = DisplayStyle.None;

            var label = item.Q<Label>("InformationValueLabel");
            label.parent.style.justifyContent = Justify.FlexStart;
            label.style.fontSize = 18;
            label.text = key;
            return item;
        }

        TemplateContainer AddItem(ICollection<TemplateContainer> items, string key, string value, Action<Label, string> labelSetter = null, Action<Label, string> valueSetter = null)
        {
            var item = m_InformationItemTemplate.Instantiate();
            items.Add(item);

            var label = item.Q<Label>("InformationPropertyLabel");
            label.text = key;
            labelSetter?.Invoke(label, key);

            var valueLabel = item.Q<Label>("InformationValueLabel");
            valueLabel.text = value;
            valueSetter?.Invoke(valueLabel, value);

            return item;
        }

        void AddItemList(ICollection<TemplateContainer> items, string key, IEnumerable<string> values, Action<Label, string> labelSetter = null)
        {
            var item = AddItem(items, key, string.Empty, labelSetter);

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

        async Task TrySetFieldNameAsync(TextElement label, string key)
        {
            while (m_FieldToName == null)
            {
                await Task.Yield();
            }

            if (m_FieldToName.TryGetValue(key, out var displayName))
            {
                label.text = displayName;
            }
        }

        async Task TrySetStatusFlowNameAsync(TextElement label, string key)
        {
            while (m_StatusFlowToName == null)
            {
                await Task.Yield();
            }

            if (m_StatusFlowToName.TryGetValue(key, out var displayName))
            {
                label.text = displayName;
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
            if (m_RootPanel != null) m_RootPanel.style.display = DisplayStyle.None;
        }

        void OnAssetDownloadButtonClicked()
        {
            UpdateDownloadButton(m_AssetDownloadButton, false);

            GetDownloadFolder(() => _ = DownloadAsset());
        }

        async Task DownloadAsset()
        {
            if (string.IsNullOrEmpty(m_DownloadFolder))
            {
                UpdateDownloadButton(m_AssetDownloadButton, true);
                return;
            }

            var assetToDownload = m_SelectedAsset;
            m_InProgressDownloads.Add(assetToDownload.Descriptor.AssetId.ToString());

            try
            {
                var sourceDataset = await assetToDownload.GetSourceDatasetAsync(CancellationToken.None);
                await foreach (var file in sourceDataset.ListFilesAsync(Range.All, CancellationToken.None))
                {
                    await using var destination = OpenWrite(Path.Combine(m_DownloadFolder, file.Descriptor.Path));

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

        void OnDatasetDownloadButtonClicked(IDataset dataset, Button button)
        {
            UpdateDownloadButton(button, false);

            GetDownloadFolder(() => _ = DownloadDataset(dataset, button));
        }

        async Task DownloadDataset(IDataset dataset, Button button)
        {
            if (string.IsNullOrEmpty(m_DownloadFolder))
            {
                UpdateDownloadButton(button, true);
                return;
            }

            m_InProgressDownloads.Add(dataset.Descriptor.DatasetId.ToString());

            try
            {
                await foreach (var file in dataset.ListFilesAsync(Range.All, CancellationToken.None))
                {
                    await using var destination = OpenWrite(Path.Combine(m_DownloadFolder, file.Descriptor.Path));

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

        async Task ListAssetCollections(VisualElement item)
        {
            var label = item.Q<Label>("InformationValueLabel");
            label.style.display = DisplayStyle.None;
            var container = item.Q<VisualElement>("InformationValue");

            var collections = new List<CollectionPath>();

            try
            {
                var collectionsAsync = m_SelectedAsset.ListLinkedAssetCollectionsAsync(Range.All, CancellationToken.None);
                await foreach (var collection in collectionsAsync)
                {
                    collections.Add(collection.Path);
                }
            }
            catch (Exception e)
            {
                e.LogException();
            }
            finally
            {
                if (collections.Count > 0)
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

        void GetDownloadFolder(Action download = null)
        {
            const string dialogHeader = "Download file to location:";

            if (string.IsNullOrEmpty(m_DownloadFolder))
            {
                m_DownloadFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

#if UNITY_EDITOR
            m_DownloadFolder = UnityEditor.EditorUtility.OpenFolderPanel(dialogHeader, m_DownloadFolder, "");
            download?.Invoke();
#else
            DialogService.ShowMessage("Download", dialogHeader, OnDownloadFolderSelected, m_DownloadFolder);
            return;

            void OnDownloadFolderSelected(string folder)
            {
                m_DownloadFolder = folder;
                download?.Invoke();
            }
#endif
        }

        static FileStream OpenWrite(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return File.OpenWrite(filePath);
        }
    }
}

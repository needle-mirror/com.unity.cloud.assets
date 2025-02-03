using System;
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
    public class AssetInformationPanelController
    {
        public delegate Task<string> GetDisplayName(string key);

        static readonly Type k_AssetPropertiesType = typeof(AssetProperties);
        static readonly HashSet<string> k_AssetPropertiesToHide = new()
        {
            nameof(AssetProperties.SourceProject),
            nameof(AssetProperties.LinkedProjects),
            nameof(AssetProperties.Name),
            nameof(AssetProperties.PreviewFileDescriptor),
        };

        const string k_NoneLabel = "None";
        const string k_NoCollectionsLabel = "No Collections Found";
        const string k_CurrentlySelectedTabClassName = "currentlySelectedTab";

        readonly OrganizationController m_OrganizationController;
        IAsset m_SelectedAsset;

        VisualElement m_RootPanel;
        ScrollView m_AssetInformationScrollView;
        VisualElement m_AssetInfoTab;
        VisualElement m_DatasetInfoTab;
        VisualElement m_AssetInformationContainer;
        VisualElement m_AssetInformationDownloadSuccessful;
        VisualElement m_DatasetInformationContainer;
        ScrollView m_DatasetScrollView;
        Button m_AssetDownloadButton;

        string m_DownloadFolder;
        readonly HashSet<string> m_InProgressDownloads = new();

        CancellationTokenSource m_CancelPopulateAsset;

        GetDisplayName m_GetFieldName;
        GetDisplayName m_GetStatusFlowName;

        public AssetInformationPanelController(OrganizationController organizationController)
        {
            m_OrganizationController = organizationController;
        }

        public void Init(VisualElement root, GetDisplayName getFieldName, GetDisplayName getStatusFlowName)
        {
            m_GetFieldName = getFieldName;
            m_GetStatusFlowName = getStatusFlowName;

            root.style.minWidth = new StyleLength {value = new Length(40.0f, LengthUnit.Percent)};
            m_RootPanel = root.Q<VisualElement>("RightPanel");

            m_AssetInformationScrollView = root.Q<ScrollView>("AssetInformationScrollView");

            m_AssetInfoTab = root.Q<Label>("AssetInfo");
            m_AssetInfoTab.RegisterCallback<ClickEvent>(OnAssetInfoTabClicked);
            m_AssetInformationContainer = root.Q<VisualElement>("AssetInformationContainer");
            m_AssetInformationDownloadSuccessful = root.Q<VisualElement>("AssetDownloadSuccessful");
            m_AssetInformationDownloadSuccessful?.Hide();
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

            m_AssetInformationScrollView.Clear();

            m_SelectedAsset = asset;

            foreach (var property in CreatePropertyInformation(string.Empty, asset.Descriptor))
            {
                m_AssetInformationScrollView.Add(property);
            }

            _ = PopulateAssetProperties(asset, m_CancelPopulateAsset.Token);
            _ = PopulateMetadata(asset.SystemMetadata, m_AssetInformationScrollView, "SystemMetadata", m_CancelPopulateAsset.Token);
            _ = PopulateMetadata(asset.Metadata as IReadOnlyMetadataContainer, m_AssetInformationScrollView, "Metadata", m_CancelPopulateAsset.Token);

            m_AssetDownloadButton.tooltip = "";

            if (m_InProgressDownloads.Contains(m_SelectedAsset.Descriptor.AssetId.ToString()))
            {
                UpdateDownloadButton(m_AssetDownloadButton, false);
            }
            else
            {
                m_AssetDownloadButton.text = "Download";
                m_AssetDownloadButton.SetEnabled(false);
            }
        }

        async Task PopulateAssetProperties(IAsset asset, CancellationToken cancellationToken)
        {
            var assetProperties = await asset.GetPropertiesAsync(cancellationToken);

            m_AssetInformationContainer.Q<Label>("Name").text = assetProperties.Name;
            m_AssetInformationContainer.Q<Label>("Version").text = assetProperties.State == AssetState.Frozen ? $"Ver. {assetProperties.FrozenSequenceNumber}" : "Pending";

            foreach (var propertyInfo in k_AssetPropertiesType.GetProperties())
            {
                if (k_AssetPropertiesToHide.Contains(propertyInfo.Name)) continue;

                var propertyInformation = CreatePropertyInformation
                (
                    propertyInfo.Name,
                    propertyInfo.GetValue(assetProperties)
                );

                foreach (var property in propertyInformation)
                {
                    m_AssetInformationScrollView.Add(property);
                }
            }
        }

        public async Task PopulateDatasetsPanel(IAsyncEnumerable<IDataset> datasets)
        {
            m_DatasetScrollView.Clear();

            var tasks = new List<Task<bool>>();

            await foreach (var dataset in datasets)
            {
                var foldout = new DatasetFoldout(dataset, CreatePropertyInformation);

                foldout.RegisterDownloadButtonCallback(() => OnDatasetDownloadButtonClicked(dataset, foldout));

                _ = PopulateMetadata(dataset.SystemMetadata, foldout.ScrollView, "System Metadata", m_CancelPopulateAsset.Token);
                _ = PopulateMetadata(dataset.Metadata as IReadOnlyMetadataContainer, foldout.ScrollView, "Metadata", m_CancelPopulateAsset.Token);

                m_DatasetScrollView.Add(foldout);

                tasks.Add(foldout.CheckIfHasFilesTask);
            }

            await Task.WhenAll(tasks);

            if (tasks.Any(task => task.IsCompletedSuccessfully && task.Result))
            {
                UpdateDownloadButton(m_AssetDownloadButton, !m_InProgressDownloads.Contains(m_SelectedAsset.Descriptor.AssetId.ToString()));
            }
        }

        IEnumerable<VisualElement> CreatePropertyInformation(string propertyName, object propertyValue)
        {
            var items = new List<VisualElement>();

            switch (propertyValue)
            {
                case AssetDescriptor assetDescriptor:
                    AddItem(items, "Id", assetDescriptor.AssetId.ToString());
                    AddItem(items, "Version", assetDescriptor.AssetVersion.ToString());
                    break;
                case DatasetDescriptor datasetDescriptor:
                    AddItem(items, "Id", datasetDescriptor.DatasetId.ToString());
                    break;
                case StatusFlowDescriptor statusFlowDescriptor:
                    AddItem(items, "Status Flow", statusFlowDescriptor.StatusFlowId, valueSetter: (l, s) => _ = SetDisplayNameAsync(m_GetStatusFlowName, l, s));
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
            var properties = new List<VisualElement>();

            Action<Label, string> getFieldName = (l, s) => _ = SetDisplayNameAsync(m_GetFieldName, l, s);

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

        static VisualElement CreateLabel(string key)
        {
            var item = new VisualElement();
            item.AddToClassList("details-container");

            var label = new Label
            {
                text = key,
                style = { fontSize = 18}
            };
            label.AddToClassList("details-label");
            item.Add(label);
            return item;
        }

        static VisualElement AddItem(ICollection<VisualElement> items, string key, string value, Action<Label, string> labelSetter = null, Action<Label, string> valueSetter = null)
        {
            var item = new VisualElement();
            item.AddToClassList("details-container");
            items.Add(item);

            var label = new Label {text = key};
            label.AddToClassList("details-label");
            item.Add(label);
            labelSetter?.Invoke(label, key);

            if (!string.IsNullOrEmpty(value) || valueSetter != null)
            {
                var valueLabel = new Label {text = value};
                valueLabel.AddToClassList("details-value");
                item.Add(valueLabel);
                valueSetter?.Invoke(valueLabel, value);
            }

            return item;
        }

        static void AddItemList(ICollection<VisualElement> items, string key, IEnumerable<string> values, Action<Label, string> labelSetter = null)
        {
            var item = AddItem(items, key, string.Empty, labelSetter);

            var valueList = values.ToList();
            if (valueList.Count != 0)
            {
                var container = new VisualElement();
                container.AddToClassList("details-value-container");
                item.Add(container);

                foreach (var value in valueList)
                {
                    var tag = new Button {text = value};
                    tag.AddToClassList("AssetInfoItem");
                    container.Add(tag);
                }
            }
            else
            {
                var valueLabel = new Label {text = k_NoneLabel};
                valueLabel.AddToClassList("details-value");
            }
        }

        static async Task SetDisplayNameAsync(GetDisplayName getDisplayName, TextElement label, string key)
        {
            var displayName = await getDisplayName(key);
            label.text = displayName;
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

                _ = ShowSuccessfulDownload();
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

        void OnDatasetDownloadButtonClicked(IDataset dataset, DatasetFoldout foldout)
        {
            UpdateDownloadButton(foldout.Q<Button>(), false);

            GetDownloadFolder(() => _ = DownloadDataset(dataset, foldout));
        }

        async Task DownloadDataset(IDataset dataset, DatasetFoldout foldout)
        {
            var button = foldout.Q<Button>();

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

                foldout.ShowDownloadSuccessLabel();
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

        async Task ShowSuccessfulDownload()
        {
            m_AssetInformationDownloadSuccessful.style.display = DisplayStyle.Flex;
            await Task.Delay(3000);
            m_AssetInformationDownloadSuccessful.style.display = DisplayStyle.None;
        }

        async Task ListAssetCollections(VisualElement item)
        {
            var label = item.Q<Label>();
            label.style.display = DisplayStyle.None;

            var container = new VisualElement();
            container.AddToClassList("details-value-container");
            item.Add(container);

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
                        var collectionItem = new Button {text = collection.GetLastComponentOfPath()};
                        collectionItem.AddToClassList("AssetInfoItem");
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

#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    class AssetInformationPanelController
    {
        static readonly HashSet<string> m_PropertiesToHide = new()
        {
            nameof(IAsset.Project),
            nameof(IAsset.Name),
            nameof(IAsset.Taxonomy),
            nameof(IAsset.Files),
            nameof(IAsset.Attachments),
            nameof(IAsset.CreatedBy),
            nameof(IAsset.UpdatedBy),
        };
        const string k_NoneLabel = "None";
        const string k_NoCategoriesLabel = "Not Available";
        const string k_NoCollectionsLabel = "No Collections Found";

        IAssetManager m_AssetManager;
        IAsset m_SelectedAsset;

        MonoBehaviour m_CoroutineHandler;

        ScrollView m_AssetInformationPanelScrollView;
        VisualElement m_AssetInformationContainer;
        VisualElement m_AssetInformationDownloadSuccessful;
        VisualTreeAsset m_AssetInformationItemTemplate;
        VisualTreeAsset m_AssetInformationTagsTemplate;
        Button m_AssetDownloadButton;

        readonly HashSet<IAsset> m_InProgressDownloads = new();

        internal void Init(VisualElement root, VisualTreeAsset assetPanelItemTemplate, VisualTreeAsset tagsTemplate, MonoBehaviour coroutineHandler)
        {
            m_AssetInformationItemTemplate = assetPanelItemTemplate;
            m_AssetInformationTagsTemplate = tagsTemplate;
            m_CoroutineHandler = coroutineHandler;

            m_AssetManager = PlatformServices.AssetManager;

            m_AssetInformationPanelScrollView = root.Q<ScrollView>("AssetInformationScrollView");
            m_AssetInformationContainer = root.Q<VisualElement>("AssetInformationContainer");
            m_AssetDownloadButton = root.Q<Button>("AssetDownloadButton");
            m_AssetInformationDownloadSuccessful = root.Q<VisualElement>("AssetDownloadSuccessful");

            m_AssetDownloadButton.clickable.clicked += OnAssetDownloadButtonClicked;
        }

        internal void PopulateAssetPanel(IAsset assetInfo)
        {
            m_AssetInformationPanelScrollView.Clear();

            m_SelectedAsset = assetInfo;

            var propertyNames = m_SelectedAsset.GetType().GetProperties().Select(property => property.Name)
                .Where(name => !m_PropertiesToHide.Contains(name));

            foreach (var propertyName in propertyNames)
            {
                var propertyValue = m_SelectedAsset.GetType().GetProperty(propertyName)?.GetValue(m_SelectedAsset);

                if (string.IsNullOrEmpty(propertyValue?.ToString())) continue;

                var item = m_AssetInformationItemTemplate.Instantiate();
                item.Q<Label>("AssetInformationPropertyLabel").text = propertyName;

                switch (propertyValue)
                {
                    case DateTime propertyValueTime:
                        item.Q<Label>("AssetInformationValueLabel").text = propertyValueTime.ToString("MM/dd/yyyy");
                        break;
                    case int propertyValueInt:
                        item.Q<Label>("AssetInformationValueLabel").text = propertyValueInt.ToString();
                        break;
                    case AssetLocation propertyAssetLocation when string.IsNullOrEmpty(propertyAssetLocation.Name):
                        continue;
                    case AssetLocation propertyAssetLocation:
                        item.Q<Label>("AssetInformationValueLabel").text = propertyAssetLocation.Name;
                        break;
                    case AssetAuthor propertyValueAuthor when string.IsNullOrEmpty(propertyValueAuthor.Name):
                        continue;
                    case AssetAuthor propertyValueAuthor:
                        item.Q<Label>("AssetInformationValueLabel").text = propertyValueAuthor.Name;
                        break;
                    case ICollection<string> propertyValueTags when propertyName == nameof(IAsset.Tags):
                    {
                        PopulateTags(item, propertyValueTags);
                        break;
                    }
                    case ICollection<CollectionPath> when propertyName == nameof(IAsset.Collections):
                    {
                        _ = ListCollections(m_SelectedAsset, item);
                        break;
                    }
                    case ICollection<string> propertyValueCategories when propertyName == nameof(IAsset.Categories):
                    {
                        PopulateCategories(item, propertyValueCategories);
                        break;
                    }
                    default:
                        item.Q<Label>("AssetInformationValueLabel").text = propertyValue.ToString();
                        break;
                }

                m_AssetInformationPanelScrollView.Add(item);
            }

            UpdateDownloadButton(!m_InProgressDownloads.Contains(m_SelectedAsset));
        }

        void PopulateTags(VisualElement item, ICollection<string> propertyValueTags)
        {
            var label = item.Q<Label>("AssetInformationValueLabel");
            if (propertyValueTags.Count != 0)
            {
                label.style.display = DisplayStyle.None;
                foreach (var tag in propertyValueTags)
                {
                    var tagItem = m_AssetInformationTagsTemplate.Instantiate();
                    tagItem.Q<Button>("TagContainer").text = tag;
                    item.Q<VisualElement>("AssetInformationValue").Add(tagItem);
                }
            }
            else
            {
                label.text = k_NoneLabel;
            }
        }

        static void PopulateCategories(VisualElement item, ICollection<string> propertyValueCategories)
        {
            var label = item.Q<Label>("AssetInformationValueLabel");
            if (propertyValueCategories.Count != 0)
            {
                label.style.display = DisplayStyle.None;
                var foldout = new Foldout
                {
                    value = false,
                    text = propertyValueCategories.First()
                };

                foreach (var category in propertyValueCategories)
                {
                    var categoryItem = new Label
                    {
                        text = category
                    };
                    foldout.Add(categoryItem);
                }

                item.Q<VisualElement>("AssetInformationValue").Add(foldout);
            }
            else
            {
                label.text = k_NoCategoriesLabel;
            }
        }

        internal void DisplayAssetInformationPanel()
        {
            if (m_AssetInformationContainer != null) m_AssetInformationContainer.style.display = DisplayStyle.Flex;
        }

        internal void HideAssetInformationPanel()
        {
            if (m_AssetInformationContainer != null) m_AssetInformationContainer.style.display = DisplayStyle.None;
        }

        internal void SetAssetPanelName(IAsset selectedAssetName)
        {
            m_AssetInformationContainer.Q<Label>("AssetInformationLabel").text = selectedAssetName.Name;
        }

        async void OnAssetDownloadButtonClicked()
        {
            UpdateDownloadButton(false);

            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var assetToDownload = m_SelectedAsset;
            m_InProgressDownloads.Add(assetToDownload);

            try
            {
                await m_AssetManager.GetAssetDownloadUrlsAsync(assetToDownload, CancellationToken.None);

                foreach (var file in assetToDownload.Files)
                {
                    await using var destination = File.OpenWrite(Path.Combine(path, file.Name));
                    using var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(file.DownloadUrl));

                    using var response = await PlatformServices.HttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead, null,
                        CancellationToken.None);
                    response.EnsureSuccessStatusCode();

                    var source = await response.Content.ReadAsStreamAsync();
                    await source.CopyToAsync(destination);
                }

                m_CoroutineHandler.StartCoroutine(ShowSuccessfulDownload());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                m_InProgressDownloads.Remove(assetToDownload);
                if (m_SelectedAsset == assetToDownload)
                {
                    UpdateDownloadButton(true);
                }
            }
        }

        void UpdateDownloadButton(bool enable)
        {
            m_AssetDownloadButton.SetEnabled(enable);
            m_AssetDownloadButton.text = enable ? "Download" : "Downloading...";
        }

        IEnumerator ShowSuccessfulDownload()
        {
            m_AssetInformationDownloadSuccessful.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(3f);
            m_AssetInformationDownloadSuccessful.style.display = DisplayStyle.None;
        }

        async Task ListCollections(IAsset asset, TemplateContainer item)
        {
            var label = item.Q<Label>("AssetInformationValueLabel");
            label.style.display = DisplayStyle.None;
            var container = item.Q<VisualElement>("AssetInformationValue");

            // Showing asset in project All case
            if (asset.Project == null)
            {
                label.text = k_NoCollectionsLabel;
                label.style.display = DisplayStyle.Flex;
                return;
            }

            try
            {
                await PlatformServices.AssetManager.GetAssetCollectionsAsync(asset, CancellationToken.None);
            }
            finally
            {
                var collections = asset.Collections.ToArray();
                if (collections.Length != 0)
                {
                    foreach (var collection in collections)
                    {
                        var collectionItem = m_AssetInformationTagsTemplate.Instantiate();
                        collectionItem.Q<Button>("TagContainer").text = collection;
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
#endif

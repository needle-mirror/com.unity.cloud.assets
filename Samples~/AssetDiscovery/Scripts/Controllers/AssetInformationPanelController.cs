#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    class AssetInformationPanelController
    {
        readonly string[] m_PropertiesToHide = { "Taxonomy", "Details", "Metadata", "Files", "Attachments" };
        const string k_NoCategoriesLabel = "Not Available";
        const string k_NoCollectionsLabel = "No collections found";

        IEnumerable<PropertyInfo> m_PropertyList;
        IAsset m_SelectedAsset;

        ScrollView m_AssetInformationPanelScrollView;
        VisualTreeAsset m_AssetInformationItemTemplate;
        VisualTreeAsset m_AssetInformationTagsTemplate;

        internal void Init(ScrollView assetPanel, IAsset assetInfo, VisualTreeAsset assetPanelItemTemplate, VisualTreeAsset tagsTemplate)
        {
            m_AssetInformationPanelScrollView = assetPanel;
            m_SelectedAsset = assetInfo;
            m_AssetInformationItemTemplate = assetPanelItemTemplate;
            m_AssetInformationTagsTemplate = tagsTemplate;

            m_PropertyList = m_SelectedAsset.GetType().GetProperties().ToList()
                .Where(property => property != null && property.Name != nameof(IAsset.Name));

            m_AssetInformationPanelScrollView.Clear();

            PopulateAssetPanel();
        }

        void PopulateAssetPanel()
        {
            foreach (var property in m_PropertyList)
            {
                var propertyValue = m_SelectedAsset.GetType().GetProperty(property.Name)?.GetValue(m_SelectedAsset, null);

                if (!string.IsNullOrEmpty(propertyValue?.ToString()) && !m_PropertiesToHide.Contains(property.Name))
                {
                    var item = m_AssetInformationItemTemplate.Instantiate();
                    item.Q<Label>("AssetInformationPropertyLabel").text = property.Name;

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
                        case List<string> propertyValueTags when property.Name == nameof(IAsset.Tags):
                        {
                            item.Q<VisualElement>("AssetInformationValueLabel").style.display = DisplayStyle.None;
                            foreach (var tag in propertyValueTags)
                            {
                                var tagItem = m_AssetInformationTagsTemplate.Instantiate();
                                tagItem.Q<Button>("AssetInformationTag").text = tag;
                                item.Q<VisualElement>("AssetInformationValue").Add(tagItem);
                            }

                            break;
                        }
                        case List<string> propertyValueCollections when property.Name == nameof(IAsset.Collections):
                        {
                            _ = ListCollections(m_SelectedAsset, item);
                            break;
                        }
                        case List<string> propertyValueCategories when property.Name == nameof(IAsset.Categories):
                        {
                            if (propertyValueCategories.Count != 0)
                            {
                                item.Q<VisualElement>("AssetInformationValueLabel").style.display = DisplayStyle.None;
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
                                item.Q<Label>("AssetInformationValueLabel").text = k_NoCategoriesLabel;
                            }
                            break;
                        }
                        default:
                            item.Q<Label>("AssetInformationValueLabel").text = propertyValue.ToString();
                            break;
                    }
                    m_AssetInformationPanelScrollView.Add(item);
                }
            }
        }

        async Task ListCollections(IAsset asset, TemplateContainer item)
        {
            var label = item.Q<Label>("AssetInformationValueLabel");
            var container = item.Q<VisualElement>("AssetInformationValue");
            try
            {
                await PlatformServices.AssetManager.GetAssetCollectionsAsync(asset, CancellationToken.None);
            }
            finally
            {
                var collections = asset.Collections.ToArray();
                if (collections.Length != 0)
                {
                    item.Q<VisualElement>("AssetInformationValueLabel").style.display = DisplayStyle.None;
                    foreach (var collection in collections)
                    {
                        var collectionItem = m_AssetInformationTagsTemplate.Instantiate();
                        collectionItem.Q<Button>("AssetInformationTag").text = collection;
                        container.Add(collectionItem);
                    }
                }
                else
                {
                    label.text = k_NoCollectionsLabel;
                }
            }
        }
    }
}
#endif

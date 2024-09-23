using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    public class DatasetFoldout : Foldout
    {
        public delegate IEnumerable<VisualElement> CreatePropertyInformation(string propertyName, object propertyValue);

        static readonly Type k_DatasetType = typeof(IDataset);
        static readonly HashSet<string> k_DatasetPropertiesToHide = new()
        {
            nameof(IDataset.Name),
            nameof(IDataset.FileOrder),
            nameof(IDataset.Metadata),
            nameof(IDataset.SystemMetadata),
        };

        static List<string> k_DatasetPropertyNames;

        public ScrollView ScrollView { get; }

        VisualElement m_DownloadSuccessLabel;
        Button m_DownloadButton;

        public DatasetFoldout(IDataset dataset, CreatePropertyInformation createPropertyInformation)
        {
            FindPropertyNames();

            text = dataset.Name;
            value = false;

            ScrollView = new ScrollView();
            Add(ScrollView);

            foreach (var propertyName in k_DatasetPropertyNames)
            {
                var propertyValue = k_DatasetType.GetProperty(propertyName)?.GetValue(dataset);

                if (string.IsNullOrEmpty(propertyValue?.ToString())) continue;

                var propertyInformation = createPropertyInformation
                (
                    propertyName,
                    propertyValue
                );

                foreach (var property in propertyInformation)
                {
                    ScrollView.Add(property);
                }
            }

            m_DownloadSuccessLabel = GetDownloadSuccessLabel();
            Add(m_DownloadSuccessLabel);

            this.RegisterValueChangedCallback(evt =>
            {
                if (!evt.newValue)
                {
                    m_DownloadSuccessLabel.Hide();
                }
            });

            m_DownloadButton = new Button {text = "Download"};
            m_DownloadButton.AddToClassList("sample-button");
            m_DownloadButton.AddToClassList("button-blue");
            Add(m_DownloadButton);
        }

        public void RegisterDownloadButtonCallback(Action callback)
        {
            m_DownloadButton.clickable.clicked += callback;
        }

        public void ShowDownloadSuccessLabel()
        {
            m_DownloadSuccessLabel?.Show();
        }

        static void FindPropertyNames()
        {
            k_DatasetPropertyNames ??= k_DatasetType.GetProperties()
                .Select(property => property.Name)
                .Where(name => !k_DatasetPropertiesToHide.Contains(name))
                .ToList();
        }

        static VisualElement GetDownloadSuccessLabel()
        {
            var downloadSuccess = new Label
            {
                text = "Download Successful",
                style = {display = DisplayStyle.None}
            };
            downloadSuccess.AddToClassList("info-label");
            downloadSuccess.RegisterCallback<ClickEvent>(evt =>
            {
                evt.StopPropagation();
                downloadSuccess.Hide();
            });

            return downloadSuccess;
        }
    }
}

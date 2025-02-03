using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    public class DatasetFoldout : Foldout
    {
        public delegate IEnumerable<VisualElement> CreatePropertyInformation(string propertyName, object propertyValue);

        static readonly Type k_DatasetType = typeof(DatasetProperties);
        static readonly HashSet<string> k_DatasetPropertiesToHide = new()
        {
            nameof(DatasetProperties.Name),
            nameof(DatasetProperties.FileOrder),
        };

        static List<string> k_DatasetPropertyNames;

        public ScrollView ScrollView { get; }
        public Task<bool> CheckIfHasFilesTask { get; }

        VisualElement m_DownloadSuccessLabel;
        Button m_DownloadButton;

        public DatasetFoldout(IDataset dataset, CreatePropertyInformation createPropertyInformation)
        {
            FindPropertyNames();

            text = dataset.Descriptor.DatasetId.ToString();
            value = false;

            ScrollView = new ScrollView();
            Add(ScrollView);

            foreach (var property in createPropertyInformation(string.Empty, dataset.Descriptor))
            {
                ScrollView.Add(property);
            }

            _ = PopulatePropertiesAsync(dataset, createPropertyInformation);

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
            m_DownloadButton.SetEnabled(false);

            CheckIfHasFilesTask = CheckIfHasFiles(dataset);
        }

        async Task<bool> CheckIfHasFiles(IDataset dataset)
        {
            var asyncEnumerator = dataset.ListFilesAsync(new Range(0,1), CancellationToken.None).GetAsyncEnumerator();
            if (await asyncEnumerator.MoveNextAsync())
            {
                m_DownloadButton.SetEnabled(true);
                m_DownloadButton.tooltip = "No files to download.";
                return true;
            }

            return false;
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

        async Task PopulatePropertiesAsync(IDataset dataset, CreatePropertyInformation createPropertyInformation)
        {
            var properties = await dataset.GetPropertiesAsync(CancellationToken.None);

            text = properties.Name;

            foreach (var propertyName in k_DatasetPropertyNames)
            {
                var propertyValue = k_DatasetType.GetProperty(propertyName)?.GetValue(properties);

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

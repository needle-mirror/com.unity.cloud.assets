using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetManagerSample : MonoBehaviour
    {
        readonly AssetPanelController m_AssetPanelController = new();
        readonly DatasetPanelController m_DatasetPanelController = new();
        readonly FileController m_FileController = new();
        readonly AssetCreationController m_AssetCreationController = new();

        AddMetadataPopupController m_AddMetadataPopupController;

        [SerializeField]
        AssetController m_AssetController;

        [SerializeField]
        VisualTreeAsset m_AssetCreationPanelTemplate;
        [SerializeField]
        VisualTreeAsset m_DatasetCreationTemplate;
        [SerializeField]
        VisualTreeAsset m_TagsTemplate;

        VisualElement m_ContentPanel;
        VisualElement m_AssetPanel;
        VisualElement m_DatasetPanel;

        void Start()
        {
            var rootVisualElement = m_AssetController.RootVisualElement;

            m_ContentPanel = rootVisualElement.Q<VisualElement>("ContentPanel");
            m_ContentPanel.style.flexDirection = FlexDirection.Column;

            // Instantiate the asset panel
            m_AssetPanel = m_AssetCreationPanelTemplate.Instantiate();

            // Extract the embedded popup
            var popup = m_AssetPanel.Q("AddMetadataPopup");
            m_AddMetadataPopupController = new AddMetadataPopupController(popup);

            InstantiateDatasetCreationPanel();
            InstantiateAssetCreationPanel();

            // Add the popup after the other panels
            m_ContentPanel.Add(popup);

            m_AssetController.HideContent += HideContent;
            m_AssetController.OrganizationSelected += OnOrganizationSelected;
            m_AssetController.ProjectSelected += OnProjectSelected;
            m_AssetController.AssetSelected += OnAssetSelected;
            m_AssetController.CreateAsset += CreateAsset;
        }

        void OnDestroy()
        {
            m_AssetController.HideContent -= HideContent;
            m_AssetController.OrganizationSelected -= OnOrganizationSelected;
            m_AssetController.ProjectSelected -= OnProjectSelected;
            m_AssetController.AssetSelected -= OnAssetSelected;
            m_AssetController.CreateAsset -= CreateAsset;

            m_AssetPanelController.OnAssetUpdated -= OnAssetUpdated;
            m_AssetPanelController.OnDatasetOpen -= OnDatasetPanelOpen;
            m_AssetPanelController.PrepareAssetUpdateAsync -= m_DatasetPanelController.UpdateDatasetAsync;

            m_DatasetPanelController.PanelClosed -= OnDatasetPanelClosed;
            m_DatasetPanelController.Cleanup();

            m_AssetCreationController.AssetCreated -= m_DatasetPanelController.OnAssetCreated;
            m_AssetCreationController.ChangeButtonEnabledState -= m_DatasetPanelController.ChangeButtonEnabledState;
            m_AssetCreationController.Cleanup();

            m_FileController.Cleanup();

            m_AssetPanel.Q<Button>("BackBtn").UnregisterCallback<ClickEvent>(m_AssetController.OnBackButtonClicked);
        }

        void InstantiateAssetCreationPanel()
        {
            m_AssetPanel.style.flexGrow = 1;
            m_AssetPanel.Hide();

            m_ContentPanel.Add(m_AssetPanel);

            m_AssetPanelController.Init
            (
                m_AssetPanel,
                m_TagsTemplate,
                m_AddMetadataPopupController
            );
            m_AssetPanelController.OnAssetUpdated += OnAssetUpdated;
            m_AssetPanelController.OnDatasetOpen += OnDatasetPanelOpen;

            m_AssetPanel.Q<Button>("BackBtn").RegisterCallback<ClickEvent>(m_AssetController.OnBackButtonClicked);
        }

        void InstantiateDatasetCreationPanel()
        {
            m_DatasetPanel = m_DatasetCreationTemplate.Instantiate();
            m_DatasetPanel.style.flexGrow = 1;
            m_DatasetPanel.Hide();

            m_ContentPanel.Add(m_DatasetPanel);

            m_DatasetPanelController.Init
            (
                m_DatasetPanel,
                m_TagsTemplate,
                m_FileController,
                m_AddMetadataPopupController
            );
            m_DatasetPanelController.PanelClosed += OnDatasetPanelClosed;

            m_FileController.Init(m_DatasetPanel);

            m_AssetCreationController.Initialize
            (
                m_DatasetPanel,
                m_FileController
            );
            m_AssetCreationController.AssetCreated += m_DatasetPanelController.OnAssetCreated;
            m_AssetCreationController.ChangeButtonEnabledState += m_DatasetPanelController.ChangeButtonEnabledState;

            m_AssetPanelController.PrepareAssetUpdateAsync += m_DatasetPanelController.UpdateDatasetAsync;
        }

        void HideContent()
        {
            m_ContentPanel?.Hide();
            m_AssetController.AssetListPanel?.Hide();
            m_AssetPanel?.Hide();
            m_DatasetPanel?.Hide();
            m_AddMetadataPopupController?.Hide();
            DialogService.Hide();
        }

        void OnProjectSelected()
        {
            m_AssetController.AssetListPanel?.Show();
            m_ContentPanel?.Show();
        }

        void OnOrganizationSelected(OrganizationId orgId)
        {
            HideContent();

            m_AddMetadataPopupController.ListFieldDefinitions(orgId);
        }

        void OnAssetSelected(IAsset asset)
        {
            m_DatasetPanel?.Hide();
            m_DatasetPanelController.Clear();

            if (asset == null)
            {
                m_AssetPanel?.Hide();
                m_AssetPanelController.Clear();
                m_AssetController.AssetListPanel?.Show();
            }
            else
            {
                m_AssetController.AssetListPanel?.Hide();
                m_AssetPanelController.OpenAsset(asset);
                m_AssetPanel?.Show();
            }
        }

        void CreateAsset()
        {
            m_AssetController.AssetListPanel?.Hide();
            m_AssetPanel?.Hide();
            m_AssetPanelController.Clear();
            m_DatasetPanelController.Clear();

            m_DatasetPanel?.Show();
            m_AssetCreationController.Show(m_AssetController.SelectedProject);
        }

        async void OnAssetUpdated(IAsset asset)
        {
            await asset.RefreshAsync(CancellationToken.None);

            m_AssetPanelController.OpenAsset(asset);
            m_AssetController.OnAssetUpdated(asset);
        }

        void OnDatasetPanelOpen(IDataset dataset, bool canUpdate)
        {
            m_AssetPanel?.Hide();
            m_AssetCreationController.Hide();
            m_DatasetPanelController.OpenDataset(dataset, canUpdate);
            m_DatasetPanel?.Show();
        }

        void OnDatasetPanelClosed(IAsset asset)
        {
            m_AssetCreationController.Hide();
            m_DatasetPanel?.Hide();

            if (asset != null)
            {
                m_AssetController.OnAssetCreated(asset);
            }
            else if (m_AssetPanelController.CurrentAsset == null)
            {
                m_AssetController.AssetListPanel?.Show();
            }
            else
            {
                m_AssetPanel?.Show();
            }
        }
    }
}

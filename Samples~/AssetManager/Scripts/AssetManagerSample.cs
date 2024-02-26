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
        VisualTreeAsset m_FileListItemTemplate;
        [SerializeField]
        VisualTreeAsset m_DatasetListItemTemplate;
        [SerializeField]
        VisualTreeAsset m_TagsTemplate;
        [SerializeField]
        VisualTreeAsset m_PopupTemplate;

        VisualElement m_ContentPanel;
        VisualElement m_AssetPanel;
        VisualElement m_DatasetPanel;

        void Start()
        {
            var rootVisualElement = m_AssetController.RootVisualElement;

            m_ContentPanel = rootVisualElement.Q<VisualElement>("ContentPanel");

            var popupContainer = rootVisualElement.Q("PopupContainer");

            // Keep order to ensure correct display overlay
            var popups = m_PopupTemplate.Instantiate();
            popupContainer.Add(popups);
            m_AddMetadataPopupController = new AddMetadataPopupController(popups);

            InstantiateDatasetCreationPanel();
            InstantiateAssetCreationPanel();

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

            m_AssetPanel.Q<Button>("BackBtn").UnregisterCallback<ClickEvent>(OnBackButtonClicked);
        }

        void InstantiateAssetCreationPanel()
        {
            m_AssetPanel = m_AssetCreationPanelTemplate.Instantiate();
            m_AssetPanel.style.height = Length.Percent(100);
            m_AssetPanel.style.width = Length.Percent(100);
            m_AssetPanel.Hide();

            m_ContentPanel.Add(m_AssetPanel);

            m_AssetPanelController.Init
            (
                m_AssetPanel,
                m_DatasetListItemTemplate,
                m_TagsTemplate,
                m_AddMetadataPopupController
            );
            m_AssetPanelController.OnAssetUpdated += OnAssetUpdated;
            m_AssetPanelController.OnDatasetOpen += OnDatasetPanelOpen;

            m_AssetPanel.Q<Button>("BackBtn").RegisterCallback<ClickEvent>(OnBackButtonClicked);
        }

        void InstantiateDatasetCreationPanel()
        {
            m_DatasetPanel = m_DatasetCreationTemplate.Instantiate();
            m_DatasetPanel.style.height = Length.Percent(100);
            m_DatasetPanel.style.width = Length.Percent(100);
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

            m_FileController.Init
            (
                m_DatasetPanel,
                m_FileListItemTemplate
            );

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
            m_ContentPanel.style.display = DisplayStyle.None;
            m_AssetController.AssetListPanel?.Hide();
            m_AssetPanel?.Hide();
            m_DatasetPanel?.Hide();
            m_AddMetadataPopupController?.Hide();
            DialogService.Hide();
        }

        void OnProjectSelected()
        {
            m_AssetController.ClearSelection();

            m_AssetController.AssetListPanel?.Show();
            m_ContentPanel.style.display = DisplayStyle.Flex;
        }

        void OnOrganizationSelected(OrganizationId orgId)
        {
            HideContent();

            m_AddMetadataPopupController.ListFieldDefinitions(orgId);
        }

        void OnAssetSelected(IAsset asset)
        {
            m_AssetController.AssetListPanel?.Hide();
            m_DatasetPanel?.Hide();
            m_DatasetPanelController.Clear();

            if (asset == null)
            {
                m_AssetPanel?.Hide();
                m_AssetPanelController.Clear();
            }
            else
            {
                m_AssetPanelController.OpenAsset(asset);
                m_AssetPanel?.Show();
            }
        }

        void OnBackButtonClicked(ClickEvent evt)
        {
            m_AssetController.ClearSelection();

            m_AssetController.AssetListPanel?.Show();
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

        void OnAssetUpdated(IAsset asset)
        {
            _ = OnAssetUpdatedAsync(asset);
        }

        async Task OnAssetUpdatedAsync(IAsset asset)
        {
            await asset.RefreshAsync(CancellationToken.None);

            m_AssetPanelController.OpenAsset(asset);
            m_AssetController.OnAssetUpdated(asset);
        }

        void OnDatasetPanelOpen(IDataset dataset)
        {
            m_AssetPanel?.Hide();
            m_AssetCreationController.Hide();
            m_DatasetPanelController.OpenDataset(dataset);
            m_DatasetPanel?.Show();
        }

        void OnDatasetPanelClosed(IAsset asset)
        {
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

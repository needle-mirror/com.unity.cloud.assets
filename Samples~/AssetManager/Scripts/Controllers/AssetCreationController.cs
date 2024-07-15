using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetManager
{
    public class AssetCreationController
    {
        Button m_CreateAssetButton;
        ProgressBar m_ProgressBar;

        FileController m_FileController;

        IAssetProject m_AssetProject;
        AssetCreation m_CurrentAssetCreation;

        public event Action<bool> ChangeButtonEnabledState;
        public event Action<IAsset, IDataset> AssetCreated;

        public void Initialize(VisualElement root, FileController fileController)
        {
            m_FileController = fileController;

            m_CreateAssetButton = root.Q<Button>("CreateAssetButton");
            m_CreateAssetButton.style.display = DisplayStyle.None;
            m_CreateAssetButton.RegisterCallback<ClickEvent>(CreateAsset);

            m_ProgressBar = root.Q<ProgressBar>();
            m_ProgressBar?.Hide();

            m_FileController.FilesAdded += OnFilesAdded;
            m_FileController.FilesRemoved += OnFilesRemoved;
        }

        public void Cleanup()
        {
            m_CreateAssetButton.UnregisterCallback<ClickEvent>(CreateAsset);

            m_FileController.FilesAdded -= OnFilesAdded;
            m_FileController.FilesRemoved -= OnFilesRemoved;
        }

        public void Show(IAssetProject project)
        {
            m_AssetProject = project;
            var assetName = $"New Asset {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            m_CurrentAssetCreation = new AssetCreation(assetName)
            {
                Description = $"Description {assetName}"
            };

            m_FileController.Show();

            SetButtonsEnabled(true);
        }

        public void Hide()
        {
            m_CreateAssetButton?.Hide();
        }

        void CreateAsset(ClickEvent evt)
        {
            if (m_ProgressBar != null)
            {
                m_ProgressBar.title = "Creating asset...";
                m_ProgressBar.value = 0;
                m_ProgressBar.Show();
            }

            SetButtonsEnabled(false);

            _ = CreateAssetAsync();
        }

        async Task CreateAssetAsync()
        {
            if (m_AssetProject == null || m_CurrentAssetCreation == null)
            {
                SetButtonsEnabled(true);
                return;
            }

            if (m_FileController.TryGetNameAndType(out var name, out var type))
            {
                m_CurrentAssetCreation.Name = name;
                m_CurrentAssetCreation.Type = type;
            }
            else
            {
                m_CurrentAssetCreation.Type = AssetType.Other;
            }

            try
            {
                var createdAsset = await m_AssetProject.CreateAssetAsync(m_CurrentAssetCreation, default);
                if (createdAsset == null)
                {
                    DialogService.ShowMessage("Error", "Failed to create asset.");
                }
                else
                {
                    await OnAssetCreated(createdAsset);
                }
            }
            catch (OperationCanceledException oe)
            {
                oe.LogException();
                DialogService.ShowMessage("Error", "Failed to create asset. Request canceled.");
            }
            catch (Exception e)
            {
                e.LogException();
                DialogService.ShowMessage("Error", "Failed to create asset.");
            }
            finally
            {
                SetButtonsEnabled(true);
            }
        }

        async Task OnAssetCreated(IAsset createdAsset)
        {
            var sourceDataset = await createdAsset.GetSourceDatasetAsync(CancellationToken.None);
            if (sourceDataset == null)
            {
                Debug.LogError($"No datasets found for created asset {createdAsset.Name}.");
            }

            await m_FileController.UploadFiles(sourceDataset, m_ProgressBar);

            SetButtonsEnabled(true);

            m_FileController.Hide();

            AssetCreated?.Invoke(createdAsset, sourceDataset);
        }

        void OnFilesAdded(IEnumerable<string> files)
        {
            if (m_CurrentAssetCreation != null && files.Any())
            {
                m_CreateAssetButton?.Show();
            }
        }

        void OnFilesRemoved(IEnumerable<string> files)
        {
            if (!files.Any())
            {
                m_CreateAssetButton?.Hide();
            }
        }

        void SetButtonsEnabled(bool isEnabled)
        {
            if (isEnabled)
            {
                m_ProgressBar?.Hide();
            }

            m_CreateAssetButton.SetEnabled(isEnabled);
            ChangeButtonEnabledState?.Invoke(isEnabled);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCaseFileReuploadExample
    {
        readonly UseCaseFileReuploadExampleBehaviour m_Behaviour = new();

        public void DisplayExample(IAsset asset)
        {
            m_Behaviour.Initialize(asset);
            AssetActions();
        }

        #region Example_UI

        string m_FilePath = string.Empty;

        protected virtual void AssetActions()
        {
            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            if (GUILayout.Button("Refresh Files") || m_Behaviour.AssetFiles == null)
            {
                _ = m_Behaviour.GetAssetFiles();
            }

            GUILayout.Label("Upload file full path:");
            m_FilePath = GUILayout.TextField(m_FilePath);

            GUI.enabled = m_Behaviour.CanCancel;
            if (GUILayout.Button("Cancel"))
            {
                m_Behaviour.Cancel();
            }
            GUI.enabled = true;

            GUILayout.Label("Asset files:");

            // Get a local copy of the list of asset files to avoid concurrent modification exceptions.
            var assetFiles = m_Behaviour.AssetFiles?.ToArray() ?? Array.Empty<IFile>();
            foreach (var assetFile in assetFiles)
            {
                DisplayAssetFile(assetFile);
            }
        }

        void DisplayAssetFile(IFile assetFile)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{assetFile.Descriptor.Path}");
            GUILayout.Space(5f);

            GUI.enabled = !string.IsNullOrEmpty(m_FilePath) && File.Exists(m_FilePath);

            if (GUILayout.Button("Upload new content"))
            {
                var memoryStream = new MemoryStream(File.ReadAllBytes(m_FilePath));
                _ = m_Behaviour.ReplaceFileAsync(assetFile, memoryStream);
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }

        #endregion
    }

    class UseCaseFileReuploadExampleBehaviour
    {
        // Member names should match with the names of the get-started behaviour snippets.
        public IAsset CurrentAsset;

        public void Initialize(IAsset asset)
        {
            if (CurrentAsset != asset)
            {
                AssetFiles = null;
            }

            CurrentAsset = asset;
        }

        #region Example_Behaviour_RefreshFiles

        public List<IFile> AssetFiles;

        public async Task GetAssetFiles()
        {
            AssetFiles = new List<IFile>();

            var fileList = CurrentAsset.ListFilesAsync(Range.All, CancellationToken.None);
            await foreach (var file in fileList)
            {
                AssetFiles.Add(file);
            }
        }

        #endregion

        #region Example_Behaviour_UploadFile

        CancellationTokenSource m_CancellationTokenSource;

        public bool CanCancel => m_CancellationTokenSource is {IsCancellationRequested: false};

        class LogProgress : IProgress<HttpProgress>
        {
            public void Report(HttpProgress value)
            {
                if (!value.UploadProgress.HasValue) return;

                Debug.Log($"Upload progress: {value.UploadProgress * 100} %");
            }
        }

        public async Task ReplaceFileAsync(IFile file, MemoryStream memoryStream)
        {
            var cancellationToken = GetCancellationToken();

            var datasets = new List<IDataset>();
            var datasetList = file.GetLinkedDatasetsAsync(Range.All, cancellationToken);

            // Remove file from all datasets
            await foreach (var dataset in datasetList)
            {
                datasets.Add(dataset);
                try
                {
                    await dataset.RemoveFileAsync(file.Descriptor.Path, cancellationToken);
                    Debug.Log($"{file.Descriptor.Path} removed from {dataset.Name}.");
                }
                catch (OperationCanceledException)
                {
                    Debug.LogWarning("File replacement cancelled.");
                    return;
                }
                catch (AggregateException e)
                {
                    Debug.LogError($"Failed to remove file reference from {dataset.Name}. {e.InnerException}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to remove file reference from {dataset.Name}. {e}");
                }
            }

            if (datasets.Count == 0)
            {
                Cancel();
                return;
            }

            if (cancellationToken.IsCancellationRequested) return;

            // Reupload to dataset[0]
            var fileCreation = new FileCreation
            {
                Path = file.Descriptor.Path,
                Description = file.Description,
                Tags = file.Tags,
                Metadata = file.Metadata,
                SystemMetadata = file.SystemMetadata,
                PortalMetadata = file.PortalMetadata
            };

            IFile newFile = null;

            try
            {
                newFile = await datasets[0].UploadFileAsync(fileCreation, memoryStream, new LogProgress(), cancellationToken);
                Debug.Log($"{newFile.Descriptor.Path} uploaded to {datasets[0].Name}.");
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("File replacement cancelled.");
                return;
            }
            catch (AggregateException e)
            {
                Debug.LogError($"Failed to upload file to {datasets[0].Name}. {e.InnerException}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to upload file to {datasets[0].Name}. {e}");
            }

            if (newFile == null)
            {
                Cancel();
                return;
            }

            if (cancellationToken.IsCancellationRequested) return;

            // Link to remaining datasets
            for (var i = 1; i < datasets.Count; ++i)
            {
                try
                {
                    await datasets[i].AddExistingFileAsync(newFile.Descriptor.Path, newFile.Descriptor.DatasetId, cancellationToken);
                    Debug.Log($"{newFile.Descriptor.Path} linked to {datasets[i].Name}.");
                }
                catch (OperationCanceledException)
                {
                    Debug.LogWarning("File replacement cancelled.");
                    return;
                }
                catch (AggregateException e)
                {
                    Debug.LogError($"Failed to link file to {datasets[i].Name}. {e.InnerException}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to link file to {datasets[i].Name}. {e}");
                }
            }

            if (m_CancellationTokenSource.Token == cancellationToken)
            {
                m_CancellationTokenSource.Dispose();
                m_CancellationTokenSource = null;
            }
        }

        public void Cancel()
        {
            if (m_CancellationTokenSource != null)
            {
                m_CancellationTokenSource.Cancel();
                m_CancellationTokenSource.Dispose();
            }

            m_CancellationTokenSource = null;
        }

        CancellationToken GetCancellationToken()
        {
            Cancel();

            m_CancellationTokenSource = new CancellationTokenSource();
            return m_CancellationTokenSource.Token;
        }

        #endregion
    }
}

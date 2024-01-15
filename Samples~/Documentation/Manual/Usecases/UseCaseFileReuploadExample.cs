using System.Linq;

namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;
    using UnityEngine;

    public class UseCaseFileReuploadExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseFileReuploadExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseFileReuploadExample : IAssetManagementUI
    {
        readonly UseCaseFileReuploadExampleBehaviour m_Behaviour;

        public UseCaseFileReuploadExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseFileReuploadExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        string m_FilePath = string.Empty;

        IAsset m_CurrentAsset;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                m_Behaviour.AssetFiles = null;
            }

            GUILayout.BeginVertical();

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

            GUILayout.EndVertical();
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
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseFileReuploadExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshFiles

        public List<IFile> AssetFiles { get; set; }

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

            var metadata = await file.Metadata.Query().ExecuteAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested) return;

            var systemMetadata = await file.SystemMetadata.Query().ExecuteAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested) return;

            var datasets = new List<IDataset>();
            var datasetList = file.GetLinkedDatasetsAsync(Range.All, cancellationToken);

            // Remove file from all datasets
            await foreach (var dataset in datasetList)
            {
                datasets.Add(dataset);
                await RemoveFileAsync(dataset, file.Descriptor.Path, cancellationToken);
                if (cancellationToken.IsCancellationRequested) return;
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
                Metadata = metadata.ToDictionary(x => x.Key, x => (object)x.Value),
                SystemMetadata = systemMetadata.ToDictionary(x => x.Key, x => (object)x.Value)
            };

            var newFile = await UploadFileAsync(datasets[0], fileCreation, memoryStream, cancellationToken);
            if (cancellationToken.IsCancellationRequested) return;
            if (newFile == null)
            {
                Cancel();
                return;
            }

            if (cancellationToken.IsCancellationRequested) return;

            // Link to remaining datasets
            for (var i = 1; i < datasets.Count; ++i)
            {
                await AddFileAsync(datasets[i], newFile, cancellationToken);
                if (cancellationToken.IsCancellationRequested) return;
            }

            if (m_CancellationTokenSource.Token == cancellationToken)
            {
                m_CancellationTokenSource.Dispose();
                m_CancellationTokenSource = null;
            }
        }

        static async Task RemoveFileAsync(IDataset dataset, string path, CancellationToken cancellationToken)
        {
            try
            {
                await dataset.RemoveFileAsync(path, cancellationToken);
                Debug.Log($"{path} removed from {dataset.Name}.");
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("File replacement cancelled.");
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

        static async Task<IFile> UploadFileAsync(IDataset dataset, IFileCreation fileCreation, Stream memoryStream, CancellationToken cancellationToken)
        {
            try
            {
                var newFile = await dataset.UploadFileAsync(fileCreation, memoryStream, new LogProgress(), cancellationToken);
                Debug.Log($"{newFile.Descriptor.Path} uploaded to {dataset.Name}.");
                return newFile;
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("File replacement cancelled.");
            }
            catch (AggregateException e)
            {
                Debug.LogError($"Failed to upload file to {dataset.Name}. {e.InnerException}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to upload file to {dataset.Name}. {e}");
            }

            return null;
        }

        static async Task AddFileAsync(IDataset dataset, IFile file, CancellationToken cancellationToken)
        {
            try
            {
                await dataset.AddExistingFileAsync(file.Descriptor.Path, file.Descriptor.DatasetId, cancellationToken);
                Debug.Log($"{file.Descriptor.Path} linked to {dataset.Name}.");
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("File replacement cancelled.");
            }
            catch (AggregateException e)
            {
                Debug.LogError($"Failed to link file to {dataset.Name}. {e.InnerException}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to link file to {dataset.Name}. {e}");
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

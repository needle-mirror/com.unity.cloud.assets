namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;
    using UnityEngine;

    public class UseCaseFileCreationExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseFileCreationExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseFileCreationExample : IAssetManagementUI
    {
        readonly UseCaseFileCreationExampleBehaviour m_Behaviour;

        public UseCaseFileCreationExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseFileCreationExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;
        Vector2 m_DatasetsScrollPosition;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                m_Behaviour.Datasets = null;
                m_Behaviour.Files = null;
            }

            if (m_CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            GUILayout.BeginVertical();

            if (GUILayout.Button("Refresh All") || m_Behaviour.Datasets == null || m_Behaviour.Files == null)
            {
                _ = m_Behaviour.GetDatasets();
                _ = m_Behaviour.GetFiles();
            }

            GUILayout.Label("Asset datasets:");
            DisplayDatasets(m_Behaviour.Datasets?.ToArray() ?? Array.Empty<IDataset>());

            GUILayout.EndVertical();
        }

        void DisplayDatasets(IReadOnlyList<IDataset> datasets)
        {
            if (datasets.Count == 0)
            {
                GUILayout.Label(" ! No datasets !");
            }
            else
            {
                var files = m_Behaviour.Files?.ToArray() ?? Array.Empty<IFile>();

                m_DatasetsScrollPosition = GUILayout.BeginScrollView(m_DatasetsScrollPosition, GUILayout.Height(Screen.height * 0.8f));

                for (var i = 0; i < datasets.Count; ++i)
                {
                    var dataset = datasets[i];
                    DisplayDataset(dataset, files.Where(file => file.LinkedDatasets.Contains(dataset.Descriptor)).ToArray());

                    GUILayout.Space(10f);
                }

                GUILayout.EndScrollView();
            }
        }

        void DisplayDataset(IDataset dataset, IReadOnlyCollection<IFile> files)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(dataset.Name);

            GUILayout.Space(5f);

#if UNITY_EDITOR
            if (GUILayout.Button("Upload new file"))
            {
                var filePath = UnityEditor.EditorUtility.OpenFilePanel("File to upload", "Assets", string.Empty);
                if (!string.IsNullOrEmpty(filePath))
                    _ = m_Behaviour.UploadFile(dataset, filePath);
            }
#endif

            GUI.enabled = m_SelectedDataset == null;
            if (GUILayout.Button("Link asset file"))
            {
                m_WindowRect = new Rect(Screen.width * 0.4f, Screen.height * 0.4f, Screen.width * 0.2f, Screen.height * 0.2f);
                m_SelectedDataset = dataset;
                m_AvailableFiles = m_Behaviour.Files?.Where(f => !f.LinkedDatasets.Contains(dataset.Descriptor)).ToList();
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();

            if (m_SelectedDataset != null)
            {
                m_WindowRect = GUILayout.Window(0, m_WindowRect, DisplayWindow, "Select files to link");
            }

            if (files.Count == 0)
            {
                GUILayout.Label(" ! No files !");
            }
            else
            {


                foreach (var file in files)
                {
                    DisplayFile(dataset, file);
                }
            }
        }

        void DisplayFile(IDataset dataset, IFile file)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"  > {file.Descriptor.Path} ({file.SizeBytes} kb)");
            if (GUILayout.Button("Unlink"))
            {
                _ = m_Behaviour.UnlinkFile(dataset, file);
            }

            GUILayout.EndHorizontal();
        }

        Rect m_WindowRect;
        IDataset m_SelectedDataset;
        List<IFile> m_AvailableFiles;

        void DisplayWindow(int windowId)
        {
            GUILayout.BeginVertical();

            GUILayout.Label($"Link files to {m_SelectedDataset.Name}:");

            if (m_AvailableFiles.Count == 0)
            {
                GUILayout.Label(" ! No files !");
            }
            else
            {
                for (var i = 0; i < m_AvailableFiles.Count; ++i)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Label(m_AvailableFiles[i].Descriptor.Path);

                    if (GUILayout.Button("Link"))
                    {
                        _ = m_Behaviour.LinkFile(m_SelectedDataset, m_AvailableFiles[i]);
                        m_AvailableFiles.RemoveAt(i);
                        GUILayout.EndHorizontal();
                        break;
                    }

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(10f);

            if (GUILayout.Button("Close"))
            {
                m_SelectedDataset = null;
                m_AvailableFiles = null;
                _ = m_Behaviour.GetFiles();
            }

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseFileCreationExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseFileCreationExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshDatasets

        public List<IDataset> Datasets { get; set; }

        public async Task GetDatasets()
        {
            Datasets = new List<IDataset>();

            await CurrentAsset.RefreshAsync(CancellationToken.None);
            var asyncList = CurrentAsset.ListDatasetsAsync(Range.All, CancellationToken.None);
            await foreach (var dataset in asyncList)
            {
                Datasets.Add(dataset);
            }
        }

        #endregion

        #region Example_Behaviour_RefreshFiles

        public List<IFile> Files { get; set; }

        public async Task GetFiles()
        {
            Files = new List<IFile>();

            await CurrentAsset.RefreshAsync(CancellationToken.None);
            var asyncList = CurrentAsset.ListFilesAsync(Range.All, CancellationToken.None);
            await foreach (var file in asyncList)
            {
                Files.Add(file);
            }
        }

        #endregion

        #region Example_Behaviour_UploadAssetFile

        class LogProgress : IProgress<HttpProgress>
        {
            public void Report(HttpProgress value)
            {
                if (!value.UploadProgress.HasValue) return;

                Debug.Log($"Upload progress: {value.UploadProgress * 100} %");
            }
        }

        public async Task UploadFile(IDataset dataset, string filePath)
        {
            var fileCreation = new FileCreation(Path.GetFileName(filePath))
            {
                Description = "Documentation example asset file creation.",
                Tags = new List<string> {"Texture", "Gray"}
            };

            var cancellationTokenSrc = new CancellationTokenSource();
            try
            {
                var progress = new LogProgress();

                var fileStream = File.OpenRead(filePath);
                var file = await dataset.UploadFileAsync(fileCreation, fileStream, progress, cancellationTokenSrc.Token);
                Files.Add(file);

                Debug.Log($"Asset file upload: {file.Descriptor.Path} added and uploaded.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to upload file: {fileCreation.Path}. {e}");
            }
        }

        #endregion

        #region Example_Behaviour_AddFileReference

        public async Task LinkFile(IDataset dataset, IFile file)
        {
            try
            {
                await dataset.AddExistingFileAsync(file.Descriptor.Path, file.Descriptor.DatasetId, CancellationToken.None);
                Debug.Log($"File: {file.Descriptor.Path} linked to dataset {dataset.Descriptor.DatasetId}.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to link asset file: {file.Descriptor.Path}. {e}");
            }
        }

        #endregion

        #region Example_Behaviour_RemoveFileReference

        public async Task UnlinkFile(IDataset dataset, IFile file)
        {
            try
            {
                await dataset.RemoveFileAsync(file.Descriptor.Path, CancellationToken.None);
                Debug.Log($"File: {file.Descriptor.Path} unlinked from dataset {dataset.Descriptor.DatasetId}.");
                Files = null;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to unlink asset file: {file.Descriptor.Path}. {e}");
            }
        }

        #endregion
    }
}

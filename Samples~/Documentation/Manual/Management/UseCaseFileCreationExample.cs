using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCaseFileCreationExample
    {
        readonly UseCaseFileCreationExampleBehaviour m_Behaviour = new();

        public void DisplayExample(IAsset asset)
        {
            m_Behaviour.Initialize(asset);
            AssetActions();
        }

        #region Example_UI

        protected virtual void AssetActions()
        {
            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            if (GUILayout.Button("Refresh All") || m_Behaviour.Datasets == null || m_Behaviour.Files == null)
            {
                _ = m_Behaviour.GetDatasets();
                _ = m_Behaviour.GetFiles();
            }

            GUILayout.Label("Asset datasets:");
            DisplayDatasets(m_Behaviour.Datasets?.ToArray() ?? Array.Empty<IDataset>());
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

                for (var i = 0; i < datasets.Count; ++i)
                {
                    var dataset = datasets[i];
                    DisplayDataset(dataset, files.Where(file => file.LinkedDatasets.Contains(dataset.Descriptor)).ToArray());

                    GUILayout.Space(10);
                }
            }
        }

        void DisplayDataset(IDataset dataset, IReadOnlyCollection<IFile> files)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(dataset.Name);

            GUILayout.Space(5f);

            if (GUILayout.Button("Create new asset file"))
            {
                _ = m_Behaviour.UploadAssetFile(dataset);
            }

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
                    DisplayAssetFile(dataset, file);
                }
            }
        }

        void DisplayAssetFile(IDataset dataset, IFile file)
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
        // Member names should match with the names of the get-started behaviour snippets.
        public IAsset CurrentAsset;

        public void Initialize(IAsset asset)
        {
            if (asset != CurrentAsset)
            {
                Datasets = null;
                Files = null;
            }

            CurrentAsset = asset;
        }

        #region Example_Behaviour_RefreshDatasets

        public List<IDataset> Datasets { get; private set; }

        public async Task GetDatasets()
        {
            Datasets = new List<IDataset>();

            await CurrentAsset.RefreshAsync(new FieldsFilter {AssetFields = AssetFields.datasets}, CancellationToken.None);
            var asyncList = CurrentAsset.ListDatasetsAsync(Range.All, CancellationToken.None);
            await foreach (var dataset in asyncList)
            {
                Datasets.Add(dataset);
            }
        }

        #endregion

        #region Example_Behaviour_RefreshFiles

        public List<IFile> Files { get; private set; }

        public async Task GetFiles()
        {
            Files = new List<IFile>();

            await CurrentAsset.RefreshAsync(new FieldsFilter {AssetFields = AssetFields.files, FileFields = FileFields.fileSize}, CancellationToken.None);
            var asyncList = CurrentAsset.ListFilesAsync(Range.All, CancellationToken.None);
            await foreach (var file in asyncList)
            {
                Files.Add(file);
            }
        }

        #endregion

        #region Example_Behaviour_UploadAssetFile

        static readonly byte[] s_Bytes = new byte[]
        {
            100, 100, 100, 100, 100, 100, 100, 100, 100, 100
        };

        class LogProgress : IProgress<HttpProgress>
        {
            public void Report(HttpProgress value)
            {
                if (!value.UploadProgress.HasValue) return;

                Debug.Log($"Upload progress: {value.UploadProgress * 100} %");
            }
        }

        public async Task UploadAssetFile(IDataset dataset)
        {
            var fileCreation = new FileCreation
            {
                Path = $"file_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}",
                Description = "Documentation example asset file creation.",
                Tags = new List<string> {"Texture", "Gray"}
            };

            var contentStream = new MemoryStream(s_Bytes);

            var cancellationTokenSrc = new CancellationTokenSource();
            try
            {
                var progress = new LogProgress();

                var file = await dataset.UploadFileAsync(fileCreation, contentStream, progress, cancellationTokenSrc.Token);
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

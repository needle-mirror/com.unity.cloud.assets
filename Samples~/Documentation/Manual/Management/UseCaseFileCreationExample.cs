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

            GUILayout.EndHorizontal();

            if (files.Count == 0)
            {
                GUILayout.Label(" ! No files !");
            }
            else
            {
                foreach (var file in files)
                {
                    DisplayAssetFile(file);
                }
            }
        }

        static void DisplayAssetFile(IFile file)
        {
            GUILayout.Label($"  > {file.Descriptor.Path}");
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
                Path = CurrentAsset.Name + $"_file_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}",
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
                Debug.LogError($"Failed to upload asset file: {fileCreation.Path}. {e}");
            }
        }

        #endregion
    }
}

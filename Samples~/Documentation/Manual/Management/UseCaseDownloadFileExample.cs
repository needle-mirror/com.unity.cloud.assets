using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCaseDownloadFileExample
    {
        readonly UseCaseDownloadFileExampleBehaviour m_Behaviour = new();

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

            if (GUILayout.Button("Refresh Files") || m_Behaviour.AssetFiles == null)
            {
                _ = m_Behaviour.GetAssetFiles();
            }

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

            if (GUILayout.Button("Download"))
            {
                _ = m_Behaviour.DownloadAssetFile(assetFile);
            }

            GUILayout.EndHorizontal();
        }

        #endregion
    }

    class UseCaseDownloadFileExampleBehaviour
    {
        // Member names should match with the names of the get-started behaviour snippets.
        public IAsset CurrentAsset;

        public void Initialize(IAsset asset)
        {
            CurrentAsset = asset;
        }

        #region Example_Behaviour_DownloadAssetFile

        class LogProgress : IProgress<HttpProgress>
        {
            public void Report(HttpProgress value)
            {
                if (!value.DownloadProgress.HasValue) return;

                Debug.Log($"Download progress: {value.DownloadProgress * 100} %");
            }
        }

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

        public async Task DownloadAssetFile(IFile assetFile)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var cancellationTokenSrc = new CancellationTokenSource();
            try
            {
                await using var destination = File.OpenWrite(Path.Combine(path, assetFile.Descriptor.Path));

                var progress = new LogProgress();
                await assetFile.DownloadAsync(destination, progress, cancellationTokenSrc.Token);

                Debug.Log($"Asset file downloaded: {assetFile.Descriptor.Path}.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to download asset file: {assetFile.Descriptor.Path}. {e}");
            }
        }

        #endregion
    }
}

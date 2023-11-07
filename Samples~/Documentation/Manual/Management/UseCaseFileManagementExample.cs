using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCaseFileManagementExample
    {
        readonly UseCaseFileManagementExampleBehaviour m_Behaviour = new();

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

            if (GUILayout.Button("Refresh Files") || m_Behaviour.Files == null)
            {
                _ = m_Behaviour.GetAssetFiles();
            }

            GUILayout.Label("Files:");

            // Get a local copy of the list of asset files to avoid concurrent modification exceptions.
            var assetFiles = m_Behaviour.Files?.ToArray() ?? Array.Empty<IFile>();
            foreach (var assetFile in assetFiles)
            {
                DisplayAssetFile(assetFile);
            }
        }

        void DisplayAssetFile(IFile assetFile)
        {
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label($"{assetFile.Descriptor.Path}");
            GUILayout.Label($"{assetFile.Description}");
            GUILayout.Space(5f);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateAssetFile(assetFile);
            }

            if (GUILayout.Button("Download"))
            {
                _ = m_Behaviour.DownloadAssetFile(assetFile);
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseFileManagementExampleBehaviour
    {
        // Member names should match with the names of the get-started behaviour snippets.
        public IAsset CurrentAsset;

        public void Initialize(IAsset asset)
        {
            if (CurrentAsset != asset)
            {
                CurrentAsset = asset;
                Files = null;
            }
        }

        #region Example_Behaviour_RefreshFiles

        public List<IFile> Files;

        public async Task GetAssetFiles()
        {
            Files = new List<IFile>();

            var fileList = CurrentAsset.ListFilesAsync(Range.All, CancellationToken.None);
            await foreach (var file in fileList)
            {
                Files.Add(file);
            }
        }

        #endregion

        #region Example_Behaviour_UpdateAssetFile

        public async Task UpdateAssetFile(IFile assetFile)
        {
            var fileUpdate = new FileUpdate(assetFile)
            {
                Description = Guid.NewGuid().ToString()[..3]
            };

            var cancellationTokenSrc = new CancellationTokenSource();
            await assetFile.UpdateAsync(fileUpdate, cancellationTokenSrc.Token);
            Debug.Log("File updated.");
        }

        #endregion

        #region Example_Behaviour_DownloadAssetFile

        class LogProgress : IProgress<HttpProgress>
        {
            public void Report(HttpProgress value)
            {
                if (!value.DownloadProgress.HasValue) return;

                Debug.Log($"Download progress: {value.DownloadProgress * 100} %");
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

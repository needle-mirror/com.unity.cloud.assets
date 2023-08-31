using System;
using System.IO;
using System.Linq;
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

            GUILayout.Label("Asset files:");
            var assetFiles = m_Behaviour.CurrentAsset.Files.ToList();
            for (var i = 0; i < assetFiles.Count; ++i)
            {
                DisplayAssetFile((m_Behaviour.CurrentAsset,assetFiles.ElementAt(i)));
            }
        }

        void DisplayAssetFile((IAsset asset, IAssetFile assetFile) tuple)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{tuple.assetFile.Name}");
            GUILayout.Space(5f);

            if (GUILayout.Button("Download"))
            {
                _ = m_Behaviour.DownloadAssetFile(tuple.asset, tuple.assetFile);
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

                Debug.Log($"Download progress: {value.DownloadProgress*100} %");
            }
        }

        public async Task DownloadAssetFile(IAsset asset, IAssetFile assetFile)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var cancellationTokenSrc = new CancellationTokenSource();
            try
            {
                await using var destination = File.OpenWrite(Path.Combine(path, assetFile.Name));

                var progress = new LogProgress();
                await PlatformServices.AssetFileManager.DownloadAssetFileAsync(asset.Project, assetFile, destination, progress, cancellationTokenSrc.Token);

                Debug.Log($"Asset file downloaded: {assetFile.Name}.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to download asset file: {assetFile.Name}. {e.Message}");
            }
        }

        #endregion
    }
}

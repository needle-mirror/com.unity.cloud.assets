using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

            if (GUILayout.Button("Get download URL"))
            {
                _ = UseCaseFileManagementExampleBehaviour.DownloadAsync(assetFile);
            }

            if (GUILayout.Button("Update"))
            {
                _ = UseCaseFileManagementExampleBehaviour.UpdateAssetFile(assetFile);
            }

            GUILayout.EndHorizontal();
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
                AssetFiles = null;
            }
        }

        #region Example_Behaviour_RefreshAssetFiles

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

        #region Example_Behaviour_UpdateAssetFile

        public static async Task UpdateAssetFile(IFile assetFile)
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

        #region Example_Behaviour_DownloadUrls

        public static async Task DownloadAsync(IFile file)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            try
            {
                await using var destination = File.OpenWrite(Path.Combine(path, file.Descriptor.Path));
                await file.DownloadAsync(destination, null, CancellationToken.None);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        #endregion
    }
}

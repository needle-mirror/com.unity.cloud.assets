using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCaseFileManagementExample
    {
        readonly UseCaseFileManagementExampleBehaviour m_Behaviour = new();

        public void DisplayExample(IProject project, IAsset asset)
        {
            m_Behaviour.Initialize(project, asset);
            AssetActions();
        }

        #region Example_UI

        protected virtual void AssetActions()
        {
            if (GUILayout.Button("Refresh file list"))
            {
                _ = m_Behaviour.RefreshAssetFiles();
            }

            GUILayout.Label("Asset files:");
            if (m_Behaviour.CurrentAsset != null)
            {
                // Get a local copy of the list of asset files to avoid concurrent modification exceptions.
                var assetFiles = m_Behaviour.CurrentAsset.Files.ToArray();
                foreach (var assetFile in assetFiles)
                {
                    DisplayAssetFile(assetFile);
                }
            }
            else
            {
                GUILayout.Label(" ! No asset selected !");
            }
        }

        void DisplayAssetFile(IAssetFile assetFile)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{assetFile.Name}");
            GUILayout.Space(5f);

            if (GUILayout.Button("Get download URL"))
            {
                _ = m_Behaviour.GetDownloadUrlAsync(assetFile);
            }

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateAssetFile(assetFile);
            }

            if (GUILayout.Button("Delete"))
            {
                _ = m_Behaviour.DeleteAssetFile(assetFile);
            }

            GUILayout.EndHorizontal();
        }

        #endregion
    }

    class UseCaseFileManagementExampleBehaviour
    {
        // Member names should match with the names of the get-started behaviour snippets.
        IProject m_CurrentProject;
        public IAsset CurrentAsset;

        public void Initialize(IProject project, IAsset asset)
        {
            m_CurrentProject = project;
            CurrentAsset = asset;
        }

        #region Example_Behaviour_RefreshAssetFiles

        public async Task RefreshAssetFiles()
        {
            var cancellationTokenSrc = new CancellationTokenSource();
            await PlatformServices.AssetManager.GetAssetDownloadUrlsAsync(CurrentAsset, cancellationTokenSrc.Token);
        }

        #endregion

        #region Example_Behaviour_UpdateAssetFile

        public async Task UpdateAssetFile(IAssetFile assetFile)
        {
            var name = assetFile.Name.Split('_');
            var index = int.Parse(name[1]) + 1;
            assetFile.Name = $"{name[0]}_{index}";

            var cancellationTokenSrc = new CancellationTokenSource();
            await PlatformServices.AssetFileManager.UpdateAssetFileAsync(m_CurrentProject, assetFile, cancellationTokenSrc.Token);
            Debug.Log("File updated.");
        }

        #endregion

        #region Example_Behaviour_DeleteAssetFile

        public async Task DeleteAssetFile(IAssetFile assetFile)
        {
            var cancellationTokenSrc = new CancellationTokenSource();
            await PlatformServices.AssetFileManager.DeleteAssetFileAsync(m_CurrentProject, assetFile, cancellationTokenSrc.Token);
            await RefreshAssetFiles();
            Debug.Log("File deleted.");
        }

        #endregion

        #region Example_Behaviour_DownloadUrls

        public async Task GetDownloadUrlAsync(IAssetFile assetFile)
        {
            var cancellationTokenSrc = new CancellationTokenSource();
            var downloadUrl = await PlatformServices.AssetFileManager.GetAssetFileUrlAsync(m_CurrentProject, assetFile, AssetFileUrlType.Download, cancellationTokenSrc.Token);
            Debug.Log($"Download URL: {downloadUrl}");
        }

        #endregion
    }
}

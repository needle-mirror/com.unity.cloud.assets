using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    public class UseCaseFileManagementExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseFileManagementExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseFileManagementExample : IAssetManagementUI
    {
        readonly UseCaseFileManagementExampleBehaviour m_Behaviour;

        public UseCaseFileManagementExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseFileManagementExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;
        Vector2 m_FilesScrollPosition;

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
                m_Behaviour.Files = null;
            }

            GUILayout.BeginVertical();

            if (GUILayout.Button("Refresh Files") || m_Behaviour.Files == null)
            {
                _ = m_Behaviour.GetAssetFiles();
            }

            GUILayout.Label("Files:");

            GUILayout.Space(5f);

            m_FilesScrollPosition = GUILayout.BeginScrollView(m_FilesScrollPosition, GUILayout.MaxHeight(Screen.height * 0.8f));

            // Get a local copy of the list of asset files to avoid concurrent modification exceptions.
            var assetFiles = m_Behaviour.Files?.ToArray() ?? Array.Empty<IFile>();
            foreach (var assetFile in assetFiles)
            {
                DisplayAssetFile(assetFile);
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
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
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseFileManagementExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_RefreshFiles

        public List<IFile> Files { get; set; }

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

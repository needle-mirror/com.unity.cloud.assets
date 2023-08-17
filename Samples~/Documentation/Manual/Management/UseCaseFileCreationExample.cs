using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Documentation.Management
{
    public class UseCaseFileCreationExample
    {
        readonly UseCaseFileCreationExampleBehaviour m_Behaviour = new();

        public void DisplayExample(IProject project, IAsset asset)
        {
            m_Behaviour.Initialize(project, asset);
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

            if (GUILayout.Button("Create new asset file"))
            {
                _ = m_Behaviour.CreateAssetFile();
            }

            GUILayout.Space(10);

            GUILayout.Label("Asset files:");
            var assetFiles = m_Behaviour.NewAssetFiles;
            for (var i = 0; i < assetFiles.Count; ++i)
            {
                DisplayAssetFile(assetFiles[i]);
            }
        }

        void DisplayAssetFile((IAsset asset, IAssetFile assetFile) tuple)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"{tuple.assetFile.Name}");
            GUILayout.Space(5f);

            if (GUILayout.Button("Upload"))
            {
                _ = m_Behaviour.UploadAssetFile(tuple.asset, tuple.assetFile);
            }

            GUILayout.EndHorizontal();
        }

        #endregion
    }

    class UseCaseFileCreationExampleBehaviour
    {
        // Member names should match with the names of the get-started behaviour snippets.
        IProject m_CurrentProject;
        public IAsset CurrentAsset;

        public void Initialize(IProject project, IAsset asset)
        {
            m_CurrentProject = project;
            CurrentAsset = asset;
        }

        #region Example_Behaviour_CreateAssetFile

        static readonly byte[] s_Bytes = new byte[]
        {
            100, 100, 100, 100, 100, 100, 100, 100, 100, 100
        };

        public List<(IAsset, IAssetFile)> NewAssetFiles { get; } = new();

        public async Task CreateAssetFile()
        {
            var fileCreation = new AssetFileCreation
            {
                Name = CurrentAsset.Name + "_file",
                Description = "Documentation example asset file creation.",
                Type = nameof(Texture2D),
                FileSize = s_Bytes.LongLength,
                Tags = new List<string> {"Texture", "Gray"}
            };

            var cancellationTokenSrc = new CancellationTokenSource();

            try
            {
                var assetFile = await PlatformServices.AssetFileManager.CreateAssetFileAsync(m_CurrentProject, CurrentAsset, fileCreation, cancellationTokenSrc.Token);
                if (assetFile != null)
                {
                    NewAssetFiles.Add((CurrentAsset, assetFile));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create asset file. {e.Message}");
                throw;
            }
        }

        #endregion

        #region Example_Behaviour_UploadAssetFile

        public async Task UploadAssetFile(IAsset asset, IAssetFile assetFile)
        {
            // Uses the same texture as the file creation.
            var contentStream = new MemoryStream(s_Bytes);

            var cancellationTokenSrc = new CancellationTokenSource();
            try
            {
                var didUpload = await PlatformServices.AssetFileManager.UploadAssetFileAsync(asset.Project, assetFile, contentStream, cancellationTokenSrc.Token);
                if (!didUpload)
                {
                    throw new Exception();
                }

                await PlatformServices.AssetFileManager.FinalizeAssetFileUploadAsync(asset.Project, assetFile, cancellationTokenSrc.Token);
                Debug.Log($"Asset file upload: {assetFile.Name} finalized.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to upload asset file: {assetFile.Name}. {e.Message}");
            }
        }

        #endregion
    }
}

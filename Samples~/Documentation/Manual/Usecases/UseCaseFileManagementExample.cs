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

        IFile m_CurrentFile;
        FileUpdate m_FileUpdate;
        IEnumerable<GeneratedTag> m_GeneratedTags;

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
                m_CurrentFile = null;
                m_FileUpdate = null;
                m_GeneratedTags = null;
            }

            GUILayout.BeginVertical();

            if (GUILayout.Button("Refresh Files") || m_Behaviour.Files == null)
            {
                _ = m_Behaviour.GetFilesAsync();
            }

            GUILayout.Space(5f);

            m_FilesScrollPosition = GUILayout.BeginScrollView(m_FilesScrollPosition, GUILayout.MaxHeight(Screen.height * 0.8f), GUILayout.Width(Screen.width * 0.3f));

            DisplayFiles();

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            DisplaySelectedFile();

            GUILayout.EndVertical();
        }

        void DisplayFiles()
        {
            // Get a local copy of the list of asset files to avoid concurrent modification exceptions.
            var assetFiles = m_Behaviour.Files?.ToArray() ?? Array.Empty<IFile>();
            foreach (var assetFile in assetFiles)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label($"{assetFile.Descriptor.Path}");

                if (GUILayout.Button("Select"))
                {
                    m_CurrentFile = assetFile;
                    m_FileUpdate = new FileUpdate(assetFile);

                    m_Behaviour.CancelTagGeneration();
                    m_GeneratedTags = null;
                }

                if (GUILayout.Button("Download"))
                {
                    _ = m_Behaviour.DownloadFileAsync(assetFile);
                }

                GUILayout.EndHorizontal();
            }
        }

        void DisplaySelectedFile()
        {
            if (m_CurrentFile == null)
            {
                GUILayout.Label("! No file selected !");
                return;
            }

            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label($"{m_CurrentFile.Descriptor.Path}");

            GUILayout.Label($"Status: {m_CurrentFile.Status}");

            var createdDate = m_CurrentFile.AuthoringInfo?.Created.ToString("d") ?? "unknown";
            GUILayout.Label($"Created on: {createdDate}");

            var modifiedDate = m_CurrentFile.AuthoringInfo?.Updated.ToString("d") ?? "unknown";
            GUILayout.Label($"Last modified on: {modifiedDate}");

            GUILayout.Label($"Size: {m_CurrentFile.SizeBytes} bytes");

            GUILayout.EndVertical();

            GUILayout.Space(5f);

            GUILayout.Label("Description:");
            m_FileUpdate.Description = GUILayout.TextField(m_FileUpdate.Description);

            GUILayout.Label("Tags:");
            var tags = string.Join(", ", m_FileUpdate.Tags ?? Array.Empty<string>());
            m_FileUpdate.Tags = GUILayout.TextField(tags).Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            if (GUILayout.Button("Generate Tags"))
            {
                _ = GenerateTagsAsync();
            }

            DisplayGeneratedTags();

            GUILayout.Space(5f);

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateFileAsync(m_CurrentFile, m_FileUpdate);
            }
        }

        async Task GenerateTagsAsync()
        {
            m_GeneratedTags = null;
            m_GeneratedTags = await m_Behaviour.GenerateTagsAsync(m_CurrentFile);
        }

        void DisplayGeneratedTags()
        {
            if (m_GeneratedTags != null)
            {
                foreach (var tag in m_GeneratedTags)
                {
                    GUILayout.BeginHorizontal();

                    GUI.enabled = !m_FileUpdate.Tags?.Contains(tag.Value) ?? true;

                    if (GUILayout.Button("Add"))
                    {
                        m_FileUpdate.Tags ??= Array.Empty<string>();
                        m_FileUpdate.Tags = m_FileUpdate.Tags.Append(tag.Value).ToArray();
                    }

                    GUILayout.Label($"{tag.Value}, Confidence: {tag.Confidence:F3}");

                    GUI.enabled = true;

                    GUILayout.EndHorizontal();
                }
            }
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

        public async Task GetFilesAsync()
        {
            Files = new List<IFile>();

            try
            {
                _ = CurrentAsset.RefreshAsync(CancellationToken.None);
                var fileList = CurrentAsset.ListFilesAsync(Range.All, CancellationToken.None);
                await foreach (var file in fileList)
                {
                    Files.Add(file);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        #endregion

        #region Example_Behaviour_UpdateAssetFile

        public async Task UpdateFileAsync(IFile assetFile, IFileUpdate fileUpdate)
        {
            await assetFile.UpdateAsync(fileUpdate, CancellationToken.None);
            Debug.Log("File updated.");
            await assetFile.RefreshAsync(CancellationToken.None);
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

        public async Task DownloadFileAsync(IFile assetFile)
        {
            const string dialogHeader = "Download file to location:";

            var defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var folder = UnityEditor.EditorUtility.OpenFolderPanel(dialogHeader, defaultFolder, "");

            if (string.IsNullOrEmpty(folder)) return;

            try
            {
                var filePath = Path.Combine(folder, assetFile.Descriptor.Path);

                // Create the necessary directories
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await using var destination = File.OpenWrite(filePath);

                var progress = new LogProgress();
                await assetFile.DownloadAsync(destination, progress, default);

                Debug.Log($"Asset file downloaded: {assetFile.Descriptor.Path}.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to download asset file: {assetFile.Descriptor.Path}. {e}");
            }
        }

        #endregion

        #region Example_Behaviour_GenerateFileTags

        CancellationTokenSource TagGenerationCancellationSource;

        public async Task<IEnumerable<GeneratedTag>> GenerateTagsAsync(IFile file)
        {
            CancelTagGeneration();

            TagGenerationCancellationSource = new CancellationTokenSource();

            try
            {
                return await file.GenerateSuggestedTagsAsync(TagGenerationCancellationSource.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancelled tag generation for {file.Descriptor.Path}.");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }

        public void CancelTagGeneration()
        {
            if (TagGenerationCancellationSource != null)
            {
                TagGenerationCancellationSource.Cancel();
                TagGenerationCancellationSource.Dispose();
            }

            TagGenerationCancellationSource = null;
        }

        #endregion
    }
}

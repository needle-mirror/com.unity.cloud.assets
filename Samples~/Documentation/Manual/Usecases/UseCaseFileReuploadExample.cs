using LogProgress = Unity.Cloud.Documentation.Assets.BaseAssetBehaviour.LogProgress;

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

    public class UseCaseFileReuploadExampleUI : IAssetManagementUI
    {
        readonly BaseAssetBehaviour m_Behaviour;

        public UseCaseFileReuploadExampleUI(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseFileReuploadExample : IAssetManagementUI
    {
        readonly UseCaseFileReuploadExampleBehaviour m_Behaviour;

        public UseCaseFileReuploadExample(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = new UseCaseFileReuploadExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;

        Dictionary<DatasetId, bool> m_Expanded = new();

        public void OnGUI()
        {
            if (m_Behaviour.CurrentAsset == null) return;

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                m_Behaviour.DatasetFiles.Clear();
                _ = m_Behaviour.GetDatasets();
            }

            if (m_Behaviour.Datasets == null)
            {
                GUILayout.Label("Loading datasets...");
                return;
            }

            GUILayout.BeginVertical();

            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = m_Behaviour.GetDatasets();
            }

            GUILayout.Space(5f);

            DisplayDatasets(m_Behaviour.Datasets.ToArray());

            GUILayout.EndVertical();
        }

        void DisplayDatasets(IReadOnlyCollection<IDataset> datasets)
        {
            if (datasets.Count == 0)
            {
                GUILayout.Label("No datasets.");
            }

            foreach (var dataset in datasets)
            {
                var datasetId = dataset.Descriptor.DatasetId;

                GUILayout.BeginHorizontal();

                GUILayout.Label(m_Behaviour.GetDatasetName(datasetId));

                var expanded = m_Expanded.GetValueOrDefault(datasetId);
                if (GUILayout.Button(expanded ? "-" : "+", GUILayout.Width(20f)))
                {
                    expanded = !expanded;
                    m_Expanded[datasetId] = expanded;

                    if (!expanded)
                    {
                        m_Behaviour.DatasetFiles.Remove(datasetId);
                    }
                }

                GUILayout.EndHorizontal();

                if (expanded)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(25);

                    DisplayFiles(datasetId);

                    GUILayout.EndHorizontal();
                }
            }
        }

        void DisplayFiles(DatasetId datasetId)
        {
            if (!m_Behaviour.DatasetFiles.ContainsKey(datasetId))
            {
                _ = m_Behaviour.GetFilesAsync(datasetId);
            }

            var files = m_Behaviour.DatasetFiles.GetValueOrDefault(datasetId);

            if (files == null)
            {
                GUILayout.Label("Loading files...");
                return;
            }

            var enumerable = files.ToArray();
            if (!enumerable.Any())
            {
                GUILayout.Label("No files.");
                return;
            }

            GUILayout.BeginVertical();

            foreach (var file in enumerable)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label($"{file.Descriptor.Path}");

                GUILayout.Space(5f);

                if (GUILayout.Button("Upload new content", GUILayout.Width(150)))
                {
#if UNITY_EDITOR
                    var path = UnityEditor.EditorUtility.OpenFilePanel("Choose a file to upload.", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        var memoryStream = new MemoryStream(File.ReadAllBytes(path));
                        _ = m_Behaviour.ReplaceFileAsync(file, memoryStream);
                    }
#else
                    Debug.Log("Feature only supported in Editor.");
#endif
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseFileReuploadExampleBehaviour : UseCaseCreateFileExampleBehaviour
    {
        public UseCaseFileReuploadExampleBehaviour(BaseAssetBehaviour behaviour)
            : base(behaviour) { }

        #region Example_Behaviour_UploadFile

        CancellationTokenSource m_FileUploadCancellationSource;

        public async Task ReplaceFileAsync(IFile file, MemoryStream memoryStream)
        {
            await file.UploadAsync(memoryStream, new LogProgress(file.Descriptor.Path), GetCancellationToken());
        }

        CancellationToken GetCancellationToken()
        {
            if (m_FileUploadCancellationSource != null)
            {
                m_FileUploadCancellationSource.Cancel();
                m_FileUploadCancellationSource.Dispose();
            }

            m_FileUploadCancellationSource = new CancellationTokenSource();
            return m_FileUploadCancellationSource.Token;
        }

        #endregion
    }
}
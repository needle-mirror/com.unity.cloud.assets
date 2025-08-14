namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;
    using UnityEngine;

    public class UseCaseManageDatasetExampleUI : IAssetManagementUI
    {
        readonly BaseAssetBehaviour m_Behaviour;
        readonly string[] m_AssetTypeList;

        public UseCaseManageDatasetExampleUI(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = behaviour;
            m_AssetTypeList = AssetTypeExtensions.AssetTypeList().ToArray();
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseManageDatasetExample : IAssetManagementUI
    {
        readonly UseCaseManageDatasetExampleBehaviour m_Behaviour;
        readonly string[] m_AssetTypeList;

        public UseCaseManageDatasetExample(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = new UseCaseManageDatasetExampleBehaviour(behaviour);
            m_AssetTypeList = AssetTypeExtensions.AssetTypeList().ToArray();
        }

        #region Example_UIContent

        IAsset m_CurrentAsset;
        Vector2 m_ListScrollPosition;
        DatasetUpdate m_DatasetUpdate;
        string m_TagsString = string.Empty;

        public void OnGUI()
        {
            if (m_Behaviour.CurrentAsset == null) return;

            if (m_CurrentAsset != m_Behaviour.CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                _ = m_Behaviour.GetDatasets();
            }

            GUILayout.BeginVertical();

            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = m_Behaviour.GetDatasets();
            }

            DisplayDatasets(m_Behaviour.Datasets.ToArray());

            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(250));

            DisplayDataset();

            GUILayout.EndVertical();
        }

        void DisplayDatasets(IReadOnlyCollection<IDataset> datasets)
        {
            if (datasets.Count == 0)
            {
                GUILayout.Label("! No datasets !");
                return;
            }

            m_ListScrollPosition = GUILayout.BeginScrollView(m_ListScrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width(250));

            foreach (var dataset in datasets)
            {
                if (!m_Behaviour.DatasetProperties.TryGetValue(dataset.Descriptor.DatasetId, out var properties))
                {
                    GUILayout.Label(dataset.Descriptor.DatasetId.ToString());
                    continue;
                }

                GUILayout.BeginHorizontal();

                GUILayout.Label($"{properties.Name}", GUILayout.Width(150));

                GUI.enabled = dataset.Descriptor.DatasetId != m_Behaviour.CurrentDatasetId;

                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    m_DatasetUpdate = null;
                    m_Behaviour.CurrentDatasetId = dataset.Descriptor.DatasetId;
                    m_TagsString = string.Join(',', properties.Tags);
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        void DisplayDataset()
        {
            if (m_Behaviour.CurrentDatasetId == null) return;

            if (!m_Behaviour.DatasetProperties.TryGetValue(m_Behaviour.CurrentDatasetId.Value, out var properties))
            {
                GUILayout.Label("Loading properties...");
                return;
            }
            
            GUILayout.Label("Name");
            var name = GUILayout.TextField(m_DatasetUpdate?.Name ?? properties.Name);
            if (name != properties.Name)
            {
                m_DatasetUpdate ??= new DatasetUpdate();
                m_DatasetUpdate.Name = name;
            }

            GUILayout.Label("Type");
            var typeIndex = m_DatasetUpdate?.Type != null ? (int) m_DatasetUpdate.Type.Value : (int) properties.Type;
            typeIndex = GUILayout.SelectionGrid(typeIndex, m_AssetTypeList, 3);
            if (typeIndex != -1 && properties.Type != (AssetType) typeIndex)
            {
                m_DatasetUpdate ??= new DatasetUpdate();
                m_DatasetUpdate.Type = (AssetType) typeIndex;
            }

            GUILayout.Label("Description");
            var description = GUILayout.TextArea(m_DatasetUpdate?.Description ?? properties.Description);
            if (description != properties.Description)
            {
                m_DatasetUpdate ??= new DatasetUpdate();
                m_DatasetUpdate.Description = description;
            }

            var isVisible = GUILayout.Toggle(m_DatasetUpdate?.IsVisible ?? properties.IsVisible, "Is visible");
            if (isVisible != properties.IsVisible)
            {
                m_DatasetUpdate ??= new DatasetUpdate();
                m_DatasetUpdate.IsVisible = isVisible;
            }

            GUILayout.Label("Tags (comma separated)");
            var tags = GUILayout.TextArea(m_TagsString);
            if (tags != m_TagsString)
            {
                m_TagsString = tags;
                m_DatasetUpdate ??= new DatasetUpdate();
                m_DatasetUpdate.Tags = m_TagsString.Split(',')
                    .Select(tag => tag.Trim())
                    .Where(tag => !string.IsNullOrEmpty(tag))
                    .ToList();
            }

            GUILayout.Label($"System tags: {string.Join(", ", properties.SystemTags)}");
            GUILayout.Label($"Workflow: {string.Join(", ", properties.WorkflowName)}");

            GUILayout.Space(15f);

            GUI.enabled = m_DatasetUpdate != null;
            
            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateDataset(m_DatasetUpdate);
            }
            
            GUI.enabled = true;
        }

        #endregion
    }

    class UseCaseManageDatasetExampleBehaviour : UseCaseCreateDatasetExampleBehaviour
    {
        public UseCaseManageDatasetExampleBehaviour(BaseAssetBehaviour behaviour)
            : base(behaviour) { }

        #region Example_Behaviour_UpdateDataset

        public async Task UpdateDataset(IDatasetUpdate update)
        {
            if (CurrentDatasetId == null) return;

            try
            {
                var datasetId = CurrentDatasetId.Value;

                var dataset = Datasets.FirstOrDefault(x => x.Descriptor.DatasetId == CurrentDatasetId)
                    ?? await CurrentAsset.GetDatasetAsync(datasetId, CancellationToken.None);

                await dataset.UpdateAsync(update, CancellationToken.None);
                await dataset.RefreshAsync(CancellationToken.None);

                DatasetProperties[datasetId] = await dataset.GetPropertiesAsync(CancellationToken.None);

                Debug.Log($"Dataset update succeeded.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to update dataset. {e}");
                throw;
            }
        }

        #endregion
    }
}

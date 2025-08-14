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
    using UnityEngine;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;

    public class UseCaseManageAssetExampleUI : IAssetManagementUI
    {
        readonly BaseAssetBehaviour m_Behaviour;
        readonly string[] m_AssetTypeList;

        public UseCaseManageAssetExampleUI(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = behaviour;
            m_AssetTypeList = AssetTypeExtensions.AssetTypeList().ToArray();
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseManageAssetExample : IAssetManagementUI
    {
        readonly UseCaseManageAssetExampleBehaviour m_Behaviour;
        readonly string[] m_AssetTypeList;

        public UseCaseManageAssetExample(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = new UseCaseManageAssetExampleBehaviour(behaviour);
            m_AssetTypeList = AssetTypeExtensions.AssetTypeList().ToArray();
        }

        #region Example_UIContent

        static GUILayoutOption s_LabelWidth = GUILayout.Width(60);

        IAsset m_CurrentAsset;
        AssetUpdate m_AssetUpdate;
        string m_TagsString = string.Empty;

        public void OnGUI()
        {
            if (m_Behaviour.CurrentAsset == null) return;

            if (!m_Behaviour.TryGetAssetProperties(m_Behaviour.CurrentAsset.Descriptor.AssetVersion, out var properties))
            {
                GUILayout.Label(" ! Asset properties not loaded !");
                return;
            }

            if (!m_Behaviour.CurrentAsset.Equals(m_CurrentAsset))
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                m_AssetUpdate = new AssetUpdate
                {
                    Name = properties.Name,
                    Tags = properties.Tags?.ToList() ?? new List<string>(),
                    PreviewFile = properties.PreviewFileDescriptor?.Path ?? "",
                    Description = properties.Description,
                };
                m_TagsString = string.Join(',', m_AssetUpdate.Tags);
            }

            GUILayout.BeginVertical();

            if (m_AssetUpdate == null)
            {
                GUILayout.Label("Loading...");
            }
            else
            {
                GUI.enabled = properties.State == AssetState.Unfrozen;
                DisplayAsset(properties);
                GUI.enabled = true;
            }

            GUILayout.EndVertical();
        }

        void DisplayAsset(AssetProperties assetProperties)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Name:", s_LabelWidth);

            m_AssetUpdate.Name = GUILayout.TextField(m_AssetUpdate.Name);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Type:", s_LabelWidth);

            var typeIndex = m_AssetUpdate.Type.HasValue ? (int) m_AssetUpdate.Type.Value : (int) assetProperties.Type;
            typeIndex = GUILayout.SelectionGrid(typeIndex, m_AssetTypeList, 3, GUILayout.ExpandWidth(true));
            if (typeIndex != -1 && assetProperties.Type != (AssetType) typeIndex)
            {
                m_AssetUpdate.Type = (AssetType) typeIndex;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Tags:", s_LabelWidth);
            m_TagsString = GUILayout.TextField(m_TagsString);
            m_AssetUpdate.Tags = m_TagsString.Split(',')
                .Select(tag => tag.Trim())
                .Where(tag => !string.IsNullOrEmpty(tag))
                .ToList();

            GUILayout.EndHorizontal();

            if (assetProperties.PreviewFileDescriptor.HasValue)
            {
                GUILayout.Label("Preview DatasetId: " + assetProperties.PreviewFileDescriptor.Value.DatasetId);
            }
            else
            {
                GUILayout.Label("No preview file.");
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label("Preview:", s_LabelWidth);

            m_AssetUpdate.PreviewFile = GUILayout.TextField(m_AssetUpdate.PreviewFile);

            GUILayout.EndHorizontal();

            GUILayout.Label("Description:");
            m_AssetUpdate.Description = GUILayout.TextArea(m_AssetUpdate.Description);

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateAssetAsync(m_AssetUpdate);
            }
        }

        #endregion
    }

    class UseCaseManageAssetExampleBehaviour
    {
        readonly BaseAssetBehaviour m_Behaviour;

        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;
        public bool TryGetAssetProperties(AssetVersion assetVersion, out AssetProperties properties) => m_Behaviour.TryGetAssetProperties(assetVersion, out properties);

        public UseCaseManageAssetExampleBehaviour(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_UpdateAsset

        public async Task UpdateAssetAsync(IAssetUpdate assetUpdate)
        {
            try
            {
                await CurrentAsset.UpdateAsync(assetUpdate, CancellationToken.None);
                
                // Update properties:
                await CurrentAsset.RefreshAsync(CancellationToken.None);
                var properties = await CurrentAsset.GetPropertiesAsync(CancellationToken.None);
                m_Behaviour.IncludeProperties(CurrentAsset.Descriptor, properties);
            }
            catch (OperationCanceledException oe)
            {
                Debug.Log(oe);
            }
            catch (AggregateException e)
            {
                Debug.LogError(e.InnerException);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        #endregion

        async Task ClearDescription()
        {
            #region Example_Behaviour_ClearDescription

            var assetUpdate = new AssetUpdate
            {
                Description = ""
            };

            await CurrentAsset.UpdateAsync(assetUpdate, CancellationToken.None);

            #endregion
        }

        async Task ClearTags()
        {
            #region Example_Behaviour_ClearTags

            var assetUpdate = new AssetUpdate
            {
                Tags = new List<string>()
            };

            await CurrentAsset.UpdateAsync(assetUpdate, CancellationToken.None);

            #endregion
        }

        async Task ClearPreviewFile()
        {
            #region Example_Behaviour_ClearPreviewFile

            var assetUpdate = new AssetUpdate
            {
                PreviewFile = ""
            };

            await CurrentAsset.UpdateAsync(assetUpdate, CancellationToken.None);

            #endregion
        }
    }
}

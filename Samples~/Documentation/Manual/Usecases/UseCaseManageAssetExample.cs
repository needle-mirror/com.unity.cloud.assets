using System.Collections.Generic;

namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;
    using Unity.Cloud.Assets;

    public class UseCaseManageAssetExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;
        readonly string[] m_AssetTypeList;

        public UseCaseManageAssetExampleUI(AssetManagementBehaviour behaviour)
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

        public UseCaseManageAssetExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseManageAssetExampleBehaviour(behaviour);
            m_AssetTypeList = AssetTypeExtensions.AssetTypeList().ToArray();
        }

        #region Example_UIContent

        static GUILayoutOption s_LabelWidth = GUILayout.Width(60);

        IAsset m_CurrentAsset;
        AssetUpdate m_AssetUpdate;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            GUILayout.Space(15f);

            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label(" ! No asset selected !");
                return;
            }

            GUILayout.BeginVertical();

            if (m_Behaviour.CurrentAsset != m_CurrentAsset)
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                _ = RefreshAssetAsync();
            }

            GUILayout.Label("Asset selected:");
            GUILayout.Space(5f);

            if (m_AssetUpdate == null)
            {
                GUILayout.Label("Loading...");
            }
            else
            {
                GUI.enabled = !m_CurrentAsset.IsFrozen;
                DisplayAsset(m_AssetUpdate);
                GUI.enabled = true;
            }

            GUILayout.EndVertical();
        }

        async Task RefreshAssetAsync()
        {
            m_AssetUpdate = null;
            await m_CurrentAsset.RefreshAsync(CancellationToken.None);
            m_AssetUpdate = new AssetUpdate(m_CurrentAsset);
        }

        void DisplayAsset(AssetUpdate assetUpdate)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Name:", s_LabelWidth);

            assetUpdate.Name = GUILayout.TextField(assetUpdate.Name, GUILayout.ExpandWidth(true));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Type:", s_LabelWidth);

            var type = assetUpdate.Type.HasValue ? (int) assetUpdate.Type.Value : -1;
            type = GUILayout.SelectionGrid(type, m_AssetTypeList, 4);
            if (type != -1)
                assetUpdate.Type = (AssetType) type;

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Tags:", s_LabelWidth);

            var tags = string.Join(',', assetUpdate.Tags);
            tags = GUILayout.TextField(tags, GUILayout.ExpandWidth(true));
            assetUpdate.Tags = tags.Split(',').ToList();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Preview:", s_LabelWidth);

            assetUpdate.PreviewFile = GUILayout.TextField(assetUpdate.PreviewFile, GUILayout.ExpandWidth(true));

            GUILayout.EndHorizontal();

            GUILayout.Label("Description:");
            assetUpdate.Description = GUILayout.TextArea(assetUpdate.Description, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateAssetAsync(assetUpdate);
            }
        }

        #endregion
    }

    class UseCaseManageAssetExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseManageAssetExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_UpdateAsset

        public async Task UpdateAssetAsync(IAssetUpdate assetUpdate)
        {
            try
            {
                await CurrentAsset.UpdateAsync(assetUpdate, CancellationToken.None);
                await CurrentAsset.RefreshAsync(CancellationToken.None);
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

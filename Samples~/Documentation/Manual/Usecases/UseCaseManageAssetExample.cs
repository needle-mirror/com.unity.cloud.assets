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
        string m_TagsString = null;

        public void OnGUI()
        {
            if (m_Behaviour.CurrentAsset == null) return;

            if (!m_Behaviour.CurrentAsset.Equals(m_CurrentAsset))
            {
                m_CurrentAsset = m_Behaviour.CurrentAsset;
                m_AssetUpdate = null;
                m_TagsString = null;
            }

            GUILayout.BeginVertical();

            if (!m_Behaviour.TryGetAssetProperties(m_Behaviour.CurrentAsset.Descriptor.AssetVersion, out var properties))
            {
                GUILayout.Label("Loading... ");
                GUILayout.EndVertical();
                return;
            }

            DisplayAsset(properties);

            GUILayout.EndVertical();
        }

        void DisplayAsset(AssetProperties assetProperties)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Name:", s_LabelWidth);

            var name = GUILayout.TextField(m_AssetUpdate?.Name ?? assetProperties.Name);
            if (name != assetProperties.Name)
            {
                m_AssetUpdate ??= new AssetUpdate();
                m_AssetUpdate.Name = name;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Type:", s_LabelWidth);

            var typeIndex = m_AssetUpdate?.Type != null ? (int) m_AssetUpdate.Type.Value : (int) assetProperties.Type;
            typeIndex = GUILayout.SelectionGrid(typeIndex, m_AssetTypeList, 3, GUILayout.ExpandWidth(true));
            if (typeIndex != -1 && assetProperties.Type != (AssetType) typeIndex)
            {
                m_AssetUpdate ??= new AssetUpdate();
                m_AssetUpdate.Type = (AssetType) typeIndex;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Tags:", s_LabelWidth);

            if (m_TagsString == null)
            {
                m_TagsString = string.Join(',', m_AssetUpdate?.Tags ?? assetProperties.Tags ?? new List<string>());
            }
            
            var tagsString = GUILayout.TextField(m_TagsString);
            if (tagsString != m_TagsString)
            {
                m_TagsString = tagsString;

                m_AssetUpdate ??= new AssetUpdate();
                m_AssetUpdate.Tags = m_TagsString.Split(',')
                    .Select(tag => tag.Trim())
                    .Where(tag => !string.IsNullOrEmpty(tag))
                    .ToList();
            }

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

            var originalPreviewFile = m_AssetUpdate?.PreviewFile ?? assetProperties.PreviewFileDescriptor?.Path ?? string.Empty;
            var previewFile = GUILayout.TextField(originalPreviewFile);
            if (previewFile != originalPreviewFile)
            {
                m_AssetUpdate ??= new AssetUpdate();
                m_AssetUpdate.PreviewFile = previewFile;
            }

            GUILayout.EndHorizontal();

            GUILayout.Label("Description:");
            var description = GUILayout.TextArea(m_AssetUpdate?.Description ?? assetProperties.Description);
            if (description != assetProperties.Description)
            {
                m_AssetUpdate ??= new AssetUpdate();
                m_AssetUpdate.Description = description;
            }

            GUI.enabled = m_AssetUpdate != null;

            if (GUILayout.Button("Update"))
            {
                _ = m_Behaviour.UpdateAssetAsync(m_AssetUpdate);
            }

            GUI.enabled = true;
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

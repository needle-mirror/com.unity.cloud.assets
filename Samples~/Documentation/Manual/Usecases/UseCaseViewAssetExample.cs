namespace Unity.Cloud.Documentation.Assets
{
    #region Example

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;

    public class UseCaseViewAssetExampleUI : IAssetManagementUI
    {
        readonly BaseAssetBehaviour m_Behaviour;

        public UseCaseViewAssetExampleUI(BaseAssetBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI()
        {
            if (m_Behaviour.CurrentAsset == null) return;

            GUILayout.Space(15f);

            if (!m_Behaviour.TryGetAssetProperties(m_Behaviour.CurrentAsset.Descriptor.AssetVersion, out var properties))
            {
                GUILayout.Label(" ! Asset properties not loaded !");
                return;
            }

            GUILayout.BeginVertical();

            DisplayAsset(m_Behaviour.CurrentAsset.Descriptor, properties);

            GUILayout.EndVertical();
        }

        static void DisplayAsset(AssetDescriptor descriptor, AssetProperties assetProperties)
        {
            GUILayout.Label("Id: " + descriptor.AssetId);
            GUILayout.Label("Version: " + descriptor.AssetVersion);

            GUILayout.Label("Asset properties:");

            GUILayout.Space(5f);
            GUILayout.Label($"Name: {assetProperties.Name}");
            GUILayout.Label($"Type: {assetProperties.Type}");
            GUILayout.Label($"Tags: {assetProperties.Tags?.ToList() ?? new List<string>()}");

            GUILayout.Space(5f);
            GUILayout.Label("Sequence Number: " + assetProperties.FrozenSequenceNumber);
            GUILayout.Label("Parent Sequence Number: " + assetProperties.ParentFrozenSequenceNumber);

            GUILayout.Space(5f);
            if (assetProperties.PreviewFileDescriptor.HasValue)
            {
                GUILayout.Label("Preview Dataset Id: " + assetProperties.PreviewFileDescriptor.Value.DatasetId);
                GUILayout.Label($"Preview File Path: {assetProperties.PreviewFileDescriptor.Value.Path}");
            }
            else
            {
                GUILayout.Label("No preview file.");
            }

            GUILayout.Space(5f);
            GUILayout.Label($"Description: {assetProperties.Description}");
        }
    }

    #endregion
}

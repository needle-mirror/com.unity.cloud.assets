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
    using Unity.Cloud.Common;

    public class UseCaseViewAssetLibraryJobExampleUI : IAssetManagementUI
    {
        readonly AssetLibrariesBehaviour m_Behaviour;

        public UseCaseViewAssetLibraryJobExampleUI(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseViewAssetLibraryJobExample : IAssetManagementUI
    {
        readonly AssetLibrariesBehaviour m_Behaviour;

        public UseCaseViewAssetLibraryJobExample(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_UIContent

        public void OnGUI()
        {
            if (m_Behaviour.CurrentAssetLibraryJob == null)
            {
                GUILayout.Label("No Asset Library Job selected.");
                return;
            }

            GUILayout.Space(15f);

            if (!m_Behaviour.TryGetAssetLibraryJobProperties(m_Behaviour.CurrentAssetLibraryJob.Id, out var properties))
            {
                GUILayout.Label(" ! Asset library job properties not loaded !");
                return;
            }

            GUILayout.BeginVertical();

            DisplayJob(m_Behaviour.CurrentAssetLibraryJob.Id, properties);

            GUILayout.EndVertical();
        }

        void DisplayJob(AssetLibraryJobId id, AssetLibraryJobProperties properties)
        {
            GUILayout.Label($"Asset Library Job ID: {id}");

            GUILayout.Space(15f);

            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = RefreshAssetLibraryJob(id);
            }

            GUILayout.Space(15f);

            GUILayout.Label($"Name: {properties.Name}");
            GUILayout.Label($"State: {properties.State}");

            GUILayout.Label($"Progress: {properties.Progress}%");
            if (!string.IsNullOrEmpty(properties.ProgressDetails))
            {
                GUILayout.Label($"Details: {properties.ProgressDetails}");
            }

            if (!string.IsNullOrEmpty(properties.FailedReason))
            {
                GUILayout.Label($"Failed Reason: {properties.FailedReason}");
            }

            if (properties.State != AssetLibraryJobState.Failed)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label($"Results: {properties.CopiedAssetDescriptor?.AssetId.ToString() ?? "Pending..."}");

                GUI.enabled = properties.CopiedAssetDescriptor != null;

                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    System.Diagnostics.Debug.Assert(properties.CopiedAssetDescriptor != null, "properties.CopiedAssetDescriptor != null");
                    _ = SetCurrentAsset(properties.CopiedAssetDescriptor.Value);
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();
            }
        }

        async Task RefreshAssetLibraryJob(AssetLibraryJobId id)
        {
            await m_Behaviour.CurrentAssetLibraryJob.RefreshAsync(CancellationToken.None);
            var properties = await m_Behaviour.CurrentAssetLibraryJob.GetPropertiesAsync(CancellationToken.None);
            m_Behaviour.IncludeProperties(id, properties);
        }

        async Task SetCurrentAsset(AssetDescriptor assetDescriptor)
        {
            var asset = m_Behaviour.AvailableAssets.FirstOrDefault(a => a.Descriptor == assetDescriptor);

            if (asset == null)
            {
                asset = await PlatformServices.AssetRepository.GetAssetAsync(assetDescriptor, CancellationToken.None);
                m_Behaviour.AvailableAssets.Add(asset);

                var properties = await asset.GetPropertiesAsync(CancellationToken.None);
                m_Behaviour.IncludeProperties(assetDescriptor, properties);
            }

            m_Behaviour.CurrentAsset = asset;
        }

        #endregion
    }
}

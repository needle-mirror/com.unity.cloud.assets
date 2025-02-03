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

    public class UseCaseVersionSearchExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseVersionSearchExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseVersionSearchExample : IAssetManagementUI
    {
        readonly UseCaseVersionSearchExampleBehaviour m_Behaviour;

        public UseCaseVersionSearchExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseVersionSearchExampleBehaviour(behaviour);
        }

        protected IAsset CurrentAsset => m_Behaviour.CurrentAsset;
        protected AssetVersion? CurrentVersion => m_Behaviour.CurrentVersion;

        #region Example_UIContent

        string m_SortingField = "versionNumber";
        SortingOrder m_SortingOrder = SortingOrder.Descending;
        AssetId m_CurrentAssetId;

        public void OnGUI()
        {
            if (m_Behaviour.CurrentAsset == null) return;

            if (m_Behaviour.CurrentAsset.Descriptor.AssetId != m_CurrentAssetId)
            {
                m_CurrentAssetId = m_Behaviour.CurrentAsset.Descriptor.AssetId;
                SearchAssetVersions();
            }

            GUILayout.Space(15f);

            GUILayout.BeginVertical();

            GUILayout.Label("Sorting Field:");
            m_SortingField = GUILayout.TextField(m_SortingField);

            GUILayout.Label("Sorting Order:");
            m_SortingOrder = (SortingOrder) GUILayout.SelectionGrid((int) m_SortingOrder, new[] {"Ascending", "Descending"}, 2);

            if (GUILayout.Button("Search"))
            {
                SearchAssetVersions();
            }

            GUILayout.Space(15f);

            GUILayout.Label("Versions: ");

            if (m_Behaviour.VersionProperties.Count == 0)
            {
                GUILayout.Label("Loading...");
            }
            else
            {
                foreach (var kvp in m_Behaviour.VersionProperties)
                {
                    DisplayVersion(kvp.Key, kvp.Value);
                }
            }

            GUILayout.EndVertical();

            DisplayCurrentVersion();
        }

        void SearchAssetVersions()
        {
            if (string.IsNullOrEmpty(m_SortingField)) return;

            _ = m_Behaviour.SearchVersions(m_SortingField, m_SortingOrder);
        }

        void DisplayVersion(AssetVersion version, AssetProperties properties)
        {
            var versionStr = properties.State switch
            {
                AssetState.Frozen => $"Ver. {properties.FrozenSequenceNumber}",
                AssetState.Unfrozen => $"WIP from Ver. {properties.ParentFrozenSequenceNumber}",
                AssetState.PendingFreeze => "Pending",
                _ => ""
            };

            var labels = properties.Labels.Select(x => x.LabelName).ToArray();
            if (labels.Length > 0)
            {
                versionStr += $" ({string.Join(", ", labels)})";
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label(versionStr, GUILayout.ExpandWidth(true));

            GUI.enabled = m_Behaviour.CurrentVersion != version;

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                m_Behaviour.CurrentVersion = version;
            }

            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }

        void DisplayCurrentVersion()
        {
            if (m_Behaviour.CurrentVersion == null)
            {
                GUILayout.Label("! No version selected. !");
                return;
            }

            var version = m_Behaviour.CurrentVersion.Value;

            if (!m_Behaviour.VersionProperties.TryGetValue(version, out var properties))
            {
                GUILayout.Label("! Version properties not loaded. !");
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Label($"Version: {version}");
            if (properties.ParentFrozenSequenceNumber > 0)
            {
                GUILayout.Label($"Parent Sequence Number: {properties.ParentFrozenSequenceNumber}");
            }

            GUILayout.Label($"State: {properties.State}");

            if (properties.State == AssetState.Unfrozen)
            {
                if (GUILayout.Button("Freeze version"))
                {
                    _ = m_Behaviour.FreezeVersion(version);
                }
            }
            else
            {
                GUILayout.Label($"Frozen Sequence Number: {properties.FrozenSequenceNumber}");

                if (GUILayout.Button("Create new version"))
                {
                    _ = m_Behaviour.CreateVersion(version);
                }
            }

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseVersionSearchExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public IAssetProject CurrentProject => m_Behaviour.CurrentProject;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseVersionSearchExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_SearchVersions

        public Dictionary<AssetVersion, AssetProperties> VersionProperties { get; } = new();

        public AssetVersion? CurrentVersion { get; set; }

        VersionQueryBuilder m_CurrentQuery;

        public async Task SearchVersions(string sortingField, SortingOrder sortingOrder)
        {
            m_CurrentQuery = CurrentProject.QueryAssetVersions(CurrentAsset.Descriptor.AssetId)
                .OrderBy(sortingField, sortingOrder);

            await PopulateVersions(m_CurrentQuery);
        }

        async Task PopulateVersions(VersionQueryBuilder query)
        {
            if (query == null) return;

            var results = query.ExecuteAsync(CancellationToken.None);

            var currentVersion = CurrentVersion;
            CurrentVersion = null;

            VersionProperties.Clear();
            await foreach (var asset in results)
            {
                var properties = await asset.GetPropertiesAsync(CancellationToken.None);
                VersionProperties.Add(asset.Descriptor.AssetVersion, properties);

                if (currentVersion.HasValue && asset.Descriptor.AssetVersion == currentVersion.Value)
                {
                    CurrentVersion = asset.Descriptor.AssetVersion;
                }
            }
        }

        #endregion

        #region Example_FreezeVersion

        public async Task FreezeVersion(AssetVersion assetVersion)
        {
            var asset = await CurrentAsset.WithVersionAsync(assetVersion, CancellationToken.None);

            await asset.FreezeAsync(new AssetFreeze
            {
                ChangeLog = "Use case coding example submission.",
                Operation = AssetFreezeOperation.CancelTransformations
            }, CancellationToken.None);
            await asset.RefreshAsync(CancellationToken.None);

            var properties = await asset.GetPropertiesAsync(CancellationToken.None);
            Debug.Log($"Version frozen with sequence number: {properties.FrozenSequenceNumber}");

            await PopulateVersions(m_CurrentQuery);
        }

        #endregion

        #region Example_CreateVersion

        public async Task CreateVersion(AssetVersion assetVersion)
        {
            var version = await CurrentAsset.WithVersionAsync(assetVersion, CancellationToken.None);
            var asset = await version.CreateUnfrozenVersionAsync(CancellationToken.None);
            await PopulateVersions(m_CurrentQuery);

            Debug.Log($"New version created with version: {asset.Descriptor.AssetVersion}");
        }

        #endregion
    }
}

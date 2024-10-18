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

        protected IAsset CurrentVersion => m_CurrentVersion;

        #region Example_UIContent

        string m_SortingField = "versionNumber";
        SortingOrder m_SortingOrder = SortingOrder.Descending;
        AssetId m_CurrentAssetId;
        IAsset m_CurrentVersion;

        public void OnGUI()
        {
            if (m_Behaviour.CurrentAsset == null) return;

            if (m_Behaviour.CurrentAsset.Descriptor.AssetId != m_CurrentAssetId)
            {
                m_CurrentAssetId = m_Behaviour.CurrentAsset.Descriptor.AssetId;
                m_CurrentVersion = null;
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

            if (m_Behaviour.AssetVersions == null)
            {
                GUILayout.Label("Loading...");
            }
            else if (m_Behaviour.AssetVersions.Count == 0)
            {
                GUILayout.Label("No versions found.");
            }
            else
            {
                foreach (var asset in m_Behaviour.AssetVersions)
                {
                    DisplayVersion(asset);
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

        void DisplayVersion(IAsset asset)
        {
            var version = asset.State switch
            {
                AssetState.Frozen => $"Ver. {asset.FrozenSequenceNumber}",
                AssetState.Unfrozen => $"WIP from Ver. {asset.ParentFrozenSequenceNumber}",
                AssetState.PendingFreeze => "Pending",
                _ => ""
            };

            var labels = asset.Labels.Select(x => x.LabelName).ToArray();
            if (labels.Length > 0)
            {
                version += $" ({string.Join(", ", labels)})";
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label(version, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                m_CurrentVersion = asset;
            }

            GUILayout.EndHorizontal();
        }

        void DisplayCurrentVersion()
        {
            if (m_CurrentVersion == null)
            {
                GUILayout.Label("! No version selected. !");
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Label($"Version: {m_CurrentVersion.Descriptor.AssetVersion}");
            if (m_CurrentVersion.ParentFrozenSequenceNumber > 0)
            {
                GUILayout.Label($"Parent Sequence Number: {m_CurrentVersion.ParentFrozenSequenceNumber}");
            }

            GUILayout.Label($"State: {m_CurrentVersion.State}");

            if (m_CurrentVersion.State == AssetState.Unfrozen)
            {
                if (GUILayout.Button("Freeze version"))
                {
                    _ = m_Behaviour.FreezeVersion(m_CurrentVersion);
                }
            }
            else
            {
                GUILayout.Label($"Frozen Sequence Number: {m_CurrentVersion.FrozenSequenceNumber}");

                if (GUILayout.Button("Create new version"))
                {
                    _ = m_Behaviour.CreateVersion(m_CurrentVersion);
                }
            }

            GUILayout.EndVertical();
        }

        #endregion
    }

    class UseCaseVersionSearchExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseVersionSearchExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_SearchVersions

        public List<IAsset> AssetVersions { get; private set; }

        VersionQueryBuilder m_CurrentQuery;

        public async Task SearchVersions(string sortingField, SortingOrder sortingOrder)
        {
            m_CurrentQuery = CurrentAsset.QueryVersions()
                .OrderBy(sortingField, sortingOrder);

            await PopulateVersions(m_CurrentQuery);
        }

        async Task PopulateVersions(VersionQueryBuilder query)
        {
            if (query == null) return;

            var results = query.ExecuteAsync(CancellationToken.None);

            AssetVersions = new List<IAsset>();
            await foreach (var asset in results)
            {
                AssetVersions ??= new List<IAsset>();
                AssetVersions.Add(asset);
            }
        }

        #endregion

        #region Example_FreezeVersion

        public async Task FreezeVersion(IAsset asset)
        {
            await asset.FreezeAsync(new AssetFreeze
            {
                ChangeLog = "Use case coding example submission.",
                Operation = AssetFreezeOperation.CancelTransformations
            }, CancellationToken.None);

            // Refresh all versions
            var tasks = AssetVersions.Select(version => version.RefreshAsync(CancellationToken.None)).ToList();
            await Task.WhenAll(tasks);

            Debug.Log($"Version frozen with sequence number: {asset.FrozenSequenceNumber}");
        }

        #endregion

        #region Example_CreateVersion

        public async Task CreateVersion(IAsset asset)
        {
            var version = await asset.CreateUnfrozenVersionAsync(CancellationToken.None);
            await PopulateVersions(m_CurrentQuery);

            Debug.Log($"New version created with version: {version.Descriptor.AssetVersion}");
        }

        #endregion
    }
}

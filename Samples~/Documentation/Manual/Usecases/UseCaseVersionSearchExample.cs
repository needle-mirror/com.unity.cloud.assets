using Unity.Cloud.Common;

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

        #region Example_UIContent

        AssetId m_CurrentAssetId;
        string m_SortingField = "versionNumber";
        SortingOrder m_SortingOrder = SortingOrder.Descending;

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
            m_SortingOrder = (SortingOrder) GUILayout.SelectionGrid((int) m_SortingOrder, new[] { "Ascending", "Descending" }, 2);

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
                    DisplayVersionInfo(asset);
                }
            }

            GUILayout.EndVertical();
        }

        void SearchAssetVersions()
        {
            if (string.IsNullOrEmpty(m_SortingField)) return;

            _ = m_Behaviour.SearchAssetVersions(m_SortingField, m_SortingOrder);
        }

        static void DisplayVersionInfo(IAsset asset)
        {
            var version = asset.IsFrozen ? $"Ver. {asset.FrozenSequenceNumber}" : $"WIP from Ver. {asset.ParentFrozenSequenceNumber}";

            var labels = asset.Labels.Select(x => x.LabelName).ToArray();
            if (labels.Length > 0)
            {
                version += $" ({string.Join(", ", labels)})";
            }

            GUILayout.Label(version);
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

        #region Example_Behaviour

        public List<IAsset> AssetVersions { get; private set; }

        public async Task SearchAssetVersions(string sortingField, SortingOrder sortingOrder)
        {
            AssetVersions = null;

            var results = CurrentAsset.QueryAssetVersions()
                .OrderBy(sortingField, sortingOrder)
                .ExecuteAsync(CancellationToken.None);

            AssetVersions = new List<IAsset>();
            await foreach (var asset in results)
            {
                AssetVersions ??= new List<IAsset>();
                AssetVersions.Add(asset);
            }
        }

        #endregion
    }
}

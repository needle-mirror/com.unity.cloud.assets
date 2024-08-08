namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;

    public class UseCaseAssetReferenceManagementExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseAssetReferenceManagementExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseAssetReferenceManagementExample : IAssetManagementUI
    {
        readonly UseCaseAssetReferenceManagementExampleBehaviour m_Behaviour;

        public UseCaseAssetReferenceManagementExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseAssetReferenceManagementExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        enum SelectionMode
        {
            Source,
            Target,
        }

        SelectionMode m_SelectionMode;
        Vector2 m_SourceScrollPosition;
        Vector2 m_TargetScrollPosition;

        IAsset m_SourceAsset;
        string m_SourceVersion;
        string m_SourceLabel;

        IAsset m_TargetAsset;
        string m_TargetVersion;
        string m_TargetLabel;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected) return;

            GUILayout.BeginVertical();

            m_SelectionMode = (SelectionMode) GUILayout.SelectionGrid((int) m_SelectionMode, new[] {"Source", "Target"}, 2);

            TrySetSourceAndTarget();

            if (GUILayout.Button("Create Reference"))
            {
                _ = m_Behaviour.CreateReference(m_SourceAsset, m_TargetAsset.Descriptor.AssetId, m_TargetVersion, m_TargetLabel);
            }

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            DisplayAssetReference(m_SourceAsset, ref m_SourceVersion, ref m_SourceLabel, SelectionMode.Source);
            DisplayAssetReference(m_TargetAsset, ref m_TargetVersion, ref m_TargetLabel, SelectionMode.Target);

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        void DisplayAssetReference(IAsset asset, ref string version, ref string label, SelectionMode selectionMode)
        {
            if (asset == null)
            {
                GUILayout.Label("No asset selected");
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Label(selectionMode.ToString());

            GUILayout.Space(5);

            GUILayout.Label(asset.Name);
            GUILayout.Label(asset.Descriptor.AssetId.ToString());

            version = GUILayout.TextField(version);
            label = GUILayout.TextField(label);

            if (GUILayout.Button("Refresh"))
            {
                _ = m_Behaviour.ListReferences(asset.Descriptor.AssetId, version, selectionMode.ToString());
            }

            DisplayReferences(selectionMode, asset);

            GUILayout.EndVertical();
        }

        void DisplayReferences(SelectionMode selectionMode, IAsset asset)
        {
            GUILayout.Label("References:");
            if (m_Behaviour.References.TryGetValue(selectionMode.ToString(), out var references))
            {
                switch (selectionMode)
                {
                    case SelectionMode.Source:
                        m_SourceScrollPosition = GUILayout.BeginScrollView(m_SourceScrollPosition);
                        break;
                    case SelectionMode.Target:
                        m_TargetScrollPosition = GUILayout.BeginScrollView(m_TargetScrollPosition);
                        break;
                }

                foreach (var reference in references)
                {
                    GUILayout.Space(5);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"Ref. {reference.ReferenceId}");
                    if (GUILayout.Button("Remove"))
                    {
                        _ = m_Behaviour.RemoveReference(asset, reference.ReferenceId);
                    }
                    GUILayout.EndHorizontal();

                    var isSource = reference.SourceAssetId == asset.Descriptor.AssetId;
                    string assetVersion;
                    string referencedId;
                    string referencedVersion;
                    if (isSource)
                    {
                        assetVersion = reference.SourceAssetVersion.ToString();
                        referencedId = reference.TargetAssetId.ToString();
                        referencedVersion = reference.TargetLabel ?? reference.TargetAssetVersion.ToString();
                    }
                    else
                    {
                        assetVersion = reference.TargetLabel ?? reference.TargetAssetVersion.ToString();
                        referencedId = reference.SourceAssetId.ToString();
                        referencedVersion = reference.SourceAssetVersion.ToString();
                    }

                    GUILayout.Label($"    Ver. {assetVersion}");
                    GUILayout.Label($"     {(isSource ? "Depends on" : "Referenced by")}");
                    GUILayout.Label($"    - id: {referencedId}");
                    GUILayout.Label($"    - ver: {referencedVersion}");
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.Label("Loading...");
            }
        }

        void TrySetSourceAndTarget()
        {
            if (m_Behaviour.CurrentAsset == null) return;

            switch (m_SelectionMode)
            {
                case SelectionMode.Source:
                    if (m_SourceAsset?.Descriptor.AssetId != m_Behaviour.CurrentAsset.Descriptor.AssetId)
                    {
                        m_SourceAsset = m_Behaviour.CurrentAsset;
                        m_SourceVersion = m_SourceAsset.Descriptor.AssetVersion.ToString();
                        _ = m_Behaviour.ListReferences(m_SourceAsset.Descriptor.AssetId, m_SourceVersion, SelectionMode.Source.ToString());
                    }

                    break;

                case SelectionMode.Target:
                    if (m_TargetAsset?.Descriptor.AssetId != m_Behaviour.CurrentAsset.Descriptor.AssetId)
                    {
                        m_TargetAsset = m_Behaviour.CurrentAsset;
                        m_TargetVersion = m_TargetAsset.Descriptor.AssetVersion.ToString();
                        _ = m_Behaviour.ListReferences(m_TargetAsset.Descriptor.AssetId, m_TargetVersion, SelectionMode.Target.ToString());
                    }

                    break;
            }
        }

        #endregion
    }

    class UseCaseAssetReferenceManagementExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;
        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        public IAssetProject CurrentProject => m_Behaviour.CurrentProject;

        public UseCaseAssetReferenceManagementExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_ListReferences

        public Dictionary<string, List<IAssetReference>> References { get; } = new();

        public async Task ListReferences(AssetId assetId, string version, string id)
        {
            References.Remove(id);

            var filter = new AssetReferenceSearchFilter();
            if (!string.IsNullOrEmpty(version))
            {
                filter.AssetVersion.WhereEquals(new AssetVersion(version));
            }

            try
            {
                var references = CurrentProject.QueryAssetReferences(assetId)
                    .SelectWhereMatchesFilter(filter)
                    .ExecuteAsync(default);

                var referencesList = new List<IAssetReference>();
                References.Add(id, referencesList);
                await foreach (var reference in references)
                {
                    referencesList.Add(reference);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public async Task CreateReference(IAsset asset, AssetId referencedAssetId, string version, string label)
        {
            IAssetReference assetReference = null;

            try
            {
                if (!string.IsNullOrEmpty(version))
                {
                    assetReference = await asset.AddReferenceAsync(referencedAssetId, new AssetVersion(version), default);
                }
                else
                {
                    assetReference = await asset.AddReferenceAsync(referencedAssetId, label, default);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            if (assetReference != null)
            {
                Debug.Log($"Reference created: {assetReference.ReferenceId}");
            }
        }

        public async Task RemoveReference(IAsset asset, string referenceId)
        {
            try
            {
                await asset.RemoveReferenceAsync(referenceId, default);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        #endregion
    }
}

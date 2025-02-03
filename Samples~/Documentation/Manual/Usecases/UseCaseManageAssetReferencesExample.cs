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

    public class UseCaseManageAssetReferencesExampleUI : IAssetManagementUI
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public UseCaseManageAssetReferencesExampleUI(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseManageAssetReferencesExample : IAssetManagementUI
    {
        readonly UseCaseManageAssetReferencesExampleBehaviour m_Behaviour;

        public UseCaseManageAssetReferencesExample(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = new UseCaseManageAssetReferencesExampleBehaviour(behaviour);
        }

        public bool IsTargetMode => m_SelectionMode == SelectionMode.Target;

        public AssetVersion SelectedVersion
        {
            get
            {
                return m_SelectionMode switch
                {
                    SelectionMode.Source => new AssetVersion(m_SourceVersion),
                    SelectionMode.Target => new AssetVersion(m_TargetVersion),
                    _ => default
                };
            }
        }
        public string TargetLabel
        {
            get => m_TargetLabel;
            set => m_TargetLabel = value;
        }

        #region Example_UIContent

        enum SelectionMode
        {
            Source,
            Target,
        }

        IAsset m_SelectedAsset;

        SelectionMode m_SelectionMode;
        Vector2 m_SourceScrollPosition;
        Vector2 m_TargetScrollPosition;

        IAsset m_SourceAsset;
        string m_SourceVersion;

        IAsset m_TargetAsset;
        string m_TargetVersion;
        string m_TargetLabel;

        public void OnGUI()
        {
            if (!m_Behaviour.IsProjectSelected)
            {
                m_SelectedAsset = null;
                m_SourceAsset = null;
                m_TargetAsset = null;
                return;
            }

            if (m_SelectedAsset != m_Behaviour.CurrentAsset)
            {
                m_SelectedAsset = m_Behaviour.CurrentAsset;

                OnCurrentAssetChanged();
            }

            GUILayout.BeginVertical();

            var selectionMode = (SelectionMode) GUILayout.SelectionGrid((int) m_SelectionMode, new[] {"Source", "Target"}, 2);
            if (m_SelectionMode != selectionMode)
            {
                m_SelectionMode = selectionMode;
                m_Behaviour.CurrentAsset = m_SelectionMode switch
                {
                    SelectionMode.Source => m_SourceAsset,
                    SelectionMode.Target => m_TargetAsset,
                    _ => m_Behaviour.CurrentAsset
                };
            }

            GUI.enabled = m_SourceAsset != null && m_TargetAsset != null;

            if (GUILayout.Button("Create Reference"))
            {
                if (m_SourceAsset != null && m_TargetAsset != null)
                {
                    _ = m_Behaviour.CreateReferenceAsync(m_SourceAsset,
                        m_TargetAsset.Descriptor.AssetId, m_TargetVersion, m_TargetLabel);
                }
            }

            GUI.enabled = true;

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            DisplayAssetReference(m_SourceAsset, ref m_SourceVersion, SelectionMode.Source);
            DisplayAssetReference(m_TargetAsset, ref m_TargetVersion, SelectionMode.Target, DisplayTargetLabelSelection);

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        void DisplayAssetReference(IAsset asset, ref string version, SelectionMode selectionMode, Action labelSelection = null)
        {
            if (asset == null)
            {
                GUILayout.Label("No asset selected");
                return;
            }

            if (!m_Behaviour.AssetProperties.TryGetValue(asset.Descriptor.AssetId, out var properties))
            {
                GUILayout.Label("Asset properties not loaded");
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Label($"{selectionMode} - {properties.Name}");

            GUILayout.Space(5);

            GUILayout.Label("Id: " + asset.Descriptor.AssetId);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ver.", GUILayout.Width(40));
            version = GUILayout.TextField(version);
            GUILayout.EndHorizontal();

            labelSelection?.Invoke();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Refresh Ver.", GUILayout.Width(100)))
            {
                _ = m_Behaviour.ListReferencesAsync(asset.Descriptor.AssetId, version, selectionMode.ToString());
            }

            if (GUILayout.Button("Show All", GUILayout.Width(100)))
            {
                _ = m_Behaviour.ListReferencesAsync(asset.Descriptor.AssetId, string.Empty, selectionMode.ToString());
            }

            GUILayout.EndHorizontal();

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
                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        _ = m_Behaviour.RemoveReferenceAsync(asset, reference.ReferenceId);
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

        void DisplayTargetLabelSelection()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Label", GUILayout.Width(40));
            m_TargetLabel = GUILayout.TextField(m_TargetLabel);
            GUILayout.EndHorizontal();
        }

        void OnCurrentAssetChanged()
        {
            switch (m_SelectionMode)
            {
                case SelectionMode.Source:
                    if (m_Behaviour.CurrentAsset == null)
                    {
                        m_SourceAsset = null;
                        m_SourceVersion = string.Empty;
                    }
                    else
                    {
                        m_SourceAsset = m_Behaviour.CurrentAsset;
                        m_SourceVersion = m_SourceAsset.Descriptor.AssetVersion.ToString();
                        _ = m_Behaviour.ListReferencesAsync(m_SourceAsset.Descriptor.AssetId, m_SourceVersion, SelectionMode.Source.ToString());
                    }

                    break;

                case SelectionMode.Target:
                    if (m_Behaviour.CurrentAsset == null)
                    {
                        m_TargetAsset = null;
                        m_TargetVersion = string.Empty;
                        m_TargetLabel = string.Empty;
                    }
                    else
                    {
                        m_TargetAsset = m_Behaviour.CurrentAsset;
                        m_TargetVersion = string.IsNullOrEmpty(m_TargetLabel) ? m_TargetAsset.Descriptor.AssetVersion.ToString() : string.Empty;
                        _ = m_Behaviour.ListReferencesAsync(m_TargetAsset.Descriptor.AssetId, m_TargetVersion, SelectionMode.Target.ToString());
                    }

                    break;
            }
        }

        #endregion
    }

    class UseCaseManageAssetReferencesExampleBehaviour
    {
        readonly AssetManagementBehaviour m_Behaviour;

        public bool IsProjectSelected => m_Behaviour.IsProjectSelected;
        IAssetProject CurrentProject => m_Behaviour.CurrentProject;

        public IAsset CurrentAsset
        {
            get => m_Behaviour.CurrentAsset;
            set => m_Behaviour.CurrentAsset = value;
        }

        public Dictionary<AssetId, AssetProperties> AssetProperties => m_Behaviour.AssetProperties;

        public UseCaseManageAssetReferencesExampleBehaviour(AssetManagementBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_ListReferences

        public Dictionary<string, List<IAssetReference>> References { get; } = new();

        public async Task ListReferencesAsync(AssetId assetId, string version, string id)
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

        #endregion

        #region Example_Behaviour_CreateReference

        public async Task CreateReferenceAsync(IAsset asset, AssetId referencedAssetId, string version, string label)
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
                References["Source"].Add(assetReference);
                References["Target"].Add(assetReference);
                Debug.Log($"Reference created: {assetReference.ReferenceId}");
            }
        }

        #endregion

        #region Example_Behaviour_RemoveReference

        public async Task RemoveReferenceAsync(IAsset asset, string referenceId)
        {
            try
            {
                await asset.RemoveReferenceAsync(referenceId, default);
                References["Source"].RemoveAll(reference => reference.ReferenceId == referenceId);
                References["Target"].RemoveAll(reference => reference.ReferenceId == referenceId);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        #endregion
    }
}

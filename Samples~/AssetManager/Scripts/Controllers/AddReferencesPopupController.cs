using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class AddReferencesPopupController : PopupController
    {
        readonly DropdownField m_AssetSelection;
        readonly DropdownField m_VersionSelection;

        readonly List<IAsset> m_Assets = new();
        readonly List<AssetVersion> m_Versions = new();
        readonly List<string> m_Labels = new();

        Func<AssetId, AssetVersion?, string, Task> m_AddReference;

        CancellationTokenSource m_FetchTokenSource;

        public AddReferencesPopupController(VisualElement root)
            : base(root, "AddReferencePopup")
        {
            m_ActionButton.SetEnabled(false);

            m_AssetSelection = root.Q<DropdownField>("AssetSelection");
            m_AssetSelection.RegisterCallback<ChangeEvent<string>>(OnAssetSelected);
            m_AssetSelection.SetEnabled(false);

            m_VersionSelection = root.Q<DropdownField>("VersionSelection");
            m_VersionSelection.RegisterCallback<ChangeEvent<string>>(OnVersionSelected);
            m_VersionSelection.SetEnabled(false);
        }

        public void Show(ProjectDescriptor projectDescriptor, Func<AssetId, AssetVersion?, string, Task> addReference)
        {
            m_ActionButton.SetEnabled(false);
            m_AssetSelection.SetEnabled(false);
            m_VersionSelection.SetEnabled(false);

            m_AddReference = addReference;

            Show();

            _ = ListAssetsAsync(projectDescriptor);
        }

        async Task ListAssetsAsync(ProjectDescriptor projectDescriptor)
        {
            if (m_FetchTokenSource != null)
            {
                m_FetchTokenSource.Cancel();
                m_FetchTokenSource.Dispose();
            }

            m_FetchTokenSource = new CancellationTokenSource();

            m_Assets.Clear();
            m_Versions.Clear();
            m_Labels.Clear();

            m_AssetSelection.SetValueWithoutNotify(string.Empty);
            m_VersionSelection.SetValueWithoutNotify(string.Empty);

            var enumerable = PlatformServices.AssetRepository.QueryAssets(new[] {projectDescriptor})
                .ExecuteAsync(m_FetchTokenSource.Token);
            await foreach (var asset in enumerable)
            {
                m_Assets.Add(asset);
            }

            m_AssetSelection.choices = m_Assets.Select(x => $"{x.Name} ({x.Descriptor.AssetId})").ToList();
            m_AssetSelection.SetEnabled(true);
        }

        async Task ListAssetVersionsAndLabelsAsync(IAsset asset)
        {
            var cancellationToken = m_FetchTokenSource.Token;

            m_ActionButton.SetEnabled(false);
            m_VersionSelection.SetEnabled(false);

            m_Labels.Clear();
            m_Versions.Clear();

            m_VersionSelection.SetValueWithoutNotify(string.Empty);

            var labelQuery = asset.QueryLabels().ExecuteAsync(cancellationToken);
            await foreach (var tuple in labelQuery)
            {
                foreach (var label in tuple.Item2)
                {
                    m_Labels.Add(label);
                }
            }

            var combinedChoices = new List<string>(m_Labels);

            var versionQuery = asset.QueryVersions().ExecuteAsync(cancellationToken);
            await foreach (var version in versionQuery)
            {
                m_Versions.Add(version.Descriptor.AssetVersion);

                var versionString = version.State switch
                {
                    AssetState.Frozen => $"Ver. {version.FrozenSequenceNumber}",
                    AssetState.PendingFreeze => $"Pending",
                    AssetState.Unfrozen => version.ParentFrozenSequenceNumber > 0 ? $"WIP of Ver.{version.ParentFrozenSequenceNumber}" : "Ver. 1 - Pending",
                    _ => "Ver. 1 - Pending"
                };

                combinedChoices.Add(versionString);
            }

            m_VersionSelection.choices = combinedChoices;
            m_VersionSelection.SetEnabled(true);
        }

        void OnAssetSelected(ChangeEvent<string> evt)
        {
            var assetIndex = m_AssetSelection.index;
            if (assetIndex >= 0 && assetIndex < m_Assets.Count)
            {
                var asset = m_Assets[assetIndex];
                _ = ListAssetVersionsAndLabelsAsync(asset);
            }
        }

        void OnVersionSelected(ChangeEvent<string> evt)
        {
            var versionIndex = m_VersionSelection.index;
            if (versionIndex >= 0 && versionIndex < m_Versions.Count + m_Labels.Count)
            {
                m_ActionButton.SetEnabled(true);
            }
        }

        protected override void OnClicked()
        {
            base.OnClicked();

            var assetIndex = m_AssetSelection.index;
            if (assetIndex >= 0 && assetIndex < m_Assets.Count)
            {
                var asset = m_Assets[assetIndex];

                AssetVersion? version = null;
                string label = null;

                var versionIndex = m_VersionSelection.index;
                if (versionIndex >= 0 && versionIndex < m_Labels.Count)
                {
                    label = m_Labels[versionIndex];
                }
                else
                {
                    version = m_Versions[versionIndex - m_Labels.Count];
                }

                m_AddReference?.Invoke(asset.Descriptor.AssetId, version, label);
                m_AddReference = null;
            }
        }
    }
}

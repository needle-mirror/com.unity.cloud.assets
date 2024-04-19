using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    class VersionLabelEntity : IVersionLabel
    {
        readonly IAssetDataSource m_AssetDataSource;

        /// <inheritdoc/>
        public VersionLabelDescriptor Descriptor { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <inheritdoc/>
        public bool IsSystemLabel { get; set; }

        /// <inheritdoc/>
        public bool IsAssignable { get; set; }

        /// <inheritdoc/>
        public Color DisplayColor { get; set; }

        /// <inheritdoc/>
        public AuthoringInfo AuthoringInfo { get; set; }

        public VersionLabelEntity(IAssetDataSource assetDataSource, VersionLabelDescriptor descriptor)
        {
            m_AssetDataSource = assetDataSource;
            Descriptor = descriptor;
        }

        /// <inheritdoc/>
        public async Task RefreshAsync(CancellationToken cancellationToken)
        {
            var data = await m_AssetDataSource.GetVersionLabelAsync(Descriptor, cancellationToken);
            if (data != null)
                this.MapFrom(data);
        }

        /// <inheritdoc/>
        public Task UpdateAsync(IVersionLabelUpdate versionLabelUpdate, CancellationToken cancellationToken)
        {
            return m_AssetDataSource.UpdateVersionLabelAsync(Descriptor, versionLabelUpdate.From(), cancellationToken);
        }

        /// <inheritdoc/>
        public async Task RenameAsync(string labelName, CancellationToken cancellationToken)
        {
            var versionLabelUpdate = new VersionLabelBaseData {Name = labelName};
            await m_AssetDataSource.UpdateVersionLabelAsync(Descriptor, versionLabelUpdate, cancellationToken);

            // On success, the descriptor must be modified immediately.
            Descriptor = new VersionLabelDescriptor(Descriptor.OrganizationId, versionLabelUpdate.Name);
        }

        /// <inheritdoc/>
        public Task ArchiveAsync(CancellationToken cancellationToken)
        {
            return m_AssetDataSource.UpdateVersionLabelStatusAsync(Descriptor, true, cancellationToken);
        }

        /// <inheritdoc/>
        public Task UnarchiveAsync(CancellationToken cancellationToken)
        {
            return m_AssetDataSource.UpdateVersionLabelStatusAsync(Descriptor, false, cancellationToken);
        }
    }
}

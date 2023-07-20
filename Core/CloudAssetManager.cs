using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class that provides access and management of cloud assets.
    /// </summary>
    public sealed class CloudAssetManager : CloudAssetProvider, IAssetManager
    {
        /// <summary>
        /// Initializes and returns an instance of <see cref="CloudAssetManager"/>
        /// </summary>
        /// <param name="serviceHttpClient"> The <see cref="IServiceHttpClient"/> used to fetch the data. </param>
        /// <param name="serviceHostResolver"> The <see cref="IServiceHostResolver"/> object. </param>
        public CloudAssetManager(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
            : base(serviceHttpClient, serviceHostResolver) { }

        internal CloudAssetManager(IAssetDataSource dataSource)
            : base(dataSource) { }

        /// <inheritdoc />
        public Task<IAsset> CreateAssetAsync(IAssetCreation assetCreation, CancellationToken token)
        {
            return m_DataSource.CreateAssetAsync(assetCreation.Organization, assetCreation.Project, assetCreation, token);
        }

        /// <inheritdoc />
        public Task UpdateAssetAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.UpdateAssetAsync(asset.Organization, asset.Project, asset, token);
        }

        /// <inheritdoc />
        public Task DeleteAssetAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.DeleteAssetAsync(asset.Organization, asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task GetAssetDownloadUrlsAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.GetAssetDownloadUrlsAsync(asset.Organization, asset.Project, asset, token);
        }

        /// <inheritdoc />
        public Task GetAssetCollectionsAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.GetAssetCollectionsAsync(asset.Organization, asset.Project, asset, token);
        }

        /// <inheritdoc />
        public Task LinkAnAssetToProjectAsync(IAsset asset, ulong destinationOrganizationId, string destinationProjectId, CancellationToken token)
        {
            return m_DataSource.LinkAnAssetToProjectAsync(asset.Organization, asset.Project, asset.Id, asset.Version, destinationOrganizationId, destinationProjectId, token);
        }

        /// <inheritdoc />
        public Task UnlinkAssetFromProjectAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.UnlinkAssetFromProjectAsync(asset.Organization, asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task<bool> CheckProjectIsAssetSourceProjectAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.CheckProjectIsAssetSourceProjectAsync(asset.Organization, asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task PublishApprovedAssetAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.PublishApprovedAssetAsync(asset.Organization, asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task WithdrawPublishedAssetAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.WithdrawPublishedAssetAsync(asset.Organization, asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task SendAssetToReviewAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.SendAssetToReviewAsync(asset.Organization, asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task ApproveAssetAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.ApproveAssetAsync(asset.Organization, asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task RejectAssetAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.RejectAssetAsync(asset.Organization, asset.Project, asset.Id, asset.Version, token);
        }
    }
}

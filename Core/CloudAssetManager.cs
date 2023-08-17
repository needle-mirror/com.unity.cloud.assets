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
        /// <param name="assetServiceConfiguration"> The asset service configuration object. </param>
        public CloudAssetManager(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver, AssetServiceConfiguration assetServiceConfiguration)
            : base(serviceHttpClient, serviceHostResolver, assetServiceConfiguration) { }

        internal CloudAssetManager(IAssetDataSource dataSource)
            : base(dataSource) { }

        /// <inheritdoc />
        public Task<IAsset> CreateAssetAsync(IAssetCreation assetCreation, CancellationToken token)
        {
            return m_DataSource.CreateAssetAsync(assetCreation.Project, assetCreation, token);
        }

        /// <inheritdoc />
        public Task UpdateAssetAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.UpdateAssetAsync(asset.Project, asset, token);
        }

        /// <inheritdoc />
        public Task DeleteAssetAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.DeleteAssetAsync(asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task GetAssetDownloadUrlsAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.GetAssetDownloadUrlsAsync(asset.Project, asset, token);
        }

        /// <inheritdoc />
        public Task GetAssetCollectionsAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.GetAssetCollectionsAsync(asset.Project, asset, token);
        }

        /// <inheritdoc />
        public Task LinkAnAssetToProjectAsync(IAsset asset, ulong destinationOrganizationId, string destinationProjectId, CancellationToken token)
        {
            return m_DataSource.LinkAnAssetToProjectAsync(asset.Project, asset.Id, asset.Version, destinationOrganizationId, destinationProjectId, token);
        }

        /// <inheritdoc />
        public Task UnlinkAssetFromProjectAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.UnlinkAssetFromProjectAsync(asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task<bool> CheckProjectIsAssetSourceProjectAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.CheckProjectIsAssetSourceProjectAsync(asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task PublishApprovedAssetAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.PublishApprovedAssetAsync(asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task WithdrawPublishedAssetAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.WithdrawPublishedAssetAsync(asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task SendAssetToReviewAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.SendAssetToReviewAsync(asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task ApproveAssetAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.ApproveAssetAsync(asset.Project, asset.Id, asset.Version, token);
        }

        /// <inheritdoc />
        public Task RejectAssetAsync(IAsset asset, CancellationToken token)
        {
            return m_DataSource.RejectAssetAsync(asset.Project, asset.Id, asset.Version, token);
        }
    }
}

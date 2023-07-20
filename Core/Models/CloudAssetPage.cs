using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    sealed class CloudAssetPage : AssetPage
    {
        readonly IAssetDataSource m_AssetDataSource;

        internal CloudAssetPage(IAssetDataSource assetDataSource, IAsset[] assets, string nextPageToken, IAssetPage previousPage)
            : base(assets, nextPageToken, previousPage)
        {
            m_AssetDataSource = assetDataSource;
        }

        internal CloudAssetPage(IAssetDataSource assetDataSource, IOrganization organization, IProject project, IAsset[] assets, string nextPageToken, Pagination pagination)
            : base(organization, project, assets, nextPageToken, null, -1)
        {
            m_AssetDataSource = assetDataSource;

            Pagination = pagination;
        }

        /// <inheritdoc/>
        public override Task<IAssetPage> GetNextAsync<TAsset>(CancellationToken token)
        {
            return m_AssetDataSource.GetNextAssetPageAsync<TAsset>(this, token);
        }
    }
}

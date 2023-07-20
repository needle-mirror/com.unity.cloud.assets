using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    public abstract class AssetPage : PagedResponse<IAsset>, IAssetPage
    {
        readonly IAssetDataSource m_AssetDataSource;

        /// <inheritdoc/>
        public IOrganization Organization { get; }

        /// <inheritdoc/>
        public IProject Project { get; }

        protected AssetPage(IAsset[] assets, string nextPageToken, IAssetPage previousPage)
            : base(assets, nextPageToken, previousPage, (previousPage?.PageEndIndex ?? -1) + assets.Length)
        {
            Organization = previousPage?.Organization;
            Project = previousPage?.Project;
        }

        protected AssetPage(IOrganization organization, IProject project, IAsset[] assets, string nextPageToken, IPagedResponse<IAsset> previousPage = null, int pageEndIndex = -1)
            : base(assets, nextPageToken, previousPage, pageEndIndex)
        {
            Organization = organization;
            Project = project;
        }

        /// <inheritdoc/>
        public abstract Task<IAssetPage> GetNextAsync<TAsset>(CancellationToken token) where TAsset : IAsset, new();
    }
}

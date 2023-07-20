using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class that provides cloud assets.
    /// <remarks>Users of this class will require a minimum <c>Asset Manager Contributor</c> role.</remarks>
    /// </summary>
    public class CloudAssetProvider : IAssetProvider
    {
        private protected readonly IAssetDataSource m_DataSource;

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <summary>
        /// Initializes and returns an instance of <see cref="CloudAssetProvider"/>
        /// </summary>
        /// <param name="serviceHttpClient"> The <see cref="IServiceHttpClient"/> used to fetch the data. </param>
        /// <param name="serviceHostResolver"> The <see cref="IServiceHostResolver"/> object. </param>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetProviderExample.cs" region="ConstructAssetProvider"/>
        /// </example>
        public CloudAssetProvider(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
            : this(serviceHttpClient, ServiceHostConfigurationFactory.Create(serviceHostResolver))
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CloudAssetProvider"/> class.
        /// </summary>
        /// <param name="serviceHttpClient"> The <see cref="IServiceHttpClient"/> used to fetch the data. </param>
        /// <param name="serviceHostConfiguration"> The configuration object. </param>
        CloudAssetProvider(IServiceHttpClient serviceHttpClient, AssetHostConfiguration serviceHostConfiguration)
            : this(AssetDataSourceFactory.CreateManagementDataSource(serviceHttpClient, serviceHostConfiguration))
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CloudAssetProvider"/> class.
        /// </summary>
        /// <param name="dataSource"></param>
        internal CloudAssetProvider(IAssetDataSource dataSource)
        {
            m_DataSource = dataSource;
            Name = GetType().Name;
        }

        /// <inheritdoc/>
        public async Task<IAsset> GetAssetAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            return await m_DataSource.GetAssetAsync<Asset>(organization, project, assetId, assetVersion, token);
        }

        /// <inheritdoc/>
        public Task<TAsset> GetAssetAsync<TAsset>(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
            where TAsset : IAsset, new()
        {
            return m_DataSource.GetAssetAsync<TAsset>(organization, project, assetId, assetVersion, token);
        }

        /// <inheritdoc/>
        public Task<IAssetPage> SearchAsync(IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken token)
        {
            return SearchAsync<Asset>(assetSearchFilter, pagination, token);
        }

        /// <inheritdoc/>
        public Task<IAssetPage> SearchAsync<TAsset>(IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken token)
            where TAsset : IAsset, new()
        {
            var organization = assetSearchFilter.GetOrganizationToSearch();
            var project = assetSearchFilter.GetProjectToSearch();
            return m_DataSource.GetAssetPageAsync<TAsset>(organization, project, assetSearchFilter, pagination, token);
        }

        /// <inheritdoc/>
        public Task<Aggregation> AggregateAsync(IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken token)
        {
            var organization = assetSearchFilter.GetOrganizationToSearch();
            var project = assetSearchFilter.GetProjectToSearch();
            return m_DataSource.GetAssetAggregateAsync(organization, project, assetSearchFilter, parameters, token);
        }
    }
}

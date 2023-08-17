using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class that provides access to a user's organizations.
    /// </summary>
    public sealed class CloudOrganizationProvider : IOrganizationProvider
    {
        readonly IUsersDataSource m_UsersDataSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudOrganizationProvider"/> class.
        /// </summary>
        /// <param name="serviceHttpClient"> The <see cref="IServiceHttpClient"/> used to fetch the data. </param>
        /// <param name="serviceHostResolver">The <see cref="IServiceHostResolver"/> object. </param>
        public CloudOrganizationProvider(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
            : this(serviceHttpClient, ServiceHostConfigurationFactory.Create(serviceHostResolver)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudOrganizationProvider"/> class.
        /// </summary>
        /// <param name="serviceHttpClient"> The <see cref="IServiceHttpClient"/> used to fetch the data. </param>
        /// <param name="serviceHostConfiguration"> The configuration object. </param>
        CloudOrganizationProvider(IServiceHttpClient serviceHttpClient, AssetHostConfiguration serviceHostConfiguration)
            : this(new UsersDataSource(serviceHttpClient, serviceHostConfiguration.GetServiceAddress())) { }

        internal CloudOrganizationProvider(IUsersDataSource dataSource)
        {
            m_UsersDataSource = dataSource;
        }

        /// <param name="token"></param>
        /// <inheritdoc/>
        public async Task<IOrganization[]> GetOrganizationsAsync(CancellationToken token)
        {
            var result = await m_UsersDataSource.GetUserOrganizationsAsync(null, token);
            return result.Item2;
        }
    }
}

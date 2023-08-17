using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Implement this interface to transform user facing data like <see cref="IProject"/> into service DTOs
    /// </summary>
    public sealed class CloudProjectProvider : IProjectProvider
    {
        readonly IProjectDataSource m_DataSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudProjectProvider"/> class.
        /// </summary>
        /// <param name="serviceHttpClient"> The <see cref="IServiceHttpClient"/> used to fetch the data. </param>
        /// <param name="serviceHostResolver"> The <see cref="IServiceHostResolver"/> object. </param>
        public CloudProjectProvider(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
            : this(serviceHttpClient, ServiceHostConfigurationFactory.Create(serviceHostResolver))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudProjectProvider"/> class.
        /// </summary>
        /// <param name="serviceHttpClient"> The <see cref="IServiceHttpClient"/> used to fetch the data. </param>
        /// <param name="serviceHostConfiguration"> The host configuration object. </param>
        CloudProjectProvider(IServiceHttpClient serviceHttpClient, AssetHostConfiguration serviceHostConfiguration)
            : this(new ProjectDataSource(serviceHttpClient, serviceHostConfiguration.GetServiceAddress()))
        {}

        internal CloudProjectProvider(IProjectDataSource dataSource)
        {
            m_DataSource = dataSource;
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IProject> ListProjectsAsync(IOrganization organization, Pagination pagination,
            CancellationToken token, bool enrichWithUsersCount = false, string xCorrelationId = null)
        {
            return m_DataSource.ListProjectsAsync(organization, null, pagination, token, enrichWithUsersCount, xCorrelationId);
        }
    }
}

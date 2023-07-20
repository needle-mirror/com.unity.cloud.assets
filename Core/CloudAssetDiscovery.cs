using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class that provides published cloud assets.
    /// <remarks>Users of this class will require a minimum <c>Asset Manager Viewer</c> role.</remarks>
    /// </summary>
    public class CloudAssetDiscovery : CloudAssetProvider
    {
        /// <summary>
        /// Initializes and returns an instance of <see cref="CloudAssetDiscovery"/>
        /// </summary>
        /// <param name="serviceHttpClient"> The <see cref="IServiceHttpClient"/> used to fetch the data. </param>
        /// <param name="serviceHostResolver"> The <see cref="IServiceHostResolver"/> object. </param>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetProviderExample.cs" region="ConstructAssetDiscovery"/>
        /// </example>
        public CloudAssetDiscovery(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
            : this(serviceHttpClient, ServiceHostConfigurationFactory.Create(serviceHostResolver))
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CloudAssetDiscovery"/> class.
        /// </summary>
        /// <param name="serviceHttpClient"> The <see cref="IServiceHttpClient"/> used to fetch the data. </param>
        /// <param name="serviceHostConfiguration"> The configuration object. </param>
        CloudAssetDiscovery(IServiceHttpClient serviceHttpClient, AssetHostConfiguration serviceHostConfiguration)
            : this(AssetDataSourceFactory.CreateDiscoveryDataSource(serviceHttpClient, serviceHostConfiguration)) { }

        /// <summary>
        /// Creates a new instance of the <see cref="CloudAssetDiscovery"/> class.
        /// </summary>
        /// <param name="dataSource"></param>
        internal CloudAssetDiscovery(IAssetDataSource dataSource)
            : base(dataSource) { }
    }
}

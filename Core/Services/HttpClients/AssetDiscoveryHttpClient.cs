using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An HTTP client in the Assets SDK providing asset discovering.
    /// </summary>
    class AssetDiscoveryHttpClient : AssetHttpClient
    {
        /// <summary>
        /// Asset Discovery Http Client Object.
        /// Enables calls to asset discovery endpoints.
        /// </summary>
        /// <param name="serviceHttpClient">An <see cref="IServiceHttpClient"/> instance. </param>
        /// <param name="serviceUrl">The url of the service. </param>
        public AssetDiscoveryHttpClient(IServiceHttpClient serviceHttpClient, string serviceUrl) : base(serviceHttpClient, serviceUrl)
        {}

        /// <inheritdoc />
        protected override string GetApiPath()
        {
            return $"/api/assets/discovery/{apiVersion}";
        }
    }
}

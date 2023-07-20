using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Base class for calling common <see cref="ApiRequest"/>.
    /// </summary>
    class AssetHttpClient : IAssetHttpClient
    {
        readonly IServiceHttpClient m_ServiceHttpClient;
        readonly Uri m_ServiceUrl;

        private protected const string apiVersion = "v1beta1";

        /// <summary>
        /// Asset Http Client Object.
        /// Enables calls to asset endpoints.
        /// </summary>
        /// <param name="serviceHttpClient">An <see cref="IServiceHttpClient"/> instance. </param>
        /// <param name="serviceUrl">The url of the service. </param>
        internal AssetHttpClient(IServiceHttpClient serviceHttpClient, string serviceUrl)
        {
            m_ServiceHttpClient = serviceHttpClient.WithApiSourceHeadersFromAssembly(Assembly.GetExecutingAssembly());
            m_ServiceUrl = new Uri(serviceUrl);
        }

        /// <inheritdoc />
        public async Task<string> GetAsync(ApiRequest request, ServiceHttpClientOptions serviceHttpClientOptions, CancellationToken token)
        {
            var uri = new Uri(m_ServiceUrl, request.ConstructUrl(GetApiPath()));

            var response = await m_ServiceHttpClient.GetAsync(uri, serviceHttpClientOptions, cancellationToken: token);
            return await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc />
        public async Task<string> PostAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token)
        {
            var uri = new Uri(m_ServiceUrl, request.ConstructUrl(GetApiPath()));

            var response = await m_ServiceHttpClient.PostAsync(uri, request.ConstructBody(), options, cancellationToken: token);
            return await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc />
        public async Task<string> PutAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token)
        {
            var uri = new Uri(m_ServiceUrl, request.ConstructUrl(GetApiPath()));

            var response = await m_ServiceHttpClient.PutAsync(uri, request.ConstructBody(), options, cancellationToken: token);
            return await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc />
        public async Task<string> PatchAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token)
        {
            var uri = new Uri(m_ServiceUrl, request.ConstructUrl(GetApiPath()));

            var response = await m_ServiceHttpClient.PatchAsync(uri, request.ConstructBody(), options, cancellationToken: token);
            return await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc />
        public async Task<string> DeleteAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token)
        {
            var uri = new Uri(m_ServiceUrl, request.ConstructUrl(GetApiPath()));

            var response = await m_ServiceHttpClient.DeleteAsync(uri, request.ConstructBody(), options, cancellationToken: token);
            return await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ServiceHttpClientOptions options, CancellationToken token)
        {
            return m_ServiceHttpClient.SendAsync(request, options, cancellationToken: token);
        }

        /// <summary>
        /// The start path of the api.
        /// </summary>
        /// <returns>A partial url string. </returns>
        protected virtual string GetApiPath()
        {
            return $"/api/assets/{apiVersion}";
        }
    }
}

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
        public async Task<string> GetAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token)
        {
            var uri = new Uri(m_ServiceUrl, request.ConstructUrl(GetApiPath()));

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri
            };

            AddHeaders(httpRequestMessage, request);

            var response = await m_ServiceHttpClient.SendAsync(httpRequestMessage, options, token);
            return await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc />
        public async Task<string> PostAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token)
        {
            var uri = new Uri(m_ServiceUrl, request.ConstructUrl(GetApiPath()));
            var content = request.ConstructBody();

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = uri,
                Content = content
            };

            AddHeaders(httpRequestMessage, request);

            var response = await m_ServiceHttpClient.SendAsync(httpRequestMessage, options, token);
            return await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc />
        public async Task<string> PutAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token)
        {
            var uri = new Uri(m_ServiceUrl, request.ConstructUrl(GetApiPath()));
            var content = request.ConstructBody();

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = uri,
                Content = content
            };

            AddHeaders(httpRequestMessage, request);

            var response = await m_ServiceHttpClient.SendAsync(httpRequestMessage, options, token);
            return await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc />
        public async Task<string> PatchAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token)
        {
            var uri = new Uri(m_ServiceUrl, request.ConstructUrl(GetApiPath()));
            var content = request.ConstructBody();

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = uri,
                Content = content
            };

            AddHeaders(httpRequestMessage, request);

            var response = await m_ServiceHttpClient.SendAsync(httpRequestMessage, options, token);
            return await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc />
        public async Task<string> DeleteAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token)
        {
            var uri = new Uri(m_ServiceUrl, request.ConstructUrl(GetApiPath()));
            var content = request.ConstructBody();

            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = uri,
                Content = content
            };

            AddHeaders(httpRequestMessage, request);

            var response = await m_ServiceHttpClient.SendAsync(httpRequestMessage, options, token);
            return await response.Content.ReadAsStringAsync();
        }

        /// <inheritdoc />
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ServiceHttpClientOptions options, CancellationToken token)
        {
            return m_ServiceHttpClient.SendAsync(request, options, token);
        }

        /// <summary>
        /// The start path of the api.
        /// </summary>
        /// <returns>A partial url string. </returns>
        protected virtual string GetApiPath()
        {
            return $"/api/assets/{apiVersion}";
        }

        static void AddHeaders(HttpRequestMessage request, ApiRequest apiRequest)
        {
            foreach (var (key, value) in apiRequest.GetHeaders())
            {
                request.Headers.Add(key, value);
            }
        }
    }
}

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An interface for an HTTP client in the Assets SDK.
    /// </summary>
    interface IAssetHttpClient
    {
        /// <summary>
        /// Returns the result body of an Http GET request.
        /// </summary>
        /// <param name="request">An <see cref="ApiRequest"/>. </param>
        /// <param name="options">The service options for the request. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the request's response body. </returns>
        Task<string> GetAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token);

        /// <summary>
        /// Returns the result body of an Http POST request
        /// </summary>
        /// <param name="request">An <see cref="ApiRequest"/>. </param>
        /// <param name="options">The service options for the request. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the request's response body. </returns>
        Task<string> PostAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token);

        /// <summary>
        /// Returns the result body of an Http POST request
        /// </summary>
        /// <param name="request">An <see cref="ApiRequest"/>. </param>
        /// <param name="options">The service options for the request. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the request's response body. </returns>
        Task<string> PutAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token);

        /// <summary>
        /// Returns the result body of an Http PATCH request
        /// </summary>
        /// <param name="request">An <see cref="ApiRequest"/>. </param>
        /// <param name="options">The service options for the request. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the request's response body. </returns>
        Task<string> PatchAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token);

        /// <summary>
        /// Returns the result body of an Http DELETE request
        /// </summary>
        /// <param name="request">An <see cref="ApiRequest"/>. </param>
        /// <param name="options">The service options for the request. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the request's response body. </returns>
        Task<string> DeleteAsync(ApiRequest request, ServiceHttpClientOptions options, CancellationToken token);

        /// <summary>
        /// Sends an asynchronous HTTP request.
        /// </summary>
        /// <param name="request">The request to be sent.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="token">Cancellation token that will try to cancel the operation.</param>
        /// <returns>An <see cref="HttpResponseMessage"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the requestUri is invalid.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken token);

        /// <summary>
        /// Sends an asynchronous HTTP request.
        /// </summary>
        /// <param name="request">The request to be sent.</param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="token">Cancellation token that will try to cancel the operation.</param>
        /// <returns>A task that will hold the HttpResponseMessage once the request is completed</returns>
        /// <exception cref="ArgumentNullException">Thrown when the request is null.</exception>
        /// <exception cref="HttpRequestException">Thrown when an HTTP response can't be obtained from the server.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the request is cancelled by a cancellation token.</exception>
        /// <exception cref="TimeoutException">Thrown when the request failed due to timeout.</exception>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, IProgress<HttpProgress> progress, CancellationToken token);

        /// <summary>
        /// Send an asynchronous HTTP request.
        /// </summary>
        /// <param name="request">The request to be sent.</param>
        /// <param name="options">The service options for the request. </param>
        /// <param name="token">The cancellation token that will try to cancel the operation.</param>
        /// <returns>A task that will hold the HttpResponseMessage once the request is completed</returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ServiceHttpClientOptions options, CancellationToken token);

        /// <summary>
        /// Send an asynchronous HTTP request.
        /// </summary>
        /// <param name="request">The request to be sent.</param>
        /// <param name="options">The service options for the request. </param>
        /// <param name="progress">The progress provider.</param>
        /// <param name="token">The cancellation token that will try to cancel the operation.</param>
        /// <returns>A task that will hold the HttpResponseMessage once the request is completed</returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ServiceHttpClientOptions options, IProgress<HttpProgress> progress, CancellationToken token);
    }
}

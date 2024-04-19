using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class RateLimitedServiceHttpClient : IServiceHttpClient
    {
        readonly IServiceHttpClient m_ServiceHttpClient;
        readonly TokenBucketRateLimiter m_RateLimiter;

        public RateLimitedServiceHttpClient(IServiceHttpClient serviceHttpClient, int queueLimit, int tokensPerPeriod, int tokenLimit, TimeSpan replenishmentPeriod)
        {
            m_ServiceHttpClient = serviceHttpClient;
            m_RateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions()
            {
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                AutoReplenishment = true,
                QueueLimit = queueLimit,
                TokensPerPeriod = tokensPerPeriod,
                TokenLimit = tokenLimit,
                ReplenishmentPeriod = replenishmentPeriod/*TimeSpan.FromSeconds(k_ReplenishmentPeriod)*/
            });
        }

        /// <inheritdoc />
        public TimeSpan Timeout
        {
            get => m_ServiceHttpClient.Timeout;
            set => m_ServiceHttpClient.Timeout = value;
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, IProgress<HttpProgress> progress,
            CancellationToken cancellationToken)
        {
            await m_RateLimiter.AcquireAsync(1, cancellationToken);
            return await m_ServiceHttpClient.SendAsync(request, completionOption, progress, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, ServiceHttpClientOptions options, HttpCompletionOption completionOption,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            await m_RateLimiter.AcquireAsync(1, cancellationToken);
            return await m_ServiceHttpClient.SendAsync(request, options, completionOption, progress, cancellationToken);
        }
    }
}

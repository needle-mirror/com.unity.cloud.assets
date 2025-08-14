using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class AssetDataSource : IAssetDataSource
    {
        const int k_QueueLimit = 100000;
        const int k_DefaultTokensPerPeriod = 30;
        const int k_DefaultTokenLimit = 30;
        const int k_SlowTokensPerPeriod = 5;
        const int k_SlowTokenLimit = 5;
        const double k_ReplenishmentPeriod = 0.45; // we add 0.05s to each period to have a safety margin
        const double k_SlowReplenishmentPeriod = 1;
        const string k_PublicApiPath = "/assets/v1";

        static readonly UCLogger k_Logger = LoggerProvider.GetLogger<AssetDataSource>();

        readonly IServiceHttpClient m_ServiceHttpClient;
        readonly IServiceHostResolver m_PublicServiceHostResolver;
        readonly Dictionary<string, IServiceHttpClient> m_HttpClients = new();
        readonly object m_Lock = new();

        internal AssetDataSource(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            if (serviceHostResolver is ServiceHostResolver unityServiceHostResolver &&
                unityServiceHostResolver.GetResolvedEnvironment() == ServiceEnvironment.Test)
            {
                var headers = new Dictionary<string, string>
                {
                    {"x-backend-host", "https://api.fd.amc.test.transformation.unity.com"}
                };
                serviceHttpClient = new ServiceHttpClientHeaderModifier(serviceHttpClient, headers);
            }

            m_ServiceHttpClient = serviceHttpClient;
            m_PublicServiceHostResolver = serviceHostResolver;
        }

        string GetPublicRequestUri(ApiRequest request)
        {
            return m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(k_PublicApiPath));
        }

        /// <inheritdoc />
        public async Task<IAssetData> GetAssetAsync(AssetDescriptor assetDescriptor, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = assetDescriptor.IsPathToAssetLibrary()
                ? new AssetRequest(assetDescriptor.AssetLibraryId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, includedFieldsFilter)
                : new AssetRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, includedFieldsFilter);
            using var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<AssetData>(jsonContent);
        }

        /// <inheritdoc />
        public async Task<IAssetData> GetAssetAsync(ProjectDescriptor projectDescriptor, AssetId assetId, string label, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new AssetRequest(projectDescriptor.ProjectId, assetId, label, includedFieldsFilter);
            using var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<AssetData>(jsonContent);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetData> ListAssetsAsync(AssetLibraryId assetLibraryId, SearchRequestParameters parameters, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (offset, length) = await parameters.PaginationRange.GetOffsetAndLengthAsync(token => GetAssetCountAsync(assetLibraryId, token), cancellationToken);
            if (length == 0) yield break;

            var request = new SearchRequest(assetLibraryId, parameters);
            var results = ListAssetsAsync(request, parameters, offset, length, cancellationToken);
            await foreach (var asset in results)
            {
                yield return asset;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetData> ListAssetsAsync(ProjectDescriptor projectDescriptor, SearchRequestParameters parameters, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (offset, length) = await parameters.PaginationRange.GetOffsetAndLengthAsync(token => GetAssetCountAsync(projectDescriptor, token), cancellationToken);
            if (length == 0) yield break;

            var request = new SearchRequest(projectDescriptor.ProjectId, parameters);
            var results = ListAssetsAsync(request, parameters, offset, length, cancellationToken);
            await foreach (var asset in results)
            {
                yield return asset;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetData> ListAssetsAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, SearchRequestParameters parameters, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (offset, length) = await parameters.PaginationRange.GetOffsetAndLengthAsync(token => GetAcrossProjectsTotalCount(organizationId, projectIds, token), cancellationToken);
            if (length == 0) yield break;

            var request = new AcrossProjectsSearchRequest(organizationId, parameters);
            var results = ListAssetsAsync(request, parameters, offset, length, cancellationToken);
            await foreach (var asset in results)
            {
                yield return asset;
            }
        }

        /// <inheritdoc />
        public Task<AggregateDto[]> GetAssetAggregateAsync(AssetLibraryId assetLibraryId, SearchAndAggregateRequestParameters parameters, CancellationToken cancellationToken)
        {
            var request = new SearchAndAggregateRequest(assetLibraryId, parameters);
            return GetAssetAggregateAsync(request, cancellationToken);
        }

        /// <inheritdoc />
        public Task<AggregateDto[]> GetAssetAggregateAsync(ProjectDescriptor projectDescriptor, SearchAndAggregateRequestParameters parameters, CancellationToken cancellationToken)
        {
            var request = new SearchAndAggregateRequest(projectDescriptor.ProjectId, parameters);
            return GetAssetAggregateAsync(request, cancellationToken);
        }

        async Task<AggregateDto[]> GetAssetAggregateAsync(ApiRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            return JsonSerialization.Deserialize<AggregationsDto>(jsonContent).Aggregations;
        }

        /// <inheritdoc />
        public async Task<AggregateDto[]> GetAssetAggregateAsync(OrganizationId organizationId, AcrossProjectsSearchAndAggregateRequestParameters parameters, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new AcrossProjectsSearchAndAggregateRequest(organizationId, parameters);
            using var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            return JsonSerialization.Deserialize<AggregationsDto>(jsonContent).Aggregations;
        }

        /// <inheritdoc />
        public async Task<AssetDescriptor> CreateAssetAsync(ProjectDescriptor projectDescriptor, IAssetCreateData assetCreation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CreateAssetRequest(projectDescriptor.ProjectId, assetCreation);
            using var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            var createdAsset = IsolatedSerialization.DeserializeWithDefaultConverters<CreatedAssetDto>(jsonContent);

            return new AssetDescriptor(projectDescriptor, createdAsset.AssetId, createdAsset.AssetVersion);
        }

        /// <inheritdoc />
        public async Task UpdateAssetAsync(AssetDescriptor assetDescriptor, IAssetUpdateData data, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new AssetRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, data);
            using var _ = await RateLimitedServiceClient(request, HttpClientExtensions.HttpMethodPatch).PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<AssetDownloadUrl> GetAssetDownloadUrlsAsync(AssetDescriptor assetDescriptor, DatasetId[] datasetIds, Range range, CancellationToken cancellationToken)
        {
            return assetDescriptor.IsPathToAssetLibrary()
                ? GetAssetDownloadUrlsAsync(assetDescriptor.AssetLibraryId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, datasetIds, range, cancellationToken)
                : GetAssetDownloadUrlsAsync(assetDescriptor.ProjectDescriptor, assetDescriptor.AssetId, assetDescriptor.AssetVersion, datasetIds, range, cancellationToken);
        }

        IAsyncEnumerable<AssetDownloadUrl> GetAssetDownloadUrlsAsync(AssetLibraryId assetLibraryId, AssetId assetId, AssetVersion assetVersion, DatasetId[] datasetIds, Range range, CancellationToken cancellationToken)
        {
            const int maxPageSize = 1000;

            var countRequest = new GetAssetDownloadUrlsRequest(assetLibraryId, assetId, assetVersion, datasetIds, 0, 1, null);
            return ListEntitiesAsync<AssetDownloadUrl>(countRequest, GetListRequest, range, cancellationToken, maxPageSize);

            ApiRequest GetListRequest(int offset, int pageSize) => new GetAssetDownloadUrlsRequest(assetLibraryId, assetId, assetVersion, datasetIds, offset, pageSize, null);
        }

        async IAsyncEnumerable<AssetDownloadUrl> GetAssetDownloadUrlsAsync(ProjectDescriptor projectDescriptor, AssetId assetId, AssetVersion assetVersion, DatasetId[] datasetIds, Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new GetAssetDownloadUrlsRequest(projectDescriptor.ProjectId, assetId, assetVersion, datasetIds, null);
            using var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            var assetDownloadUrlsDto = JsonSerialization.Deserialize<AssetDownloadUrlsDto>(jsonContent);

            var (offset, length) = await range.GetOffsetAndLengthAsync(token => GetTotalCount(request, token), cancellationToken);
            if (length == 0) yield break;

            for (var i = offset; i < assetDownloadUrlsDto.FileUrls.Count; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return new AssetDownloadUrl
                {
                    FilePath = assetDownloadUrlsDto.FileUrls[i].Path,
                    DownloadUrl = GetEscapedUri(assetDownloadUrlsDto.FileUrls[i].Url)
                };
            }
        }

        /// <inheritdoc />
        public async Task LinkAssetToProjectAsync(AssetDescriptor assetDescriptor, ProjectDescriptor destinationProject, CancellationToken cancellationToken)
        {
            var request = new LinkAssetToProjectRequest(assetDescriptor.ProjectId, destinationProject.ProjectId, assetDescriptor.AssetId);
            using var _ = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task UnlinkAssetFromProjectAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var request = new UnlinkAssetFromProjectRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId);
            using var _ = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> CheckIsProjectAssetSourceAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = AssetRequest.CheckProjectIsAssetSourceProjectRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId);
            using var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            return bool.Parse(await response.GetContentAsStringAsync());
        }

        /// <inheritdoc />
        public async Task<bool> CheckAssetBelongsToProjectAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = AssetRequest.CheckAssetBelongsToProjectRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId);
            using var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            return bool.Parse(await response.GetContentAsStringAsync());
        }

        /// <inheritdoc />
        public async Task UpdateAssetStatusAsync(AssetDescriptor assetDescriptor, string statusName, CancellationToken cancellationToken)
        {
            var request = new AssetStatusRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, statusName);
            using var _ = await RateLimitedServiceClient(request, HttpClientExtensions.HttpMethodPatch).PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task UploadContentAsync(Uri uploadUri, Stream sourceStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            const string blobTypeHeaderKey = "X-Ms-Blob-Type";
            const string blobTypeHeaderValue = "BlockBlob";

            cancellationToken.ThrowIfCancellationRequested();

            if (uploadUri == null)
            {
                throw new InvalidUrlException("Upload url is null or empty");
            }

            using var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Put;
            httpRequestMessage.RequestUri = uploadUri;
            httpRequestMessage.Content = new StreamContent(sourceStream);

            httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            httpRequestMessage.Headers.Add(blobTypeHeaderKey, blobTypeHeaderValue);

            using var response = await RateLimitedServiceClient("UploadFile", HttpMethod.Put)
                .SendAsync(httpRequestMessage, ServiceHttpClientOptions.SkipDefaultAuthenticationOption(), HttpCompletionOption.ResponseContentRead, progress, cancellationToken);

            try
            {
                _ = response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                throw new UploadFailedException($"Upload of content stream for file id {uploadUri} failed.");
            }
        }

        /// <inheritdoc />
        public async Task DownloadContentAsync(Uri downloadUri, Stream destinationStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (downloadUri == null)
            {
                throw new InvalidUrlException("Download url is null or empty");
            }

            using var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = downloadUri;

            using var response = await m_ServiceHttpClient.SendAsync(httpRequestMessage, ServiceHttpClientOptions.SkipDefaultAuthenticationOption(), HttpCompletionOption.ResponseContentRead, progress, cancellationToken);
            response.EnsureSuccessStatusCode();

            var source = await response.Content.ReadAsStreamAsync();

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await source.CopyToAsync(destinationStream, cancellationToken);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Could not write to {nameof(destinationStream)}", nameof(destinationStream), e);
            }
            finally
            {
                await source.DisposeAsync();
            }
        }

        /// <inheritdoc />
        public async Task RemoveAssetMetadataAsync(AssetDescriptor assetDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var request = new RemoveMetadataRequest(assetDescriptor.ProjectId,
                assetDescriptor.AssetId,
                assetDescriptor.AssetVersion,
                metadataType,
                keys);

            using var _ = await RateLimitedServiceClient(request, HttpMethod.Delete).DeleteAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Uri GetServiceRequestUrl(string relativePath)
        {
            return new Uri(m_PublicServiceHostResolver.GetResolvedRequestUri(relativePath));
        }

        async IAsyncEnumerable<T> ListEntitiesAsync<T>(PaginationHelpers.GetTotalCount getTotalCount, Func<string, int, ApiRequest> getListRequest, Range range, [EnumeratorCancellation] CancellationToken cancellationToken, int maxPageSize = 1000)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (offset, length) = await range.GetOffsetAndLengthAsync(getTotalCount, cancellationToken);

            if (length == 0) yield break;

            var pageSize = Math.Min(maxPageSize, Math.Max(offset, length));

            string next = null;

            var count = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = getListRequest(next, pageSize);
                using var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

                var jsonContent = await response.GetContentAsStringAsync();
                cancellationToken.ThrowIfCancellationRequested();

                var pageDto = IsolatedSerialization.DeserializeWithDefaultConverters<EntityPageDto<T>>(jsonContent);

                if (pageDto.Results == null || pageDto.Results.Length == 0) break;

                if (pageDto.Total.HasValue)
                {
                    // Cap the length to the total number of results.
                    length = Math.Min(length, pageDto.Total.Value);
                }

                // Update the next token.
                next = pageDto.Next;

                for (var i = 0; i < pageDto.Results.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Bring offset to 0 before starting to yield results.
                    if (offset-- > 0) continue;

                    // Stop yielding results if we have reached the desired count.
                    if (count >= length) break;

                    ++count;
                    yield return pageDto.Results[i];
                }
            } while (count < length && !string.IsNullOrEmpty(next));
        }

        IAsyncEnumerable<T> ListEntitiesAsync<T>(ApiRequest countRequest, Func<string, int, ApiRequest> getListRequest, Range range, CancellationToken cancellationToken, int maxPageSize = 1000)
            => ListEntitiesAsync<T>(token => GetTotalCount(countRequest, token), getListRequest, range, cancellationToken, maxPageSize);

        async IAsyncEnumerable<T> ListEntitiesAsync<T>(PaginationHelpers.GetTotalCount getTotalCount, Func<int, int, ApiRequest> getListRequest, Range range, [EnumeratorCancellation] CancellationToken cancellationToken, int maxPageSize = 1000)
        {
            var (offset, length) = await range.GetOffsetAndLengthAsync(getTotalCount, cancellationToken);
            var pageSize = Math.Min(maxPageSize, Math.Max(offset, length));

            var count = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = getListRequest(offset, pageSize);
                using var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

                var jsonContent = await response.GetContentAsStringAsync();
                cancellationToken.ThrowIfCancellationRequested();

                var pageDto = IsolatedSerialization.DeserializeWithDefaultConverters<EntityPageDto<T>>(jsonContent);

                if (pageDto.Results == null || pageDto.Results.Length == 0) break;

                for (var i = 0; i < pageDto.Results.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (count >= length) break;

                    ++count;
                    yield return pageDto.Results[i];
                }

                if (pageDto.Total.HasValue)
                {
                    // Cap the length to the total number of entries.
                    length = Math.Min(length, pageDto.Total.Value);
                }

                // Update the offset and page size for the next iteration
                offset += pageSize;
                pageSize = Math.Min(pageSize, length - offset);
            } while (count < length);
        }

        IAsyncEnumerable<T> ListEntitiesAsync<T>(ApiRequest countRequest, Func<int, int, ApiRequest> getListRequest, Range range, CancellationToken cancellationToken, int maxPageSize = 1000)
            => ListEntitiesAsync<T>(token => GetTotalCount(countRequest, token), getListRequest, range, cancellationToken, maxPageSize);

        async IAsyncEnumerable<IAssetData> ListAssetsAsync(ApiRequest request, SearchRequestParameters parameters, int offset, int length, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const int maxPageSize = 99;

            if (length == 0) yield break;

            var pagination = parameters.Pagination;

            var lastIndex = offset + length;
            var pageSize = Math.Min(maxPageSize, lastIndex);
            parameters.Pagination.Limit = pageSize;

            var startPage = offset / pageSize;
            var currentIndex = offset;

            var firstPage = await AdvanceTokenToFirstPageAsync<AssetData>(request, pagination, startPage, cancellationToken);

            for (var i = offset % pageSize; i < firstPage.Results.Length; ++i)
            {
                if (currentIndex++ >= lastIndex) break;

                cancellationToken.ThrowIfCancellationRequested();

                yield return firstPage.Results[i];
            }

            pagination.Token = firstPage.Next;

            pageSize = Math.Min(maxPageSize, length);
            pagination.Limit = pageSize;

            var results = GetNextAsync<AssetData>(request, pagination, currentIndex, offset, length, cancellationToken);
            await foreach (var result in results)
            {
                yield return result;
            }
        }

        async Task<EntityPageDto<T>> AdvanceTokenToFirstPageAsync<T>(ApiRequest request, SearchRequestPagination pagination, int startPage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestUri = GetPublicRequestUri(request);

            var currentPage = 0;

            HttpResponseMessage response = null;
            string jsonContent;
            try
            {
                response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(requestUri, request.ConstructBody(),
                    ServiceHttpClientOptions.Default(), cancellationToken);

                while (currentPage < startPage)
                {
                    ++currentPage;

                    jsonContent = await response.GetContentAsStringAsync();
                    cancellationToken.ThrowIfCancellationRequested();

                    var pageTokenDto = JsonSerialization.Deserialize<PageTokenDto>(jsonContent);
                    pagination.Token = pageTokenDto.Token;

                    cancellationToken.ThrowIfCancellationRequested();

                    response?.Dispose(); // dispose of the response before re-assignment
                    response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(requestUri, request.ConstructBody(),
                        ServiceHttpClientOptions.Default(), cancellationToken);
                }

                jsonContent = await response.GetContentAsStringAsync();
            }
            finally
            {
                response?.Dispose();
            }

            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<EntityPageDto<T>>(jsonContent);
        }

        async IAsyncEnumerable<T> GetNextAsync<T>(ApiRequest request, SearchRequestPagination pagination, int index, int offset, int length, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var requestUri = GetPublicRequestUri(request);

            var cutoff = offset + length;
            while (index < cutoff)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrEmpty(pagination.Token)) break;

                using var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(requestUri, request.ConstructBody(),
                    ServiceHttpClientOptions.Default(), cancellationToken);

                var jsonContent = await response.GetContentAsStringAsync();
                cancellationToken.ThrowIfCancellationRequested();

                var dto = IsolatedSerialization.DeserializeWithDefaultConverters<EntityPageDto<T>>(jsonContent);

                // To prevent an infinite loop, return if no assets were returned
                if (dto.Results.Length == 0) break;

                foreach (var asset in dto.Results)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (++index < offset) continue;
                    if (index > cutoff) yield break;

                    yield return asset;
                }

                pagination.Token = dto.Next;
            }
        }

        async Task<int> GetAcrossProjectsTotalCount(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, CancellationToken cancellationToken)
        {
            var parameters = new AcrossProjectsSearchAndAggregateRequestParameters(projectIds.ToArray(), AssetTypeSearchCriteria.SearchKey);
            var aggregations = await GetAssetAggregateAsync(organizationId, parameters, cancellationToken);
            var total = 0;
            foreach (var aggregate in aggregations)
            {
                total += aggregate.Count;
            }

            return total;
        }

        IServiceHttpClient RateLimitedServiceClient(ApiRequest request, HttpMethod httpMethod)
        {
            return RateLimitedServiceClient(request, httpMethod.ToString());
        }

        IServiceHttpClient RateLimitedServiceClient(ApiRequest request, string httpMethod)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return m_ServiceHttpClient;
#else
            var requestKey = request.GetType() + httpMethod;
            IServiceHttpClient client;

            lock (m_Lock)
            {
                if (m_HttpClients.TryGetValue(requestKey, out client)) return client;

                client = IsSlowRequest(request)
                    ? new RateLimitedServiceHttpClient(m_ServiceHttpClient, k_QueueLimit, k_SlowTokensPerPeriod,
                        k_SlowTokenLimit, TimeSpan.FromSeconds(k_SlowReplenishmentPeriod))
                    : new RateLimitedServiceHttpClient(m_ServiceHttpClient, k_QueueLimit, k_DefaultTokensPerPeriod,
                        k_DefaultTokenLimit, TimeSpan.FromSeconds(k_ReplenishmentPeriod));

                m_HttpClients[requestKey] = client;
            }

            return client;
#endif
        }

        IServiceHttpClient RateLimitedServiceClient(string requestType, HttpMethod httpMethod)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return m_ServiceHttpClient;
#else
            var requestKey = requestType + httpMethod;
            IServiceHttpClient client;

            lock (m_Lock)
            {
                if (m_HttpClients.TryGetValue(requestKey, out client)) return client;

                client = new RateLimitedServiceHttpClient(m_ServiceHttpClient, k_QueueLimit, k_DefaultTokensPerPeriod,
                    k_SlowTokenLimit, TimeSpan.FromSeconds(k_ReplenishmentPeriod));

                m_HttpClients[requestKey] = client;
            }

            return client;
#endif
        }

        static bool IsSlowRequest(ApiRequest request)
        {
            return request is SearchRequest or AcrossProjectsSearchRequest or SearchAndAggregateRequest or AcrossProjectsSearchAndAggregateRequest;
        }

        static Uri GetEscapedUri(string url)
        {
            var uri = new Uri(url);

            // Using the AbsoluteUri of an existing Uri ensures that the url is properly escaped.
            return new Uri(uri.AbsoluteUri);
        }
    }
}

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
        const string k_PublicApiPath = "/assets/v1";

        readonly IServiceHttpClient m_ServiceHttpClient;
        readonly IServiceHostResolver m_PublicServiceHostResolver;

        internal AssetDataSource(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            m_ServiceHttpClient = serviceHttpClient;
            m_PublicServiceHostResolver = serviceHostResolver;
        }

        string GetPublicRequestUri(ApiRequest request)
        {
            return m_PublicServiceHostResolver.GetResolvedRequestUri(request.ConstructUrl(k_PublicApiPath));
        }

        /// <inheritdoc/>
        public async Task<IAssetData> GetAssetAsync(AssetDescriptor assetDescriptor, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken)
        {
            var request = new GetAssetByIdAndVersionRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, includedFieldsFilter);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);
            var jsonContent = await response.GetContentAsString();

            return IsolatedSerialization.DeserializeWithDefaultConverters<AssetData>(jsonContent);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetData> ListAssetsAsync(ProjectDescriptor projectDescriptor, IAssetSearchFilter assetSearchFilter, Pagination pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Set up the request.
            var requestFilter = GetRequestFilter(assetSearchFilter);
            var searchPagination = new SearchRequestPagination(pagination.SortingField, pagination.SortingOrder.ToString());

            // Still missing definitions for optional params:
            // - SearchRequestResultFields resultFields
            // - bool includeThumbnailDownloadURLs
            var requestParams = new SearchRequestParameters(requestFilter, assetSearchFilter.IncludedFields, searchPagination);

            var request = new SearchRequest(projectDescriptor.ProjectId, requestParams);

            var (offset, length) = await pagination.Range.GetOffsetAndLengthAsync(token => GetTotalCount(projectDescriptor, token), cancellationToken);
            if (length == 0) yield break;

            var results = ListAssetsAsync(request, requestParams, searchPagination, offset, length, cancellationToken);
            await foreach (var asset in results)
            {
                yield return asset;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetData> ListAssetsAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, IAssetSearchFilter assetSearchFilter, Pagination pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Set up the request.
            var requestFilter = GetRequestFilter(assetSearchFilter);
            var searchPagination = new SearchRequestPagination(pagination.SortingField, pagination.SortingOrder.ToString());

            // Still missing definitions for optional params:
            // - SearchRequestResultFields resultFields
            // - bool includeThumbnailDownloadURLs
            var enumerable = projectIds?.ToArray() ?? Array.Empty<ProjectId>();
            var requestParams = new AcrossProjectsSearchRequestParameters(enumerable, requestFilter, assetSearchFilter.IncludedFields, searchPagination);

            var request = new AcrossProjectsSearchRequest(organizationId, requestParams);

            var (offset, length) = await pagination.Range.GetOffsetAndLengthAsync(token => GetAcrossProjectsTotalCount(organizationId, enumerable, token), cancellationToken);
            if (length == 0) yield break;

            var results = ListAssetsAsync(request, requestParams, searchPagination, offset, length, cancellationToken);
            await foreach (var asset in results)
            {
                yield return asset;
            }
        }

        /// <inheritdoc />
        public async Task<Aggregation> GetAssetAggregateAsync(ProjectDescriptor projectDescriptor, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken cancellationToken)
        {
            var requestFilter = GetRequestFilter(assetSearchFilter);
            var requestParams = new SearchAndAggregateRequestParameters(requestFilter, parameters.AggregationField, parameters.ResultLimit);
            var request = new SearchAndAggregateRequest(projectDescriptor.ProjectId, requestParams);
            var response = await m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
            var jsonContent = await response.GetContentAsString();

            var aggregations = JsonSerialization.Deserialize<AggregationsDto>(jsonContent).Aggregations;

            var data = new Dictionary<string, int>();
            for (var i = 0; i < aggregations.Length; ++i)
            {
                data.TryAdd(aggregations[i].Value, aggregations[i].Count);
            }

            return new Aggregation(data);
        }

        /// <inheritdoc />
        public async Task<Aggregation> GetAssetAggregateAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken cancellationToken)
        {
            var requestFilter = GetRequestFilter(assetSearchFilter);
            var requestParams = new AcrossProjectsSearchAndAggregateRequestParameters(projectIds, requestFilter, parameters.AggregationField, parameters.ResultLimit);
            var request = new AcrossProjectsSearchAndAggregateRequest(organizationId, requestParams);
            var response = await m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
            var jsonContent = await response.GetContentAsString();

            var aggregations = JsonSerialization.Deserialize<AggregationsDto>(jsonContent).Aggregations;

            var data = new Dictionary<string, int>();
            for (var i = 0; i < aggregations.Length; ++i)
            {
                data.TryAdd(aggregations[i].Value, aggregations[i].Count);
            }

            return new Aggregation(data);
        }

        /// <inheritdoc />
        public async Task<IAssetData> CreateAssetAsync(ProjectDescriptor projectDescriptor, IAssetCreateData assetCreation, CancellationToken cancellationToken)
        {
            var request = new CreateAssetRequest(projectDescriptor.ProjectId, assetCreation);
            var response = await m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
            var jsonContent = await response.GetContentAsString();

            var createdAsset = JsonSerialization.Deserialize<CreatedAssetDto>(jsonContent);
            createdAsset.AssetId ??= "";
            createdAsset.StorageId ??= "";

            return new AssetData(createdAsset.AssetId, createdAsset.AssetVersion, createdAsset.StorageId)
            {
                SourceProjectId = projectDescriptor.ProjectId,
                Name = assetCreation.Name,
                Description = assetCreation.Description,
                Type = assetCreation.Type
            };
        }

        /// <inheritdoc />
        public Task UpdateAssetAsync(AssetDescriptor assetDescriptor, IAssetUpdateData data, CancellationToken cancellationToken)
        {
            var request = new UpdateAssetRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, data);
            return m_ServiceHttpClient.PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AssetDownloadUrl>> GetAssetDownloadUrlsAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var request = new GetAssetDownloadUrlsRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);
            var jsonContent = await response.GetContentAsString();

            var assetDownloadUrlsDto = JsonSerialization.Deserialize<AssetDownloadUrlsDto>(jsonContent);

            var urlList = assetDownloadUrlsDto.FileUrls.Select(f => new AssetDownloadUrl {FilePath = f.Path, DownloadUrl = new Uri(f.Url)}).ToList();

            return urlList;
        }

        /// <inheritdoc />
        public Task LinkAssetToProjectAsync(AssetDescriptor assetDescriptor, ProjectDescriptor destinationProject, CancellationToken cancellationToken)
        {
            var request = new LinkAssetToProjectRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, destinationProject.ProjectId);
            return m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Task UnlinkAssetFromProjectAsync(AssetDescriptor assetDescriptor, ProjectDescriptor destinationProject, CancellationToken cancellationToken)
        {
            var request = new UnlinkAssetFromProjectRequest(destinationProject.ProjectId, assetDescriptor.AssetId);
            return m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> CheckIsProjectAssetSourceAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var request = new CheckProjectIsAssetSourceProjectRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            return bool.Parse(await response.GetContentAsString());
        }

        /// <inheritdoc />
        public async Task<bool> CheckAssetBelongsToProjectAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var request = new CheckAssetBelongsToProjectRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            return bool.Parse(await response.GetContentAsString());
        }

        /// <inheritdoc />
        public Task PublishApprovedAssetAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var request = new ChangeAssetStatusRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, ChangeAssetStatusAction.published);
            return m_ServiceHttpClient.PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Task WithdrawPublishedAssetAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var request = new ChangeAssetStatusRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, ChangeAssetStatusAction.withdrawn);
            return m_ServiceHttpClient.PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Task SendAssetToReviewAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var request = new ChangeAssetStatusRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, ChangeAssetStatusAction.inreview);
            return m_ServiceHttpClient.PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Task ApproveAssetAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var request = new ChangeAssetStatusRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, ChangeAssetStatusAction.approved);
            return m_ServiceHttpClient.PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Task RejectAssetAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            var request = new ChangeAssetStatusRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, ChangeAssetStatusAction.rejected);
            return m_ServiceHttpClient.PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        static SearchRequestFilter GetRequestFilter(IAssetSearchFilter assetSearchFilter)
        {
            var anyQuery = assetSearchFilter.AccumulateAnyCriteria();

            return new SearchRequestFilter(assetSearchFilter.AccumulateIncludedCriteria(),
                assetSearchFilter.AccumulateExcludedCriteria(),
                anyQuery,
                anyQuery is {Count: > 0} ? assetSearchFilter.AnyQueryMinimumMatch : null,
                assetSearchFilter.Collections);
        }

        async Task<int> GetTotalCount(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            var aggregation = await GetAssetAggregateAsync(projectDescriptor, new AssetSearchFilter(), new AggregationParameters(AssetTypeSearchCriteria.SearchKey), cancellationToken);
            return aggregation.Total;
        }

        async Task<int> GetAcrossProjectsTotalCount(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, CancellationToken cancellationToken)
        {
            var aggregation = await GetAssetAggregateAsync(organizationId, projectIds, new AssetSearchFilter(), new AggregationParameters(AssetTypeSearchCriteria.SearchKey), cancellationToken);
            return aggregation.Total;
        }

        /// <inheritdoc />
        public async Task UploadContentAsync(Uri uploadUri, Stream sourceStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            const string blobTypeHeaderKey = "X-Ms-Blob-Type";
            const string blobTypeHeaderValue = "BlockBlob";

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

            var httpClientOptions = new ServiceHttpClientOptions(true, false, false,
                false, retryPolicy: new NoRetryPolicy());

            var response = await m_ServiceHttpClient.SendAsync(httpRequestMessage, httpClientOptions,
                HttpCompletionOption.ResponseContentRead, progress, cancellationToken);

            var result = response.EnsureSuccessStatusCode();
            if (!result.IsSuccessStatusCode)
            {
                throw new UploadFailedException($"Upload of content stream for file id {uploadUri} failed.");
            }
        }

        /// <inheritdoc />
        public async Task DownloadContentAsync(Uri downloadUri, Stream destinationStream, IProgress<HttpProgress> progress, CancellationToken token)
        {
            if (downloadUri == null)
            {
                throw new InvalidUrlException("Download url is null or empty");
            }

            using var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = downloadUri;

            using var response = await m_ServiceHttpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, progress, token);
            response.EnsureSuccessStatusCode();

            var source = await response.Content.ReadAsStreamAsync();

            try
            {
                await source.CopyToAsync(destinationStream, token);
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
        public Task RemoveAssetMetadataAsync(AssetDescriptor assetDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken token)
        {
            var request = new RemoveMetadataRequest(assetDescriptor.ProjectId,
                assetDescriptor.AssetId,
                assetDescriptor.AssetVersion,
                metadataType,
                keys);
            return m_ServiceHttpClient.DeleteAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
        }

        /// <inheritdoc />
        public Uri GetServiceUrl()
        {
            return new Uri(m_PublicServiceHostResolver.GetResolvedAddress());
        }

        async IAsyncEnumerable<IAssetData> ListAssetsAsync(ApiRequest request, SearchRequestParameters parameters, SearchRequestPagination pagination, int offset, int length, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const int maxPageSize = 99;

            var lastIndex = offset + length;
            var pageSize = Math.Min(maxPageSize, lastIndex);
            parameters.Pagination.Limit = pageSize;

            var startPage = offset / pageSize;
            var currentIndex = offset;

            var firstPage = await AdvanceTokenToFirstPageAsync(request, pagination, startPage, cancellationToken);

            for (var i = offset % pageSize; i < firstPage.Assets.Length; ++i)
            {
                if (currentIndex++ >= lastIndex) break;

                yield return firstPage.Assets[i];
            }

            pagination.Token = firstPage.Token;

            pageSize = Math.Min(maxPageSize, length);
            pagination.Limit = pageSize;

            var enumerator = GetNextAsset(request, pagination, currentIndex, offset, length, cancellationToken);
            while (await enumerator.MoveNextAsync())
            {
                yield return enumerator.Current;
            }
        }

        async Task<AssetPageDto> AdvanceTokenToFirstPageAsync(ApiRequest request, SearchRequestPagination pagination, int startPage, CancellationToken cancellationToken)
        {
            var requestUri = GetPublicRequestUri(request);

            var currentPage = 0;

            var response = await m_ServiceHttpClient.PostAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            string jsonContent;
            while (currentPage < startPage)
            {
                ++currentPage;
                jsonContent = await response.GetContentAsString();
                var pageTokenDto = JsonSerialization.Deserialize<PageTokenDto>(jsonContent);
                pagination.Token = pageTokenDto.Token;
                response = await m_ServiceHttpClient.PostAsync(requestUri, request.ConstructBody(),
                    ServiceHttpClientOptions.Default(), cancellationToken);
            }

            jsonContent = await response.GetContentAsString();
            return IsolatedSerialization.DeserializeWithDefaultConverters<AssetPageDto>(jsonContent);
        }

        async IAsyncEnumerator<IAssetData> GetNextAsset(ApiRequest request, SearchRequestPagination pagination, int index, int offset, int length, CancellationToken cancellationToken)
        {
            var requestUri = GetPublicRequestUri(request);

            var lastIndex = offset + length;
            while (index <= lastIndex)
            {
                if (string.IsNullOrEmpty(pagination.Token)) break;

                var response = await m_ServiceHttpClient.PostAsync(requestUri, request.ConstructBody(),
                    ServiceHttpClientOptions.Default(), cancellationToken);
                var jsonContent = await response.GetContentAsString();
                var dto = IsolatedSerialization.DeserializeWithDefaultConverters<AssetPageDto>(jsonContent);

                // To prevent an infinite loop, return if no assets were returned
                if (dto.Assets.Length == 0) break;

                foreach (var asset in dto.Assets)
                {
                    if (++index < offset) continue;
                    if (index > lastIndex) break;

                    yield return asset;
                }

                pagination.Token = dto.Token;
            }
        }
    }
}

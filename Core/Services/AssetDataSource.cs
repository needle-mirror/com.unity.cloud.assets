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
            cancellationToken.ThrowIfCancellationRequested();

            var request = new GetAssetByIdAndVersionRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, includedFieldsFilter);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<AssetData>(jsonContent);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetData> ListAssetsAsync(ProjectDescriptor projectDescriptor, SearchData searchData, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Set up the request.
            var requestFilter = GetRequestFilter(searchData.AssetSearchFilter);
            var searchPagination = new SearchRequestPagination(searchData.Pagination.SortingField, searchData.Pagination.SortingOrder.ToString());

            // Still missing definitions for optional params:
            // - SearchRequestResultFields resultFields
            // - bool includeThumbnailDownloadURLs
            var requestParams = new SearchRequestParameters(requestFilter, searchData.IncludedFields, searchPagination);

            var request = new SearchRequest(projectDescriptor.ProjectId, requestParams);

            var (offset, length) = await searchData.Pagination.Range.GetOffsetAndLengthAsync(token => GetTotalCount(projectDescriptor, token), cancellationToken);
            if (length == 0) yield break;

            var results = ListAssetsAsync(request, requestParams, searchPagination, offset, length, cancellationToken);
            await foreach (var asset in results)
            {
                yield return asset;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetData> ListAssetsAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, SearchData searchData, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Set up the request.
            var requestFilter = GetRequestFilter(searchData.AssetSearchFilter);
            var searchPagination = new SearchRequestPagination(searchData.Pagination.SortingField, searchData.Pagination.SortingOrder.ToString());

            // Still missing definitions for optional params:
            // - SearchRequestResultFields resultFields
            // - bool includeThumbnailDownloadURLs
            var enumerable = projectIds?.ToArray() ?? Array.Empty<ProjectId>();
            var requestParams = new AcrossProjectsSearchRequestParameters(enumerable, requestFilter, searchData.IncludedFields, searchPagination);

            var request = new AcrossProjectsSearchRequest(organizationId, requestParams);

            var (offset, length) = await searchData.Pagination.Range.GetOffsetAndLengthAsync(token => GetAcrossProjectsTotalCount(organizationId, enumerable, token), cancellationToken);
            if (length == 0) yield break;

            var results = ListAssetsAsync(request, requestParams, searchPagination, offset, length, cancellationToken);
            await foreach (var asset in results)
            {
                yield return asset;
            }
        }

        /// <inheritdoc />
        public async Task<AggregateDto[]> GetAssetAggregateAsync(ProjectDescriptor projectDescriptor, AggregationData aggregationData, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestFilter = GetRequestFilter(aggregationData.AssetSearchFilter);
            var requestParams = new SearchAndAggregateRequestParameters(requestFilter, aggregationData.AggregationField, aggregationData.ResultLimit);
            var request = new SearchAndAggregateRequest(projectDescriptor.ProjectId, requestParams);
            var response = await m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return JsonSerialization.Deserialize<AggregationsDto>(jsonContent).Aggregations;
        }

        /// <inheritdoc />
        public async Task<AggregateDto[]> GetAssetAggregateAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, AggregationData aggregationData, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestFilter = GetRequestFilter(aggregationData.AssetSearchFilter);
            var requestParams = new AcrossProjectsSearchAndAggregateRequestParameters(projectIds, requestFilter, aggregationData.AggregationField, aggregationData.ResultLimit);
            var request = new AcrossProjectsSearchAndAggregateRequest(organizationId, requestParams);
            var response = await m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return JsonSerialization.Deserialize<AggregationsDto>(jsonContent).Aggregations;
        }

        /// <inheritdoc />
        public async Task<IAssetData> CreateAssetAsync(ProjectDescriptor projectDescriptor, IAssetCreateData assetCreation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CreateAssetRequest(projectDescriptor.ProjectId, assetCreation);
            var response = await m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var createdAsset = IsolatedSerialization.DeserializeWithDefaultConverters<CreatedAssetDto>(jsonContent);

            return new AssetData(createdAsset.AssetId, createdAsset.AssetVersion)
            {
                SourceProjectId = projectDescriptor.ProjectId,
                LinkedProjectIds = new[] {projectDescriptor.ProjectId},
                Name = assetCreation.Name,
                Description = assetCreation.Description,
                Type = assetCreation.Type,
                Datasets = createdAsset.Datasets,
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
            cancellationToken.ThrowIfCancellationRequested();

            var request = new GetAssetDownloadUrlsRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

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
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CheckProjectIsAssetSourceProjectRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            return bool.Parse(await response.GetContentAsString());
        }

        /// <inheritdoc />
        public async Task<bool> CheckAssetBelongsToProjectAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CheckAssetBelongsToProjectRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            return bool.Parse(await response.GetContentAsString());
        }

        /// <inheritdoc />
        public Task UpdateAssetStatusAsync(AssetDescriptor assetDescriptor, AssetStatusAction assetStatusAction, CancellationToken cancellationToken)
        {
            var request = new ChangeAssetStatusRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, assetStatusAction);
            return m_ServiceHttpClient.PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        static SearchRequestFilter GetRequestFilter(IAssetSearchFilter assetSearchFilter)
        {
            var anyQuery = assetSearchFilter.AccumulateAnyCriteria();

            return new SearchRequestFilter(assetSearchFilter.AccumulateIncludedCriteria(),
                assetSearchFilter.AccumulateExcludedCriteria(),
                anyQuery.criteria,
                anyQuery.criteria is {Count: > 0} ? anyQuery.minimumMatches : null,
                assetSearchFilter.Collections.GetValue());
        }

        async Task<int> GetTotalCount(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            var aggregateData = new AggregationData
            {
                AssetSearchFilter = new AssetSearchFilter(),
                AggregationField = AssetTypeSearchCriteria.SearchKey,
                ResultLimit = int.MaxValue
            };
            var aggregations = await GetAssetAggregateAsync(projectDescriptor, aggregateData, cancellationToken);
            var total = 0;
            foreach (var aggregate in aggregations)
            {
                total += aggregate.Count;
            }
            return total;
        }

        async Task<int> GetAcrossProjectsTotalCount(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, CancellationToken cancellationToken)
        {
            var aggregateData = new AggregationData
            {
                AssetSearchFilter = new AssetSearchFilter(),
                AggregationField = AssetTypeSearchCriteria.SearchKey,
                ResultLimit = int.MaxValue
            };
            var aggregations = await GetAssetAggregateAsync(organizationId, projectIds, aggregateData, cancellationToken);
            var total = 0;
            foreach (var aggregate in aggregations)
            {
                total += aggregate.Count;
            }
            return total;
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

            using var response = await m_ServiceHttpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, progress, cancellationToken);
            response.EnsureSuccessStatusCode();

            var source = await response.Content.ReadAsStreamAsync();

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

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
        public Task RemoveAssetMetadataAsync(AssetDescriptor assetDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var request = new RemoveMetadataRequest(assetDescriptor.ProjectId,
                assetDescriptor.AssetId,
                assetDescriptor.AssetVersion,
                metadataType,
                keys);
            return m_ServiceHttpClient.DeleteAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public Uri GetServiceUrl()
        {
            return new Uri(m_PublicServiceHostResolver.GetResolvedAddress());
        }

        async IAsyncEnumerable<IAssetData> ListAssetsAsync(ApiRequest request, SearchRequestParameters parameters, SearchRequestPagination pagination, int offset, int length, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (length == 0) yield break;

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

                cancellationToken.ThrowIfCancellationRequested();

                yield return firstPage.Assets[i];
            }

            pagination.Token = firstPage.Token;

            pageSize = Math.Min(maxPageSize, length);
            pagination.Limit = pageSize;

            var results = GetNextAsset(request, pagination, currentIndex, offset, length, cancellationToken);
            await foreach (var result in results)
            {
                yield return result;
            }
        }

        async Task<AssetPageDto> AdvanceTokenToFirstPageAsync(ApiRequest request, SearchRequestPagination pagination, int startPage, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var requestUri = GetPublicRequestUri(request);

            var currentPage = 0;

            var response = await m_ServiceHttpClient.PostAsync(requestUri, request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            string jsonContent;
            while (currentPage < startPage)
            {
                ++currentPage;

                jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var pageTokenDto = JsonSerialization.Deserialize<PageTokenDto>(jsonContent);
                pagination.Token = pageTokenDto.Token;

                cancellationToken.ThrowIfCancellationRequested();

                response = await m_ServiceHttpClient.PostAsync(requestUri, request.ConstructBody(),
                    ServiceHttpClientOptions.Default(), cancellationToken);
            }

            jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<AssetPageDto>(jsonContent);
        }

        async IAsyncEnumerable<IAssetData> GetNextAsset(ApiRequest request, SearchRequestPagination pagination, int index, int offset, int length, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var requestUri = GetPublicRequestUri(request);

            var lastIndex = offset + length;
            while (index <= lastIndex)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrEmpty(pagination.Token)) break;

                var response = await m_ServiceHttpClient.PostAsync(requestUri, request.ConstructBody(),
                    ServiceHttpClientOptions.Default(), cancellationToken);

                var jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var dto = IsolatedSerialization.DeserializeWithDefaultConverters<AssetPageDto>(jsonContent);

                // To prevent an infinite loop, return if no assets were returned
                if (dto.Assets.Length == 0) break;

                foreach (var asset in dto.Assets)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (++index < offset) continue;
                    if (index > lastIndex) break;

                    yield return asset;
                }

                pagination.Token = dto.Token;
            }
        }
    }
}

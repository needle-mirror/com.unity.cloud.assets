using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class AssetDataSource : IAssetDataSource
    {
        readonly IAssetHttpClient m_DiscoveryClient;
        readonly IAssetHttpClient m_ManagementClient;
        readonly AssetServiceConfiguration m_AssetServiceConfiguration;

        IAssetHttpClient Client => m_AssetServiceConfiguration.IsDiscovery ? m_DiscoveryClient : m_ManagementClient;
        string AssetPath => m_AssetServiceConfiguration.IsDiscovery ? "" : "/assets";

        internal AssetDataSource(IServiceHttpClient serviceHttpClient, string serviceAddress, AssetServiceConfiguration assetServiceConfiguration)
        {
            m_DiscoveryClient = new AssetDiscoveryHttpClient(serviceHttpClient, serviceAddress);
            m_ManagementClient = new AssetHttpClient(serviceHttpClient, serviceAddress);
            m_AssetServiceConfiguration = assetServiceConfiguration;
        }

        internal AssetDataSource(IAssetHttpClient client)
        {
            m_DiscoveryClient = client;
            m_ManagementClient = client;
            m_AssetServiceConfiguration = new AssetServiceConfiguration();
        }

        /// <inheritdoc/>
        public async Task<TAsset> GetAssetAsync<TAsset>(IProject project, string assetId, int assetVersion, CancellationToken token)
            where TAsset : IAsset, new()
        {
            var request = new GetAssetByIdAndVersionRequest(project.Organization.GenesisId, project.Id, assetId, assetVersion);
            var response = await Client.GetAsync(request, ServiceHttpClientOptions.Default(), token);

            var asset = IsolatedJsonConvert.DeserializeObject<TAsset>(response, new JsonAssetConverter());
            return GetInitializedAsset(asset, project);
        }

        public async IAsyncEnumerable<TAsset> ListAssetsAsync<TAsset>(IProject project, IAssetSearchFilter assetSearchFilter, Pagination pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
            where TAsset : IAsset, new()
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Set up the request.
            var requestFilter = GetRequestFilter(assetSearchFilter);
            var searchPagination = new SearchRequestPagination(pagination.SortingField, pagination.SortingOrder.ToString());

            // Still missing definitions for optional params:
            // - SearchRequestResultFields resultFields
            // - bool includeThumbnailDownloadURLs
            var requestParams = new SearchRequestParameters(requestFilter, pagination: searchPagination);

            // Still missing definitions for optional params:
            // - string xCorrelationId
            var request = new SearchRequest(project.Organization.GenesisId,
                project.Id,
                AssetPath,
                null,
                requestParams);

            var offsetAndLength = await pagination.Range.GetOffsetAndLengthAsync(token => GetTotalCount(project, token), cancellationToken);
            if (offsetAndLength.Length == 0) yield break;

            const int maxPageSize = 99;

            var limit = offsetAndLength.Offset + offsetAndLength.Length;
            var pageSize = Math.Min(maxPageSize, limit);
            request.SearchRequestParameter.Pagination.PageSize = pageSize;
            var startPage = offsetAndLength.Offset / pageSize;

            var firstPage = await AdvanceTokenToFirstPageAsync<TAsset>(request, startPage, cancellationToken);

            limit = Math.Min(limit, firstPage.Assets.Length);
            int index;
            for (index = offsetAndLength.Offset % pageSize; index < limit; ++index)
            {
                yield return GetInitializedAsset(firstPage.Assets[index], project);
            }

            if (string.IsNullOrEmpty(firstPage.Token)) yield break;

            searchPagination.Token = firstPage.Token;
            pageSize = Math.Min(maxPageSize, offsetAndLength.Length);
            searchPagination.PageSize = pageSize;

            var lastIndex = offsetAndLength.Offset + offsetAndLength.Length;
            while (index < lastIndex)
            {
                var response = await Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), cancellationToken);
                var dto = IsolatedJsonConvert.DeserializeObject<AssetPageDto<TAsset>>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

                // To prevent an infinite loop, return if no assets were returned
                // or if the token is empty.
                if (dto.Assets.Length == 0) break;

                foreach (var asset in dto.Assets)
                {
                    if (++index < offsetAndLength.Offset) continue;
                    if (index > lastIndex) break;

                    yield return GetInitializedAsset(asset, project);
                }

                // Break if the token is empty.
                if (string.IsNullOrEmpty(dto.Token)) break;

                searchPagination.Token = dto.Token;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TAsset> ListAssetsAsync<TAsset>(IOrganization organization, IEnumerable<IProject> projects, IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken cancellationToken)
            where TAsset : IAsset, new()
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Set up the request.
            var requestFilter = GetRequestFilter(assetSearchFilter);
            var searchPagination = new SearchRequestPagination(pagination.SortingField, pagination.SortingOrder.ToString());

            // Still missing definitions for optional params:
            // - SearchRequestResultFields resultFields
            // - bool includeThumbnailDownloadURLs
            var requestParams = new AcrossProjectsSearchRequestParameters(projects.Select(p => p.Id), requestFilter, pagination: searchPagination);

            // Still missing definitions for optional params:
            // - string xCorrelationId
            var request = new AcrossProjectsSearchRequest(organization.GenesisId,
                AssetPath,
                null,
                requestParams);

            var offsetAndLength = await pagination.Range.GetOffsetAndLengthAsync(token => GetAcrossProjectsTotalCount(organization, projects, token), cancellationToken);
            if (offsetAndLength.Length == 0) yield break;

            const int maxPageSize = 99;

            var limit = offsetAndLength.Offset + offsetAndLength.Length;
            var pageSize = Math.Min(maxPageSize, limit);
            request.AcrossProjectsSearchRequestParameters.Pagination.PageSize = pageSize;
            var startPage = offsetAndLength.Offset / pageSize;

            var firstPage = await AdvanceTokenToFirstPageAsync<TAsset>(request, startPage, cancellationToken);

            limit = Math.Min(limit, firstPage.Assets.Length);
            int index;
            for (index = offsetAndLength.Offset % pageSize; index < limit; ++index)
            {
                var asset = firstPage.Assets[index];

                yield return GetInitializedAsset(asset, projects.FirstOrDefault(p => p.Id == asset.SourceProjectId));
            }

            if (string.IsNullOrEmpty(firstPage.Token)) yield break;

            searchPagination.Token = firstPage.Token;
            pageSize = Math.Min(maxPageSize, offsetAndLength.Length);
            searchPagination.PageSize = pageSize;

            var lastIndex = offsetAndLength.Offset + offsetAndLength.Length;
            while (index <= lastIndex)
            {
                var response = await Client.PostAsync(request, ServiceHttpClientOptions.Default(), cancellationToken);
                var dto = IsolatedJsonConvert.DeserializeObject<AssetPageDto<TAsset>>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

                // To prevent an infinite loop, return if no assets were returned
                // or if the token is empty.
                if (dto.Assets.Length == 0) break;

                foreach (var asset in dto.Assets)
                {
                    if (++index < offsetAndLength.Offset) continue;
                    if (index > lastIndex) break;

                    yield return GetInitializedAsset(asset, projects.FirstOrDefault(p => p.Id == asset.SourceProjectId));
                }

                if (string.IsNullOrEmpty(dto.Token)) break;

                searchPagination.Token = dto.Token;
            }
        }

        public async Task<Aggregation> GetAssetAggregateAsync(IProject project, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken token)
        {
            var requestFilter = GetRequestFilter(assetSearchFilter);
            var requestParams = new SearchAndAggregateRequestParameters(requestFilter, parameters.AggregationField, parameters.ResultLimit);
            var request = new SearchAndAggregateRequest(project.Organization.GenesisId,
                project.Id,
                AssetPath,
                null,
                requestParams);

            var response = await Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);
            var aggregations = IsolatedJsonConvert.DeserializeObject<AggregationsDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType).Aggregations;

            var data = new Dictionary<string, int>();
            for (var i = 0; i < aggregations.Length; ++i)
            {
                data.TryAdd(aggregations[i].Value, aggregations[i].Count);
            }

            return new Aggregation(data);
        }

        /// <inheritdoc />
        public async Task<Aggregation> GetAssetAggregateAsync(IOrganization organization, IEnumerable<IProject> projects, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken token)
        {
            var requestFilter = GetRequestFilter(assetSearchFilter);
            var requestParams = new AcrossProjectsSearchAndAggregateRequestParameters(projects.Select(p => p.Id), requestFilter, parameters.AggregationField, parameters.ResultLimit);
            var request = new AcrossProjectsSearchAndAggregateRequest(organization.GenesisId,
                AssetPath,
                null,
                requestParams);

            var response = await Client.PostAsync(request, ServiceHttpClientOptions.Default(), token);
            var aggregations = IsolatedJsonConvert.DeserializeObject<AggregationsDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType).Aggregations;

            var data = new Dictionary<string, int>();
            for (var i = 0; i < aggregations.Length; ++i)
            {
                data.TryAdd(aggregations[i].Value, aggregations[i].Count);
            }

            return new Aggregation(data);
        }

        /// <inheritdoc />
        public async Task<IAsset> CreateAssetAsync(IProject project, IAssetCreation assetCreation, CancellationToken token)
        {
            var asset = assetCreation.MapFrom();
            var request = new CreateAssetRequest(project.Organization.GenesisId, project.Id, asset);
            var response = await m_ManagementClient.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            var createdAsset = IsolatedJsonConvert.DeserializeObject<CreatedAssetDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            asset = GetInitializedAsset(asset, project);
            asset.Id = createdAsset.AssetId;
            asset.Version = createdAsset.AssetVersion;
            asset.StorageId = createdAsset.StorageId;

            return asset;
        }

        /// <inheritdoc />
        public async Task<IAsset> UpdateAssetAsync(IProject project, IAsset asset, CancellationToken token)
        {
            var request = new UpdateAssetRequest(project.Organization.GenesisId, project.Id, asset);
            _ = await m_ManagementClient.PatchAsync(request, ServiceHttpClientOptions.Default(), token);

            return asset;
        }

        /// <inheritdoc />
        public async Task DeleteAssetAsync(IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var request = new DeleteAssetRequest(project.Organization.GenesisId, project.Id, assetId, assetVersion);
            _ = await m_ManagementClient.DeleteAsync(request, ServiceHttpClientOptions.Default(), token);
        }

        /// <inheritdoc />
        public async Task<IAsset> GetAssetDownloadUrlsAsync(IProject project, IAsset asset, CancellationToken token)
        {
            var request = new GetAssetDownloadUrlsRequest(project.Organization.GenesisId, project.Id, asset.Id, asset.Version);
            var response = await m_ManagementClient.GetAsync(request, ServiceHttpClientOptions.Default(), token);

            var assetDownloadUrlsDto = IsolatedJsonConvert.DeserializeObject<AssetDownloadUrlsDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            asset.OnFilesUpdated(
                assetDownloadUrlsDto.Files.ToArray(),
                assetDownloadUrlsDto.Attachments.ToArray());

            return GetInitializedAsset(asset);
        }

        /// <inheritdoc />
        public async Task<IAsset> GetAssetCollectionsAsync(IProject project, IAsset asset, CancellationToken token)
        {
            var request = new GetAssetCollectionsRequest(project.Organization.GenesisId, project.Id, asset.Id, asset.Version);
            var response = await m_ManagementClient.GetAsync(request, ServiceHttpClientOptions.Default(), token);

            var assetCollectionsDto = IsolatedJsonConvert.DeserializeObject<AssetCollectionListDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            asset.OnCollectionsUpdated(assetCollectionsDto.Collections);

            return asset;
        }

        /// <inheritdoc />
        public async Task LinkAnAssetToProjectAsync(IProject project, string assetId, int assetVersion, ulong destinationOrganizationId, string destinationProjectId, CancellationToken token)
        {
            var linkRequest = new LinkAssetToProjectRequest(project.Organization.GenesisId,
                project.Id,
                assetId,
                assetVersion,
                destinationOrganizationId,
                destinationProjectId);
            _ = await m_ManagementClient.PostAsync(linkRequest, ServiceHttpClientOptions.NoRetryOption(), token);
        }

        /// <inheritdoc />
        public async Task UnlinkAssetFromProjectAsync(IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var unlinkRequest = new UnlinkAssetFromProjectRequest(project.Organization.GenesisId,
                project.Id,
                assetId,
                assetVersion);
            _ = await m_ManagementClient.PostAsync(unlinkRequest, ServiceHttpClientOptions.NoRetryOption(), token);
        }

        /// <inheritdoc />
        public async Task<bool> CheckProjectIsAssetSourceProjectAsync(IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var checkRequest = new CheckProjectIsAssetSourceProjectRequest(project.Organization.GenesisId, project.Id, assetId, assetVersion);
            var response = await m_ManagementClient.GetAsync(checkRequest, ServiceHttpClientOptions.Default(), token);

            return bool.Parse(response);
        }

        /// <inheritdoc />
        public async Task<string> PublishApprovedAssetAsync(IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var request = new ChangeAssetStatusRequest(project.Organization.GenesisId, project.Id, assetId, assetVersion, ChangeAssetStatusAction.publish);
            var response = await m_ManagementClient.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            return response;
        }

        /// <inheritdoc />
        public async Task<string> WithdrawPublishedAssetAsync(IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var request = new ChangeAssetStatusRequest(project.Organization.GenesisId, project.Id, assetId, assetVersion, ChangeAssetStatusAction.withdraw);
            var response = await m_ManagementClient.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            return response;
        }

        /// <inheritdoc />
        public async Task<string> SendAssetToReviewAsync(IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var request = new ChangeAssetStatusRequest(project.Organization.GenesisId, project.Id, assetId, assetVersion, ChangeAssetStatusAction.review);
            var response = await m_ManagementClient.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            return response;
        }

        /// <inheritdoc />
        public async Task<string> ApproveAssetAsync(IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var request = new ChangeAssetStatusRequest(project.Organization.GenesisId, project.Id, assetId, assetVersion, ChangeAssetStatusAction.approve);
            var response = await m_ManagementClient.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            return response;
        }

        /// <inheritdoc />
        public async Task<string> RejectAssetAsync(IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var request = new ChangeAssetStatusRequest(project.Organization.GenesisId, project.Id, assetId, assetVersion, ChangeAssetStatusAction.reject);
            var response = await m_ManagementClient.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            return response;
        }

        static SearchRequestFilter GetRequestFilter(IAssetSearchFilter assetSearchFilter)
        {
            var anyQuery = assetSearchFilter.AccumulateAnyCriteria();

            return new SearchRequestFilter(assetSearchFilter.AccumulateIncludedCriteria(),
                assetSearchFilter.AccumulateExcludedCriteria(),
                anyQuery,
                anyQuery is {Count: > 0} ? assetSearchFilter.AnyQueryMinimumMatch : null);
        }

        static TAsset GetInitializedAsset<TAsset>(TAsset asset, IProject project) where TAsset : IAsset
        {
            asset.Project = project;

            return GetInitializedAsset(asset);
        }

        static TAsset GetInitializedAsset<TAsset>(TAsset asset) where TAsset : IAsset
        {
            foreach (var file in asset.Files)
            {
                file.AssetId = asset.Id;
                file.AssetVersion = asset.Version;
            }

            foreach (var file in asset.Attachments)
            {
                file.AssetId = asset.Id;
                file.AssetVersion = asset.Version;
            }

            return asset;
        }

        async Task<int> GetTotalCount(IProject project, CancellationToken cancellationToken)
        {
            var aggregation = await GetAssetAggregateAsync(project, new AssetSearchFilter(), new AggregationParameters(nameof(IAsset.Type)), cancellationToken);
            return aggregation.Total;
        }

        async Task<int> GetAcrossProjectsTotalCount(IOrganization organization, IEnumerable<IProject> projects, CancellationToken cancellationToken)
        {
            var aggregation = await GetAssetAggregateAsync(organization, projects, new AssetSearchFilter(), new AggregationParameters(nameof(IAsset.Type)), cancellationToken);
            return aggregation.Total;
        }

        async Task<AssetPageDto<TAsset>> AdvanceTokenToFirstPageAsync<TAsset>(SearchRequest request, int startPage, CancellationToken cancellationToken)
            where TAsset : IAsset, new()
        {
            var currentPage = 0;

            var response = await Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), cancellationToken);
            while (currentPage < startPage)
            {
                ++currentPage;
                var pageTokenDto = IsolatedJsonConvert.DeserializeObject<PageTokenDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);
                request.SearchRequestParameter.Pagination.Token = pageTokenDto.Token;
                response = await Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), cancellationToken);
            }

            return IsolatedJsonConvert.DeserializeObject<AssetPageDto<TAsset>>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);
        }

        async Task<AssetPageDto<TAsset>> AdvanceTokenToFirstPageAsync<TAsset>(AcrossProjectsSearchRequest request, int startPage, CancellationToken cancellationToken)
            where TAsset : IAsset, new()
        {
            var currentPage = 0;

            var response = await Client.PostAsync(request, ServiceHttpClientOptions.Default(), cancellationToken);
            while (currentPage < startPage)
            {
                ++currentPage;
                var pageTokenDto = IsolatedJsonConvert.DeserializeObject<PageTokenDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);
                request.AcrossProjectsSearchRequestParameters.Pagination.Token = pageTokenDto.Token;
                response = await Client.PostAsync(request, ServiceHttpClientOptions.Default(), cancellationToken);
            }

            return IsolatedJsonConvert.DeserializeObject<AssetPageDto<TAsset>>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);
        }
    }
}

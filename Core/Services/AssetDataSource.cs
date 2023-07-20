using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class AssetDataSource : IAssetDataSource
    {
        readonly IAssetHttpClient m_Client;
        readonly string m_AssetPath;

        internal AssetDataSource(IAssetHttpClient client, string assetPath = "")
        {
            m_Client = client;
            m_AssetPath = assetPath;
        }

        /// <inheritdoc/>
        public async Task<TAsset> GetAssetAsync<TAsset>(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
            where TAsset : IAsset, new()
        {
            var request = new GetAssetByIdAndVersionRequest(organization.GenesisId, project.Id, assetId, assetVersion);
            var response = await m_Client.GetAsync(request, ServiceHttpClientOptions.Default(), token);

            var asset = IsolatedJsonConvert.DeserializeObject<TAsset>(response, new JsonAssetConverter());
            InitializeAsset(asset);
            return asset;
        }

        public async Task<IAssetPage> GetAssetPageAsync<TAsset>(IOrganization organization, IProject project, IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken token)
            where TAsset : IAsset, new()
        {
            var requestFilter = GetRequestFilter(assetSearchFilter);
            var searchPagination = new SearchRequestPagination(pagination.SortingField, pageSize: pagination.PageSize);

            // Still missing definitions for optional params:
            // - SearchRequestResultFields resultFields
            // - bool includeThumbnailDownloadURLs
            var requestParams = new SearchRequestParameters(requestFilter, pagination: searchPagination);

            // Still missing definitions for optional params:
            // - string xCorrelationId
            var request = new SearchRequest(organization.GenesisId,
                project.Id,
                m_AssetPath,
                null,
                requestParams);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);
            var assetPageDto = IsolatedJsonConvert.DeserializeObject<AssetPageDto<TAsset>>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            // Convert to IAsset array and return a constructed page.
            var assets = new IAsset[assetPageDto.Assets.Length];
            for (var i = 0; i < assets.Length; ++i)
            {
                assets[i] = assetPageDto.Assets[i];
                assets[i].Organization = organization;
                assets[i].Project = project;
                InitializeAsset(assets[i]);
            }

            return new CloudAssetPage(this, organization, project, assets, assetPageDto.Token, pagination);
        }

        /// <inheritdoc/>
        public async Task<IAssetPage> GetNextAssetPageAsync<TAsset>(IAssetPage assetPage, CancellationToken token)
            where TAsset : IAsset, new()
        {
            var searchPagination = new SearchRequestPagination(
                assetPage.Pagination.SortingField,
                assetPage.NextPageToken,
                assetPage.Pagination.PageSize);

            var requestParams = new SearchRequestParameters(pagination: searchPagination);
            var request = new SearchRequest(assetPage.Organization.GenesisId,
                assetPage.Project.Id,
                m_AssetPath,
                null,
                requestParams);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);
            var assetPageDto = IsolatedJsonConvert.DeserializeObject<AssetPageDto<TAsset>>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            // Convert to IAsset array
            var assets = new IAsset[assetPageDto.Assets.Length];
            for (var i = 0; i < assets.Length; ++i)
            {
                assets[i] = assetPageDto.Assets[i];
                assets[i].Organization = assetPage.Organization;
                assets[i].Project = assetPage.Project;
                InitializeAsset(assets[i]);
            }

            return new CloudAssetPage(this, assets, assetPageDto.Token, assetPage);
        }

        public async Task<Aggregation> GetAssetAggregateAsync(IOrganization organization, IProject project, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken token)
        {
            var requestFilter = GetRequestFilter(assetSearchFilter);
            var requestParams = new SearchAndAggregateRequestParameters(requestFilter, parameters.AggregationField, parameters.ResultLimit);
            var request = new SearchAndAggregateRequest(organization.GenesisId,
                project.Id,
                m_AssetPath,
                null,
                requestParams);

            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);
            var aggregations = IsolatedJsonConvert.DeserializeObject<AggregationsDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType).Aggregations;

            var data = new Dictionary<string, int>();
            for (var i = 0; i < aggregations.Length; ++i)
            {
                data.TryAdd(aggregations[i].Value, aggregations[i].Count);
            }

            return new Aggregation(data);
        }

        /// <inheritdoc />
        public async Task<IAsset> CreateAssetAsync(IOrganization organization, IProject project, IAssetCreation assetCreation, CancellationToken token)
        {
            var asset = assetCreation.MapFrom();
            var request = new CreateAssetRequest(organization.GenesisId, project.Id, asset);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            var createdAsset = IsolatedJsonConvert.DeserializeObject<CreatedAssetDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            asset.Organization = organization;
            asset.Project = project;
            asset.Id = createdAsset.AssetId;
            asset.Version = createdAsset.AssetVersion;
            asset.StorageId = createdAsset.StorageId;

            return asset;
        }

        /// <inheritdoc />
        public async Task<IAsset> UpdateAssetAsync(IOrganization organization, IProject project, IAsset asset, CancellationToken token)
        {
            var request = new UpdateAssetRequest(organization.GenesisId, project.Id, asset);
            _ = await m_Client.PatchAsync(request, ServiceHttpClientOptions.Default(), token);

            return asset;
        }

        /// <inheritdoc />
        public async Task DeleteAssetAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var request = new DeleteAssetRequest(organization.GenesisId, project.Id, assetId, assetVersion);
            _ = await m_Client.DeleteAsync(request, ServiceHttpClientOptions.Default(), token);
        }

        /// <inheritdoc />
        public async Task<IAsset> GetAssetDownloadUrlsAsync(IOrganization organization, IProject project, IAsset asset, CancellationToken token)
        {
            var request = new GetAssetDownloadUrlsRequest(organization.GenesisId, project.Id, asset.Id, asset.Version);
            var response = await m_Client.GetAsync(request, ServiceHttpClientOptions.Default(), token);

            var assetDownloadUrlsDto = IsolatedJsonConvert.DeserializeObject<AssetDownloadUrlsDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            asset.OnFilesUpdated(
                assetDownloadUrlsDto.Files.ToArray(),
                assetDownloadUrlsDto.Attachments.ToArray());

            InitializeAsset(asset);

            return asset;
        }

        /// <inheritdoc />
        public async Task<IAsset> GetAssetCollectionsAsync(IOrganization organization, IProject project, IAsset asset, CancellationToken token)
        {
            var request = new GetAssetCollectionsRequest(organization.GenesisId, project.Id, asset.Id, asset.Version);
            var response = await m_Client.GetAsync(request, ServiceHttpClientOptions.Default(), token);

            var assetCollectionsDto = IsolatedJsonConvert.DeserializeObject<AssetCollectionListDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            asset.OnCollectionsUpdated(assetCollectionsDto.Collections);

            return asset;
        }

        /// <inheritdoc />
        public async Task LinkAnAssetToProjectAsync(IOrganization organization, IProject project, string assetId, int assetVersion, ulong destinationOrganizationId, string destinationProjectId, CancellationToken token)
        {
            var linkRequest = new LinkAssetToProjectRequest(organization.GenesisId, project.Id, assetId, assetVersion, destinationOrganizationId, destinationProjectId);
            _ = await m_Client.PostAsync(linkRequest, ServiceHttpClientOptions.NoRetryOption(), token);
        }

        /// <inheritdoc />
        public async Task UnlinkAssetFromProjectAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var unlinkRequest = new UnlinkAssetFromProjectRequest(organization.GenesisId, project.Id, assetId, assetVersion);
            _ = await m_Client.PostAsync(unlinkRequest, ServiceHttpClientOptions.NoRetryOption(), token);
        }

        /// <inheritdoc />
        public async Task<bool> CheckProjectIsAssetSourceProjectAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var checkRequest = new CheckProjectIsAssetSourceProjectRequest(organization.GenesisId, project.Id, assetId, assetVersion);
            var response = await m_Client.GetAsync(checkRequest, ServiceHttpClientOptions.Default(), token);

            return bool.Parse(response);
        }

        /// <inheritdoc />
        public async Task<string> PublishApprovedAssetAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var request = new ChangeAssetStatusRequest(organization.GenesisId, project.Id, assetId, assetVersion, ChangeAssetStatusAction.publish);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            return response;
        }

        /// <inheritdoc />
        public async Task<string> WithdrawPublishedAssetAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var request = new ChangeAssetStatusRequest(organization.GenesisId, project.Id, assetId, assetVersion, ChangeAssetStatusAction.withdraw);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            return response;
        }

        /// <inheritdoc />
        public async Task<string> SendAssetToReviewAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var request = new ChangeAssetStatusRequest(organization.GenesisId, project.Id, assetId, assetVersion, ChangeAssetStatusAction.review);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            return response;
        }

        /// <inheritdoc />
        public async Task<string> ApproveAssetAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var request = new ChangeAssetStatusRequest(organization.GenesisId, project.Id, assetId, assetVersion, ChangeAssetStatusAction.approve);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            return response;
        }

        /// <inheritdoc />
        public async Task<string> RejectAssetAsync(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken token)
        {
            var request = new ChangeAssetStatusRequest(organization.GenesisId, project.Id, assetId, assetVersion, ChangeAssetStatusAction.reject);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

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

        static void InitializeAsset(IAsset asset)
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
        }
    }
}

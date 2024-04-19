using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class AssetDataSource
    {
        /// <inheritdoc/>
        public async IAsyncEnumerable<IVersionLabelData> ListVersionLabelsAsync(OrganizationId organizationId, PaginationData pagination, bool? archived, bool? systemLabels, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const int maxPageSize = 1000;

            var countRequest = new GetVersionLabelListRequest(organizationId, 0, 1, archived, systemLabels);
            var (offset, length) = await pagination.Range.GetOffsetAndLengthAsync(token => GetTotalCount(countRequest, token), cancellationToken);
            var pageSize = Math.Min(maxPageSize, Math.Max(offset, length));

            var count = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = new GetVersionLabelListRequest(organizationId, offset, pageSize, archived, systemLabels);
                var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

                var jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var pageDto = IsolatedSerialization.DeserializeWithDefaultConverters<VersionLabelListDto>(jsonContent);

                if (pageDto.Versionlabels == null || pageDto.Versionlabels.Length == 0) break;

                for (var i = 0; i < pageDto.Versionlabels.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (count >= length) break;

                    ++count;
                    yield return pageDto.Versionlabels[i];
                }

                // Cap the length to the total number of entries.
                length = Math.Min(length, pageDto.Total);
                // Update the offset and page size for the next iteration
                offset += pageSize;
                pageSize = Math.Min(pageSize, length - offset);
            } while (count < length);
        }

        /// <inheritdoc/>
        public async Task<IVersionLabelData> GetVersionLabelAsync(VersionLabelDescriptor versionLabelDescriptor, CancellationToken cancellationToken)
        {
            // Not yet implemented in backend, we need to pass through search all API
            /*
            cancellationToken.ThrowIfCancellationRequested();

            var request = new VersionLabelRequest(versionLabelDescriptor.OrganizationId, versionLabelDescriptor.LabelName);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            return JsonSerialization.Deserialize<VersionLabelData>(jsonContent);
            */

            var results = ListVersionLabelsAsync(versionLabelDescriptor.OrganizationId, new PaginationData {Range = Range.All}, null, null, cancellationToken);
            await foreach (var result in results.WithCancellation(cancellationToken))
            {
                if (result.Name == versionLabelDescriptor.LabelName)
                {
                    return result;
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<IVersionLabelData> CreateVersionLabelAsync(OrganizationId organizationId, IVersionLabelBaseData versionLabelCreation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CreateVersionLabelRequest(organizationId, versionLabelCreation);
            var response = await m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var createdLabel = JsonSerialization.Deserialize<CreatedLabelDto>(jsonContent);
            if (createdLabel.Name != Uri.EscapeDataString(versionLabelCreation.Name))
            {
                k_Logger.LogWarning($"The created label name '{createdLabel.Name}' does not match the requested label name '{versionLabelCreation.Name}' when URL escaped as '{Uri.EscapeDataString(versionLabelCreation.Name)}'.");
            }

            return new VersionLabelData
            {
                Name = versionLabelCreation.Name,
                Description = versionLabelCreation.Description,
                DisplayColor = versionLabelCreation.DisplayColor
            };
        }

        /// <inheritdoc/>
        public Task UpdateVersionLabelAsync(VersionLabelDescriptor versionLabelDescriptor, IVersionLabelBaseData versionlabelUpdate, CancellationToken cancellationToken)
        {
            var request = new VersionLabelRequest(versionLabelDescriptor.OrganizationId, versionLabelDescriptor.LabelName, versionlabelUpdate);
            return m_ServiceHttpClient.PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc/>
        public Task UpdateVersionLabelStatusAsync(VersionLabelDescriptor versionLabelDescriptor, bool archive, CancellationToken cancellationToken)
        {
            var request = new UpdateVersionLabelStatusRequest(versionLabelDescriptor.OrganizationId, versionLabelDescriptor.LabelName, archive);
            return m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<AssetVersionLabelsDto> ListLabelsAcrossAssetVersions(ProjectDescriptor projectDescriptor, AssetId assetId, PaginationData pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const int maxPageSize = 1000;

            var countRequest = new AssetVersionLabelRequest(projectDescriptor.ProjectId, assetId, 0, 1);
            var (offset, length) = await pagination.Range.GetOffsetAndLengthAsync(token => GetTotalCount(countRequest, token), cancellationToken);
            var pageSize = Math.Min(maxPageSize, Math.Max(offset, length));

            var count = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = new AssetVersionLabelRequest(projectDescriptor.ProjectId, assetId, offset, pageSize);
                var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

                var jsonContent = await response.GetContentAsString();
                cancellationToken.ThrowIfCancellationRequested();

                var pageDto = IsolatedSerialization.DeserializeWithDefaultConverters<AssetVersionLabelListDto>(jsonContent);

                if (pageDto.AssetVersionLabels == null || pageDto.AssetVersionLabels.Length == 0) break;

                for (var i = 0; i < pageDto.AssetVersionLabels.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (count >= length) break;

                    ++count;
                    yield return pageDto.AssetVersionLabels[i];
                }

                // Cap the length to the total number of entries.
                length = Math.Min(length, pageDto.Total);
                // Update the offset and page size for the next iteration
                offset += pageSize;
                pageSize = Math.Min(pageSize, length - offset);
            } while (count < length);
        }

        /// <inheritdoc/>
        public Task AssignVersionLabelsAsync(AssetDescriptor assetDescriptor, IEnumerable<string> versionLabels, CancellationToken cancellationToken)
        {
            var request = new AssignVersionLabelRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, true, versionLabels);
            return m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc/>
        public Task UnassignVersionLabelsAsync(AssetDescriptor assetDescriptor, IEnumerable<string> versionLabels, CancellationToken cancellationToken)
        {
            var request = new AssignVersionLabelRequest(assetDescriptor.ProjectId, assetDescriptor.AssetId, assetDescriptor.AssetVersion, false, versionLabels);
            return m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(), ServiceHttpClientOptions.Default(), cancellationToken);
        }

        async Task<int> GetTotalCount(ApiRequest apiRequest, CancellationToken cancellationToken)
        {
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(apiRequest), ServiceHttpClientOptions.Default(), cancellationToken);
            var jsonContent = await response.GetContentAsString();
            var pageDto = IsolatedSerialization.Deserialize<PaginationDto>(jsonContent, IsolatedSerialization.defaultSettings);
            return pageDto.Total;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class AssetDataSource
    {
        /// <inheritdoc/>
        public async IAsyncEnumerable<IProjectData> ListProjectsAsync(OrganizationId organizationId, PaginationData pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const int maxPageSize = 99;

            var (offset, length) = await pagination.Range.GetOffsetAndLengthAsync(_cancellationToken =>
            {
                _cancellationToken.ThrowIfCancellationRequested();
                return Task.FromResult(int.MaxValue);
            }, cancellationToken);

            if (length == 0) yield break;

            var pageSize = Math.Min(maxPageSize, Math.Max(offset, length));
            var pageNumber = offset / pageSize + 1;

            var startIndex = offset % pageSize;
            var count = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var request = new ListProjectsRequest(organizationId, pageNumber, pageSize);
                var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                    cancellationToken);

                var jsonContent = await response.GetContentAsString();

                cancellationToken.ThrowIfCancellationRequested();

                var projectPageDto = IsolatedSerialization.DeserializeWithDefaultConverters<ProjectPageDto>(jsonContent);

                ++pageNumber;

                if (projectPageDto.Projects == null || projectPageDto.Projects.Length == 0) break;

                for (var i = 0; i < projectPageDto.Projects.Length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (count == 0 && i < startIndex) continue;
                    if (count >= length) break;

                    ++count;
                    yield return projectPageDto.Projects[i];
                }
            } while (count < length);
        }

        /// <inheritdoc/>
        public async Task<IProjectData> GetProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new ProjectRequest(projectDescriptor.ProjectId);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);

            var jsonContent = await response.GetContentAsString();

            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<ProjectData>(jsonContent);
        }

        /// <inheritdoc/>
        public async Task<IProjectData> CreateProjectAsync(OrganizationId organizationId, IProjectBaseData projectCreation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CreateProjectRequest(organizationId, projectCreation);
            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();

            cancellationToken.ThrowIfCancellationRequested();

            var projectDto = IsolatedSerialization.DeserializeWithDefaultConverters<CreatedProjectDto>(jsonContent);

            return new ProjectData(projectDto.Id)
            {
                Name = projectCreation.Name,
                Metadata = projectCreation.Metadata
            };
        }
    }
}

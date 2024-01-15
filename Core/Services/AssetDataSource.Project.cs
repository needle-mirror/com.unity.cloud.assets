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
        public async IAsyncEnumerable<IProjectData> ListProjectsAsync(OrganizationId organizationId, Pagination pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            const int maxPageSize = 99;

            var (offset, length) = await pagination.Range.GetOffsetAndLengthAsync(_ => Task.FromResult(int.MaxValue), cancellationToken);
            var pageSize = Math.Min(maxPageSize, Math.Max(offset, length));
            var pageNumber = offset / pageSize + 1;

            var startIndex = offset % pageSize;
            var count = 0;
            do
            {
                var request = new ListProjectsRequest(organizationId,pageNumber, pageSize);
                var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                    cancellationToken);
                var jsonContent = await response.GetContentAsString();
                var projectPageDto = IsolatedSerialization.DeserializeWithDefaultConverters<ProjectPageDto>(jsonContent);

                ++pageNumber;

                if (projectPageDto.Projects == null || projectPageDto.Projects.Length == 0) break;

                for (var i = 0; i < projectPageDto.Projects.Length; ++i)
                {
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
            var request = new ProjectRequest(projectDescriptor.ProjectId);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(),
                cancellationToken);
            var jsonContent = await response.GetContentAsString();

            return IsolatedSerialization.DeserializeWithDefaultConverters<ProjectData>(jsonContent);
        }

        /// <inheritdoc/>
        public async Task<IProjectData> CreateProjectAsync(OrganizationId organizationId, IProjectBaseData projectCreation, CancellationToken cancellationToken)
        {
            var request = new CreateProjectRequest(organizationId, projectCreation);
            var response = await m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
            var jsonContent = await response.GetContentAsString();

            var projectDto = IsolatedSerialization.DeserializeWithDefaultConverters<CreatedProjectDto>(jsonContent);

            return new ProjectData(projectDto.Id)
            {
                Name = projectCreation.Name,
                Metadata = projectCreation.Metadata
            };
        }
    }
}

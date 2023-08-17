using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class ProjectDataSource : IProjectDataSource
    {
        readonly IAssetHttpClient m_Client;

        internal ProjectDataSource(IServiceHttpClient serviceHttpClient, string serviceAddress)
            : this(new AssetHttpClient(serviceHttpClient, serviceAddress)) { }

        internal ProjectDataSource(IAssetHttpClient client)
        {
            m_Client = client;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<IProject> ListProjectsAsync(IOrganization organization,
            string userId,
            Pagination pagination,
            [EnumeratorCancellation] CancellationToken cancellationToken,
            bool enrichWithUsersCount = false, string xCorrelationId = null)
        {
            const int maxPageSize = 99;

            var offsetAndLength = await pagination.Range.GetOffsetAndLengthAsync(_ => Task.FromResult(int.MaxValue), cancellationToken);
            var pageSize = Math.Min(maxPageSize, Math.Max(offsetAndLength.Offset, offsetAndLength.Length));
            var pageNumber = offsetAndLength.Offset / pageSize + 1;

            var startIndex = offsetAndLength.Offset % pageSize;
            var count = 0;
            do
            {
                var request = new GetProjectsByOrganizationAndUserIdsRequest(organization.GenesisId, userId, pageNumber, pageSize, enrichWithUsersCount, xCorrelationId);
                var response = await m_Client.GetAsync(request, ServiceHttpClientOptions.Default(), cancellationToken);
                var projectPageDto = IsolatedJsonConvert.DeserializeObject<ProjectPageDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

                ++pageNumber;

                if (projectPageDto.Projects == null || projectPageDto.Projects.Length == 0) break;

                for (var i = 0; i < projectPageDto.Projects.Length; ++i)
                {
                    if (count == 0 && i < startIndex) continue;

                    ++count;
                    yield return GetInitializedProject(projectPageDto.Projects[i], organization);
                }
            } while (count < offsetAndLength.Length);
        }

        static IProject GetInitializedProject(AssetProject project, IOrganization organization)
        {
            project.Initialize(organization);
            return project;
        }
    }
}

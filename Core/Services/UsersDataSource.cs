using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class UsersDataSource : IUsersDataSource
    {
        readonly IAssetHttpClient m_Client;

        internal UsersDataSource(IServiceHttpClient serviceHttpClient, string serviceAddress)
            : this(new AssetHttpClient(serviceHttpClient, serviceAddress)) { }

        internal UsersDataSource(IAssetHttpClient client)
        {
            m_Client = client;
        }

        /// <inheritdoc/>
        public async Task<(IUser user, IOrganization[] organizations)> GetUserOrganizationsAsync(string userId, CancellationToken token)
        {
            var request = new UserAndOrganizationsRequest(userId);
            var response = await m_Client.GetAsync(request, ServiceHttpClientOptions.Default(), token);

            var userAndOrganizationsDto = IsolatedJsonConvert.DeserializeObject<UserAndOrganizationsDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);
            return CopyFrom(userAndOrganizationsDto);
        }

        /// <inheritdoc/>
        public async Task<IProjectPage> ListProjectsAsync(IOrganization organization, string userId,
            Pagination pagination,
            CancellationToken token,
            bool enrichWithUsersCount = false, string xCorrelationId = null)
        {
            var request = new GetProjectsByOrganizationAndUserIdsRequest(organization.GenesisId, userId, pagination.PageNumber, pagination.PageSize, enrichWithUsersCount, xCorrelationId);
            var response = await m_Client.GetAsync(request, ServiceHttpClientOptions.Default(), token);
            var projectPageDto = IsolatedJsonConvert.DeserializeObject<ProjectPageDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            return new ProjectPage(this, organization, userId, pagination, projectPageDto.MapFrom());
        }

        static (IUser, IOrganization[]) CopyFrom(UserAndOrganizationsDto dto)
        {
            var user = new User
            {
                Id = dto.Id,
                GenesisId = dto.GenesisId,
                Name = dto.Name,
                Email = dto.Email
            };

            var orgs = new List<IOrganization>();
            foreach (var org in dto.Organizations)
            {
                org.Initialize();
                orgs.Add(org);
            }

            return (user, orgs.ToArray());
        }
    }
}

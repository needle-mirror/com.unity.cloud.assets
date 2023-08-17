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

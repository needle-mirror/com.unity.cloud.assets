using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class GetProjectsByOrganizationAndUserIdsRequest : ApiRequest
    {
        /// <summary>
        /// ApiAssetsUsersV1UserIdOrganizationOrganizationIdProjectsGet Request Object.
        /// Reads a list of projects in an org that a user has access to.
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        public GetProjectsByOrganizationAndUserIdsRequest(OrganizationId organizationId,
            string userId = null,
            int? page = default,
            int? pageSize = default)
        {
            userId = string.IsNullOrEmpty(userId) ? "me" : userId;

            m_PathAndQueryParams = $"/users/{userId}/organizations/{organizationId}/projects";

            AddParamToQueryParams("Page", page.ToString());
            AddParamToQueryParams("Limit", pageSize.ToString());
        }
    }
}

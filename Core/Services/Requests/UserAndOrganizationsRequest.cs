namespace Unity.Cloud.Assets
{
    class UserAndOrganizationsRequest : ApiRequest
    {
        /// <summary>
        /// Request object for fetching a user and its associated organizations.
        /// </summary>
        /// <param name="userId"></param>
        public UserAndOrganizationsRequest(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = "me";
            }

            // https://services.docs.internal.unity3d.com/unity-services-gateway/api-documentation/routes/unity/v1/#operation/unity-getUserOrganizations
            m_PathAndQueryParams = $"/api/unity/v1/users/{userId}/organizations";
        }

        /// <inheritdoc/>
        public override string ConstructUrl(string requestBasePath)
        {
            // User and org info comes from a different end point than AMC, so we don't use the base path in the url
            return m_PathAndQueryParams;
        }
    }
}

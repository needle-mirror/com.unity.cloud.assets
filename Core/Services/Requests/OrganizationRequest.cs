using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Base class for api requests on assets.
    /// </summary>
    class OrganizationRequest : ApiRequest
    {
        /// <summary>
        /// AssetRequest Request Object.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        public OrganizationRequest(OrganizationId organizationId)
        {
            m_RequestUrl = $"/organizations/{organizationId}";
        }
    }
}

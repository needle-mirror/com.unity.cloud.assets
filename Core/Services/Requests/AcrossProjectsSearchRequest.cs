using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// AcrossProjectsSearchRequest
    /// Across projects search assets based on criteria.
    /// </summary>
    class AcrossProjectsSearchRequest : OrganizationRequest
    {
        /// <summary>Accessor for AcrossProjectsSearchRequestParameters </summary>
        public AcrossProjectsSearchRequestParameters Parameters { get; }

        /// <summary>
        /// Across projects search Request Object.
        /// Across projects search assets based on criteria.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="parameters">The search asset request criteria.</param>
        public AcrossProjectsSearchRequest(OrganizationId organizationId,
            AcrossProjectsSearchRequestParameters parameters = default)
            : base(organizationId)
        {
            m_PathAndQueryParams += $"/assets/search";

            Parameters = parameters;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.SerializeWithDefaultConverters(Parameters);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

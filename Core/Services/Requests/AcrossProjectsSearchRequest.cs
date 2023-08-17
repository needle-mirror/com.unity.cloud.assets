using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// AcrossProjectsSearchRequest
    /// Across projects search assets based on criteria.
    /// </summary>
    internal class AcrossProjectsSearchRequest : AssetRequest
    {
        /// <summary>Accessor for AcrossProjectsSearchRequestParameters </summary>
        public AcrossProjectsSearchRequestParameters AcrossProjectsSearchRequestParameters { get; }

        /// <summary>
        /// Across projects search Request Object.
        /// Across projects search assets based on criteria.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        /// <param name="acrossProjectsSearchRequestParameters">The search asset request criteria.</param>
        public AcrossProjectsSearchRequest(ulong organizationId,
            string assetPath,
            string xCorrelationId = default(string),
            AcrossProjectsSearchRequestParameters acrossProjectsSearchRequestParameters = default)
            : base(organizationId, null, xCorrelationId)
        {
            m_PathAndQueryParams = $"/organizations/{OrganizationId}{assetPath}/search";

            AcrossProjectsSearchRequestParameters = acrossProjectsSearchRequestParameters;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(AcrossProjectsSearchRequestParameters);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

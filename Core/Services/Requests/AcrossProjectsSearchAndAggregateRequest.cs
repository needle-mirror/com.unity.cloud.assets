using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// AcrossProjectsSearchAndAggregateRequest
    /// Aggregations of assets across projects that match a criteria by a defined field.
    /// </summary>
    internal class AcrossProjectsSearchAndAggregateRequest : AssetRequest
    {
        /// <summary>Accessor for CrossProjectsSearchAndAggregateRequestParameters </summary>
        public AcrossProjectsSearchAndAggregateRequestParameters AcrossProjectsSearchAndAggregateRequestParameters { get; }

        /// <summary>
        /// Search Request Object.
        /// Search assets based on criteria.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        /// <param name="acrossProjectsSearchAndAggregateRequestParameters">The search asset request criteria.</param>
        public AcrossProjectsSearchAndAggregateRequest(ulong organizationId,
            string assetPath,
            string xCorrelationId = default(string),
            AcrossProjectsSearchAndAggregateRequestParameters acrossProjectsSearchAndAggregateRequestParameters = default(AcrossProjectsSearchAndAggregateRequestParameters))
            : base(organizationId, null, xCorrelationId)
        {
            m_PathAndQueryParams = $"/organizations/{OrganizationId}{assetPath}/aggregations/search";

            AcrossProjectsSearchAndAggregateRequestParameters = acrossProjectsSearchAndAggregateRequestParameters;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(AcrossProjectsSearchAndAggregateRequestParameters);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

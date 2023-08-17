using System;
using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// SearchAndAggregateRequest
    /// Aggregations of assets that match a criteria by a defined field.
    /// </summary>
    internal class SearchAndAggregateRequest : AssetRequest
    {
        /// <summary>Accessor for searchAndAggregateRequestParameter </summary>
        public SearchAndAggregateRequestParameters SearchAndAggregateRequestParameter { get; }

        /// <summary>
        /// SearchAndAggregate Request Object.
        /// Aggregations of assets that match a criteria by a defined field.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="projectId">ID of the project</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        /// <param name="searchAndAggregateRequestParameter">The request containing the read filter and the field to be used in the aggregation..</param>
        public SearchAndAggregateRequest(ulong organizationId,
            string projectId,
            string assetPath,
            string xCorrelationId = default(string),
            SearchAndAggregateRequestParameters searchAndAggregateRequestParameter = default(SearchAndAggregateRequestParameters))
            : base(organizationId, projectId, xCorrelationId)
        {
            m_PathAndQueryParams += $"{assetPath}/aggregations/search";

            SearchAndAggregateRequestParameter = searchAndAggregateRequestParameter;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A list of IMultipartFormSection representing the request body.</returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(SearchAndAggregateRequestParameter);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

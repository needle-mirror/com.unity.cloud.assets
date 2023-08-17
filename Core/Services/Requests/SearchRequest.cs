using System;
using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// SearchRequest
    /// Search assets based on criteria.
    /// </summary>
    internal class SearchRequest : AssetRequest
    {
        /// <summary>Accessor for searchRequestParameter </summary>
        public SearchRequestParameters SearchRequestParameter { get; }

        /// <summary>
        /// Search Request Object.
        /// Search assets based on criteria.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="projectId">ID of the project</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        /// <param name="searchRequestParameter">The search asset request criteria.</param>
        public SearchRequest(ulong organizationId,
            string projectId,
            string assetPath,
            string xCorrelationId = default(string),
            SearchRequestParameters searchRequestParameter = default(SearchRequestParameters))
            : base(organizationId, projectId, xCorrelationId)
        {
            m_PathAndQueryParams += $"{assetPath}/search";

            SearchRequestParameter = searchRequestParameter;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(SearchRequestParameter);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

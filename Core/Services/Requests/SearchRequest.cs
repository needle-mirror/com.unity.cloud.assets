using System;
using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// SearchRequest
    /// Search assets based on criteria.
    /// </summary>
    class SearchRequest : ProjectRequest
    {
        /// <summary>Accessor for searchRequestParameter </summary>
        public SearchRequestParameters SearchRequestParameter { get; }

        /// <summary>
        /// Search Request Object.
        /// Search assets based on criteria.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        /// <param name="searchRequestParameter">The search asset request criteria.</param>
        public SearchRequest(ProjectId projectId, SearchRequestParameters searchRequestParameter = default, string xCorrelationId = default)
            : base(projectId, xCorrelationId)
        {
            m_PathAndQueryParams += $"/assets/search";

            SearchRequestParameter = searchRequestParameter;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(SearchRequestParameter, SerializationUtilities.Converters);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

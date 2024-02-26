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
        public SearchRequestParameters Parameters { get; }

        /// <summary>
        /// Search Request Object.
        /// Search assets based on criteria.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="parameters">The search asset request criteria.</param>
        public SearchRequest(ProjectId projectId, SearchRequestParameters parameters = default)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/search";

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

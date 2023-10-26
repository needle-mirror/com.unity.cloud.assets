using System;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Builds an API request that references a collection by path.
    /// </summary>
    class CollectionRequest : ProjectRequest
    {
        /// <summary>
        /// Initializes and returns an API request for a collection.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="collectionPath">The path to the collection</param>
        /// <param name="xCorrelationId">Correlation id of the request</param>
        public CollectionRequest(ProjectId projectId, CollectionPath collectionPath, string xCorrelationId = default)
            : base(projectId, xCorrelationId)
        {
            m_PathAndQueryParams += $"/collections/{Uri.EscapeDataString(collectionPath)}";
        }
    }
}

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Builds an API request that references a collection by path.
    /// </summary>
    class CollectionRequest : AssetRequest
    {
        /// <summary>
        /// Initializes and returns an API request for a collection.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="projectId">ID of the project</param>
        /// <param name="collectionPath">The path to the collection</param>
        /// <param name="xCorrelationId">Correlation id of the request</param>
        public CollectionRequest(ulong organizationId, string projectId, string collectionPath, string xCorrelationId = default(string))
            : base(organizationId, projectId, xCorrelationId)
        {
            m_PathAndQueryParams += $"/collections/{collectionPath}";
        }
    }
}

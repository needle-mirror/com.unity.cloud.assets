namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Get the list of collections from a project.
    /// </summary>
    class GetCollectionListRequest : AssetRequest
    {
        /// <summary>
        /// Initializes an API request to get the list of collections from a project.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="projectId">ID of the project</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public GetCollectionListRequest(ulong organizationId, string projectId, string xCorrelationId = default(string))
            : base(organizationId, projectId, xCorrelationId)
        {
            m_PathAndQueryParams += "/collections";
        }
    }
}

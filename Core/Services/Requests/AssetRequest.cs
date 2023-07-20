namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Base class for api requests on assets.
    /// </summary>
    abstract class AssetRequest : ApiRequest
    {
        /// <summary>Accessor for organizationId </summary>
        public ulong OrganizationId { get; }

        /// <summary>Accessor for projectId </summary>
        public string ProjectId { get; }

        /// <summary>Accessor for xCorrelationId </summary>
        public string XCorrelationId { get; }

        /// <summary>
        /// AssetRequest Request Object.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="projectId">ID of the project</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        protected AssetRequest(ulong organizationId, string projectId, string xCorrelationId = default(string))
        {
            OrganizationId = organizationId;
            ProjectId = projectId;

            m_PathAndQueryParams = $"/organizations/{OrganizationId}/projects/{ProjectId}";

            XCorrelationId = xCorrelationId;
        }
    }
}

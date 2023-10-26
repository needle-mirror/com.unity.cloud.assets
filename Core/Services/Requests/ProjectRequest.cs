using System.Collections.Generic;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Base class for api requests on assets.
    /// </summary>
    class ProjectRequest : ApiRequest
    {
        /// <summary>Accessor for X-Correlation-Id </summary>
        readonly string m_xCorrelationId;

        /// <summary>
        /// AssetRequest Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public ProjectRequest(ProjectId projectId, string xCorrelationId = default)
        {
            m_PathAndQueryParams = $"/projects/{projectId}";

            m_xCorrelationId = xCorrelationId;
        }

        public override IEnumerable<(string, string)> GetHeaders()
        {
            if (!string.IsNullOrEmpty(m_xCorrelationId))
            {
                yield return ("X-Correlation-Id", m_xCorrelationId);
            }
        }
    }
}

using System.Collections.Generic;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Base class for api requests on assets.
    /// </summary>
    class OrganizationRequest : ApiRequest
    {
        /// <summary>Accessor for X-Correlation-Id </summary>
        readonly string m_xCorrelationId;

        /// <summary>
        /// AssetRequest Request Object.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public OrganizationRequest(OrganizationId organizationId, string xCorrelationId = default)
        {
            m_PathAndQueryParams = $"/organizations/{organizationId}";

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

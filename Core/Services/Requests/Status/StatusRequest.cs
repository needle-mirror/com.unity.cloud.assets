using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class StatusRequest : OrganizationRequest
    {
        public StatusRequest(OrganizationId organizationId, int offset, int limit)
            : base(organizationId)
        {
            m_RequestUrl += "/status";

            AddParamToQuery("Offset", offset.ToString());
            AddParamToQuery("Limit", limit.ToString());
        }
    }
}

using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class CreateVersionLabelRequest : OrganizationRequest
    {
        readonly IVersionLabelBaseData m_Data;

        public CreateVersionLabelRequest(OrganizationId organizationId, IVersionLabelBaseData versionLabelData)
            : base(organizationId)
        {
            m_RequestUrl += "/labels";

            m_Data = versionLabelData;
        }

        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(m_Data, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class CreateFieldDefinitionRequest : OrganizationRequest
    {
        readonly IFieldDefinitionBaseData m_Data;

        public CreateFieldDefinitionRequest(OrganizationId organizationId, IFieldDefinitionBaseData data)
            : base(organizationId)
        {
            m_PathAndQueryParams += "/templates/fields";

            m_Data = data;
        }

        /// <inheritdoc/>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedSerialization.Serialize(m_Data, IsolatedSerialization.defaultSettings);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

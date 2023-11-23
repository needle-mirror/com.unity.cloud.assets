using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class ModifyFieldDefinitionSelectionRequest : FieldDefinitionRequest
    {
        public ModifyFieldDefinitionSelectionRequest(OrganizationId organizationId, string fieldKey, IEnumerable<string> values)
            : base(organizationId, fieldKey)
        {
            m_PathAndQueryParams += "/accepted-values";

            AddParamToQueryParams("values", values);
        }

        public override HttpContent ConstructBody()
        {
            return new StringContent("", Encoding.UTF8, "application/json");
        }
    }
}

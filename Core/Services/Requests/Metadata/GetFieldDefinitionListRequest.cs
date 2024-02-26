using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class GetFieldDefinitionListRequest : OrganizationRequest
    {
        public GetFieldDefinitionListRequest(OrganizationId organizationId, int limit, SortingOrder sortingOrder, string nextToken, bool includeDeleted)
            : base(organizationId)
        {
            m_RequestUrl += "/templates/fields";

            AddParamToQuery("IncludeDeleted", includeDeleted.ToString());
            AddParamToQuery("SortingOrder", sortingOrder.ToString());
            AddParamToQuery("Limit", limit.ToString());
            AddParamToQuery("Next", nextToken);
        }
    }
}

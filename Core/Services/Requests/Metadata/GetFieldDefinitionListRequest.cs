using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class GetFieldDefinitionListRequest : OrganizationRequest
    {
        public GetFieldDefinitionListRequest(OrganizationId organizationId, int limit, SortingOrder sortingOrder, string nextToken, bool includeDeleted)
            : base(organizationId)
        {
            m_PathAndQueryParams += "/templates/fields";

            AddParamToQueryParams("IncludeDeleted", includeDeleted.ToString());
            AddParamToQueryParams("SortingOrder", sortingOrder.ToString());
            AddParamToQueryParams("Limit", limit.ToString());
            AddParamToQueryParams("Next", nextToken);
        }
    }
}

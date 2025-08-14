using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class GetLabelListRequest : ApiRequest
    {
        /// <summary>
        /// Creates an instance of the <see cref="GetLabelListRequest"/> for an organization.
        /// </summary>
        /// <param name="organizationId">ID of the organization.</param>
        /// <param name="offset">Pagination offset.</param>
        /// <param name="limit">Pagination limit.</param>
        /// <param name="archived">Whether the labels are archived.</param>
        /// <param name="systemLabels">Whether the labels are system labels.</param>
        public GetLabelListRequest(OrganizationId organizationId, int offset, int limit, bool? archived, bool? systemLabels)
        {
            m_RequestUrl += $"/organizations/{organizationId}/labels";

            var status = string.Empty;
            if (archived.HasValue)
            {
                status = archived.Value ? "archived" : "active";
            }

            AddParamToQuery("IsSystemLabel", systemLabels?.ToString());
            AddParamToQuery("Status", status);
            AddParamToQuery("Offset", offset.ToString());
            AddParamToQuery("Limit", limit.ToString());
        }

        /// <summary>
        /// Creates an instance of the <see cref="GetLabelListRequest"/> for a library.
        /// </summary>
        /// <param name="assetLibraryId">ID of the library.</param>
        /// <param name="offset">Pagination offset.</param>
        /// <param name="limit">Pagination limit.</param>
        /// <param name="archived">Whether the labels are archived.</param>
        /// <param name="systemLabels">Whether the labels are system labels.</param>
        public GetLabelListRequest(AssetLibraryId assetLibraryId, int offset, int limit, bool? archived, bool? systemLabels)
        {
            m_RequestUrl = $"/libraries/{assetLibraryId}/labels";

            var status = string.Empty;
            if (archived.HasValue)
            {
                status = archived.Value ? "archived" : "active";
            }

            AddParamToQuery("IsSystemLabel", systemLabels?.ToString());
            AddParamToQuery("Status", status);
            AddParamToQuery("Offset", offset.ToString());
            AddParamToQuery("Limit", limit.ToString());
        }
    }
}

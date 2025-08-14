using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class SearchTransformationRequest : ProjectOrLibraryRequest
    {
        public SearchTransformationRequest(ProjectId projectId, TransformationSearchData searchData)
            : base(projectId)
        {
            m_RequestUrl += "/transformations";

            AddParamToQuery("assetId", searchData.AssetId);
            AddParamToQuery("assetVersion", searchData.AssetVersion);
            AddParamToQuery("datasetId", searchData.DatasetId);
            AddParamToQuery("userId", searchData.UserId);
            AddParamToQuery("status", searchData.Status?.ToString());
        }
    }
}

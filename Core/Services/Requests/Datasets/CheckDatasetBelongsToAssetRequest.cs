using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class CheckDatasetBelongsToAssetRequest : DatasetRequest
    {
        public CheckDatasetBelongsToAssetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, datasetId, xCorrelationId)
        {
            m_PathAndQueryParams += "/check";
        }
    }
}

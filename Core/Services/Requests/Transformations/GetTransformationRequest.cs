using Unity.Cloud.Common;

namespace Unity.Cloud.Assets.Transformations
{
    /// <summary>
    /// Represents a get transformation URL request.
    /// </summary>
    class GetTransformationRequest : DatasetRequest
    {
        public GetTransformationRequest(TransformationId transformationId, ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId)
            : base(projectId, assetId, assetVersion, datasetId, null)
        {
            m_PathAndQueryParams += $"/transformations/{transformationId}";
        }
    }
}

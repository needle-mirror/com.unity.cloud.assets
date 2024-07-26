using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class AssignAssetStatusFlowRequest : AssetRequest
    {
        public AssignAssetStatusFlowRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, string statusFlowId)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/statusflows/{statusFlowId}/assign";
        }
    }
}

using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class AssetVersionLabelRequest : ProjectRequest
    {
        public AssetVersionLabelRequest(ProjectId projectId, AssetId assetId, int offset, int limit)
            : base(projectId)
        {
            m_RequestUrl += $"/assets/{assetId}/labels";

            AddParamToQuery("Offset", offset.ToString());
            AddParamToQuery("Limit", limit.ToString());
        }
    }
}

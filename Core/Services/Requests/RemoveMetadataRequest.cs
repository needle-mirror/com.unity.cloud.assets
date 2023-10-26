using System.Collections.Generic;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class RemoveMetadataRequest : AssetRequest
    {
        public RemoveMetadataRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, string from, IEnumerable<string> keys, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, xCorrelationId)
        {
            m_PathAndQueryParams += $"/fields";
            AddParamToQueryParams(from, keys);
        }

        public RemoveMetadataRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string from, IEnumerable<string> keys, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, xCorrelationId)
        {
            m_PathAndQueryParams += $"/datasets/{datasetId}/fields";
            AddParamToQueryParams(from, keys);
        }

        public RemoveMetadataRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, string from, IEnumerable<string> keys, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, xCorrelationId)
        {
            m_PathAndQueryParams += $"/datasets/{datasetId}/files/{filePath}/fields";
            AddParamToQueryParams(from, keys);
        }
    }
}

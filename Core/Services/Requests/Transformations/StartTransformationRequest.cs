using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets.Transformations
{

    /// <summary>
    /// Represents a get transformation URL request.
    /// </summary>
    class StartTransformationRequest : DatasetRequest
    {
        public StartTransformationRequest(WorkflowType workflowType, ProjectId projectId, AssetId assetId,
            AssetVersion assetVersion, DatasetId datasetId)
            : base(projectId, assetId, assetVersion, datasetId, null)
        {
            m_PathAndQueryParams +=
                $"/transformations/start/{IsolatedSerialization.SerializeWithConverters(workflowType, IsolatedSerialization.StringEnumConverter).Replace("\"", "")}";
        }
    }
}

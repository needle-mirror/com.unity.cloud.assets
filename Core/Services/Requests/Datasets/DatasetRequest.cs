using System;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class DatasetRequest : AssetRequest
    {
        /// <summary>
        /// AssetRequest Request Object.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="assetId">ID of the asset</param>
        /// <param name="assetVersion">ID of the asset version</param>
        /// <param name="datasetId"></param>
        /// <param name="xCorrelationId"></param>
        protected DatasetRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, xCorrelationId)
        {
            m_PathAndQueryParams += $"/datasets/{datasetId}";
        }
    }
}

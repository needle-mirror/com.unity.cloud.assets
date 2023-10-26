using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class GenerateFileTagsRequest : FileRequest
    {
        /// <summary>
        /// Create Asset File Request Object.
        /// Create a single asset file.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file will linked to.</param>
        /// <param name="assetVersion">The version of the asset the file will linked to.</param>
        /// <param name="datasetId"></param>
        /// <param name="filePath">The path to the file for which the upload will be finalized.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public GenerateFileTagsRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, datasetId, filePath, xCorrelationId)
        {
            m_PathAndQueryParams += $"/auto-tags";
        }
    }
}

using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a finalized upload asset file request.
    /// </summary>
    class FileRequest : DatasetRequest
    {
        /// <summary>
        /// Creates an Asset File Request Object.
        /// Creates a single asset file.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file will linked to.</param>
        /// <param name="assetVersion">The version of the asset the file will linked to.</param>
        /// <param name="datasetId">The id of the dataset. </param>
        /// <param name="filePath">The path to the file in the dataset.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public FileRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, datasetId, xCorrelationId)
        {
            m_PathAndQueryParams += $"/files/{filePath}";
        }
    }
}

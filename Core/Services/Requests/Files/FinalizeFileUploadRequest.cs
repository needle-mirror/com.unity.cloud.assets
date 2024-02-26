using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a finalized upload asset file request.
    /// </summary>
    class FinalizeFileUploadRequest : FileRequest
    {
        /// <summary>
        /// Creates an Asset File Request Object.
        /// Creates a single asset file.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file will linked to.</param>
        /// <param name="assetVersion">The version of the asset the file will linked to.</param>
        /// <param name="filePath">The path to the file for which the upload will be finalized.</param>
        public FinalizeFileUploadRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, string filePath)
            : base(projectId, assetId, assetVersion, filePath)
        {
            m_RequestUrl += $"/finalize";
        }
    }
}

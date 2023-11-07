using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents an update asset file request.
    /// </summary>
    class UpdateFileRequest : AssetRequest
    {
        /// <summary>
        /// The asset file to update.
        /// </summary>
        public IFileBaseData FileData { get; }

        /// <summary>
        /// Updates an Asset File Request Object.
        /// Updates a single asset file.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="filePath"></param>
        /// <param name="fileData">The asset file to update.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public UpdateFileRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, string filePath, IFileBaseData fileData, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, xCorrelationId)
        {
            FileData = fileData;

            m_PathAndQueryParams += $"/files/{filePath}";
        }

        /// <summary>
        /// Provides a helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(FileData, SerializationUtilities.DatasetIdConverter);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

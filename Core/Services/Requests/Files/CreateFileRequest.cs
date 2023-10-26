using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a create asset file request.
    /// </summary>
    class CreateFileRequest : DatasetRequest
    {
        /// <summary>
        /// The asset file to create.
        /// </summary>
        IFileBaseData FileData { get; }

        /// <summary>
        /// Create Asset File Request Object.
        /// Create a single asset file.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file will linked to.</param>
        /// <param name="assetVersion">The version of the asset the file will linked to.</param>
        /// <param name="datasetId"></param>
        /// <param name="fileData">The asset file to create.</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public CreateFileRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, IFileBaseData fileData, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, datasetId, xCorrelationId)
        {
            m_PathAndQueryParams += $"/files";

            FileData = fileData;
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = IsolatedJsonConvert.SerializeObject(FileData);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

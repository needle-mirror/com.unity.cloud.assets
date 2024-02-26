using System;
using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a finalized upload asset file request.
    /// </summary>
    class FileRequest : AssetRequest
    {
        readonly IFileBaseData m_Data;

        /// <summary>
        /// Creates an Asset File Request Object.
        /// Creates a single asset file.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file will linked to.</param>
        /// <param name="assetVersion">The version of the asset the file will linked to.</param>
        /// <param name="datasetId">The id of the dataset. </param>
        /// <param name="filePath">The path to the file in the dataset.</param>
        /// <param name="data">The object containing the data of the file.</param>
        public FileRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, IFileBaseData data = null)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/datasets/{datasetId}/files/{Uri.EscapeDataString(filePath)}";

            m_Data = data;
        }

        /// <summary>
        /// Creates an Asset File Request Object.
        /// Creates a single asset file.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file will linked to.</param>
        /// <param name="assetVersion">The version of the asset the file will linked to.</param>
        /// <param name="filePath">The path to the file in the dataset.</param>
        /// <param name="data">The object containing the data of the file.</param>
        public FileRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, string filePath, IFileBaseData data = null)
            : base(projectId, assetId, assetVersion)
        {
            m_RequestUrl += $"/files/{Uri.EscapeDataString(filePath)}";

            m_Data = data;
        }

        /// <summary>
        /// Provides a helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            if (m_Data == null)
            {
                return base.ConstructBody();
            }

            var body = IsolatedSerialization.SerializeWithConverters(m_Data);
            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}

using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a get asset file url request.
    /// </summary>
    class GetFileUrlRequest : FileRequest
    {
        /// <summary>
        /// Get Asset File Url Request Object.
        /// Get a single asset file url.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="datasetId"></param>
        /// <param name="filePath">The asset file id url to get.</param>
        /// <param name="urlType"></param>
        /// <param name="fileData"></param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public GetFileUrlRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, string urlType, IFileData fileData, string xCorrelationId = default)
            : base(projectId, assetId, assetVersion, datasetId, filePath, xCorrelationId)
        {
            m_PathAndQueryParams += $"/url";

            AddParamToQueryParams("urlType", urlType);
            if (fileData != null)
            {
                AddParamToQueryParams("userChecksum", fileData.UserChecksum);
                AddParamToQueryParams("fileSize", fileData.SizeBytes.ToString());
                // TODO?? AddParamToQueryParams("width", "???");
            }
        }
    }
}

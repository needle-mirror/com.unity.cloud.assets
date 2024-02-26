using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a get asset file URL request.
    /// </summary>
    class GetFileUrlRequest : FileRequest
    {
        /// <summary>
        /// Gets an Asset File URL Request Object.
        /// Gets a single asset file URL.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">The id of the asset the file is linked to.</param>
        /// <param name="assetVersion">The version of the asset the file is linked to.</param>
        /// <param name="datasetId"></param>
        /// <param name="filePath">The asset file id url to get.</param>
        /// <param name="urlType"></param>
        /// <param name="fileData"></param>
        public GetFileUrlRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, string urlType, IFileData fileData)
            : base(projectId, assetId, assetVersion, datasetId, filePath)
        {
            m_RequestUrl += $"/url";

            AddParamToQuery("urlType", urlType);
            if (fileData != null)
            {
                AddParamToQuery("userChecksum", fileData.UserChecksum);
                AddParamToQuery("fileSize", fileData.SizeBytes.ToString());
                // To be done by [UCAM-317] AddParamToQueryParams("width", width.ToString())
            }
        }
    }
}

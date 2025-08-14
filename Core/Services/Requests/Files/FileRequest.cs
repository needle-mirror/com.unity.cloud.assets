using System;
using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Represents a finalized upload asset file request.
    /// </summary>
    class FileRequest : DatasetRequest
    {
        readonly IFileBaseData m_Data;

        /// <summary>
        /// Creates an instance of a <see cref="FileRequest"/> for a file in a project.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">ID the asset the file will link to.</param>
        /// <param name="assetVersion">The version of the asset the file will link to.</param>
        /// <param name="datasetId">ID the dataset. </param>
        /// <param name="filePath">The path to the file in the dataset.</param>
        /// <param name="data">The object containing the data of the file.</param>
        public FileRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, IFileBaseData data = null)
            : base(projectId, assetId, assetVersion, datasetId)
        {
            m_RequestUrl += $"/files/{Uri.EscapeDataString(filePath)}";

            m_Data = data;
        }

        /// <summary>
        /// Creates an instance of a <see cref="FileRequest"/> for a file in a library.
        /// </summary>
        /// <param name="assetLibraryId">ID of the library.</param>
        /// <param name="assetId">ID the asset the file will link to.</param>
        /// <param name="assetVersion">The version of the asset the file will link to.</param>
        /// <param name="datasetId">ID the dataset. </param>
        /// <param name="filePath">The path to the file in the dataset.</param>
        /// <param name="data">The object containing the data of the file.</param>
        public FileRequest(AssetLibraryId assetLibraryId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, IFileBaseData data = null)
            : base(assetLibraryId, assetId, assetVersion, datasetId)
        {
            m_RequestUrl += $"/files/{Uri.EscapeDataString(filePath)}";

            m_Data = data;
        }

        /// <summary>
        /// Creates an instance of a <see cref="FileRequest"/> for a file in a project.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">ID the asset the file will link to.</param>
        /// <param name="assetVersion">The version of the asset the file will link to.</param>
        /// <param name="datasetId">ID the dataset. </param>
        /// <param name="filePath">The path to the file in the dataset.</param>
        /// <param name="includedFileFields">Sets the fields to be included in the response.</param>
        public FileRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, string filePath, FileFields includedFileFields)
            : this(projectId, assetId, assetVersion, datasetId, filePath)
        {
            includedFileFields.Parse(AddFieldFilterToQueryParams);
        }

        /// <summary>
        /// Creates an instance of a <see cref="FileRequest"/> for a dataset in a project.
        /// </summary>
        /// <param name="projectId">ID of the project.</param>
        /// <param name="assetId">ID the asset the file will link to.</param>
        /// <param name="assetVersion">The version of the asset the file will link to.</param>
        /// <param name="datasetId">ID the dataset. </param>
        /// <param name="includedFieldsFilter">Sets the fields to be included in the response.</param>
        /// <param name="token">Pagination token.</param>
        /// <param name="limit">Pagination limit.</param>
        public FileRequest(ProjectId projectId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, FileFields includedFieldsFilter, string token = null, int? limit = null)
            : base(projectId, assetId, assetVersion, datasetId)
        {
            m_RequestUrl += "/files";

            includedFieldsFilter.Parse(AddFieldFilterToQueryParams);

            AddParamToQuery("Limit", limit?.ToString());
            AddParamToQuery("Token", token);
        }

        /// <summary>
        /// Creates an instance of a <see cref="FileRequest"/> for a dataset in a library.
        /// </summary>
        /// <param name="assetLibraryId">ID of the library.</param>
        /// <param name="assetId">ID the asset the file will link to.</param>
        /// <param name="assetVersion">The version of the asset the file will link to.</param>
        /// <param name="datasetId">ID the dataset. </param>
        /// <param name="includedFieldsFilter">Sets the fields to be included in the response.</param>
        /// <param name="token">Pagination token.</param>
        /// <param name="limit">Pagination limit.</param>
        public FileRequest(AssetLibraryId assetLibraryId, AssetId assetId, AssetVersion assetVersion, DatasetId datasetId, FileFields includedFieldsFilter, string token = null, int? limit = null)
            : base(assetLibraryId, assetId, assetVersion, datasetId)
        {
            m_RequestUrl += "/files";

            includedFieldsFilter.Parse(AddFieldFilterToQueryParams);

            AddParamToQuery("Limit", limit?.ToString());
            AddParamToQuery("Token", token);
        }

        /// <inheritdoc />
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

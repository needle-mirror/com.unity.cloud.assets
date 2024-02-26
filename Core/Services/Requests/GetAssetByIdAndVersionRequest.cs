using System;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// GetAssetByIdAndVersionRequest
    /// Get a single asset by id and version.
    /// </summary>
    class GetAssetByIdAndVersionRequest : AssetRequest
    {
        /// <summary>
        /// GetAssetByIdAndVersion Request Object.
        /// Get a single asset by id and version.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="assetId">ID of the asset</param>
        /// <param name="assetVersion">Version of the asset</param>
        /// <param name="includedFieldsFilter">Sets the fields to be included in the response.</param>
        public GetAssetByIdAndVersionRequest(ProjectId projectId,
            AssetId assetId,
            AssetVersion assetVersion,
            FieldsFilter includedFieldsFilter)
            : base(projectId, assetId, assetVersion)
        {
            includedFieldsFilter?.Parse(AddFieldFilterToQueryParams);
        }

        void AddFieldFilterToQueryParams(string value)
        {
            AddParamToQuery("IncludeFields", value);
        }
    }
}

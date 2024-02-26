using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    [Flags]
    enum AssetFields
    {
        /// <summary>
        /// Only the default fields will be populated
        /// </summary>
        none = 0,
        all = ~none,
        description = 1,
        authoring = 2,
        metadata = 4,
        previewFile = 16,
        previewFileUrl = 32,
        /// <summary>
        /// Will populate the dataset cache with only the default fields; use DatasetFields to specify which fields to populate.
        /// </summary>
        datasets = 64,
        /// <summary>
        /// Will populate the file cache with only the default fields; use FileFields to specify which fields to populate.
        /// </summary>
        files = 128,
    }

    [Flags]
    enum DatasetFields
    {
        /// <summary>
        /// Only the default fields will be populated
        /// </summary>
        none = 0,
        all = ~none,
        description = 1,
        authoring = 2,
        metadata = 8,
        files = 32,
        filesOrder = 64,
    }

    [Flags]
    enum FileFields
    {
        /// <summary>
        /// Only the default fields will be populated
        /// </summary>
        none = 0,
        all = ~none,
        description = 1,
        authoring = 2,
        downloadUrl = 4,
        metadata = 16,
        userChecksum = 64,
        fileSize = 128,
        previewUrl = 256,
    }

    class FieldsFilter
    {
        public AssetFields AssetFields { get; set; } = AssetFields.none;
        public DatasetFields DatasetFields { get; set; } = DatasetFields.none;
        public FileFields FileFields { get; set; } = FileFields.none;
        public List<string> MetadataFields { get; } = new();

        public static FieldsFilter None => new()
        {
            AssetFields = AssetFields.none,
            DatasetFields = DatasetFields.none,
            FileFields = FileFields.none,
        };

        public static FieldsFilter DefaultAssetIncludes => new()
        {
            AssetFields = AssetFields.description | AssetFields.authoring | AssetFields.previewFile,
            DatasetFields = DatasetFields.none,
            FileFields = FileFields.none,
        };

        public static FieldsFilter DefaultDatasetIncludes => new()
        {
            AssetFields = AssetFields.datasets,
            DatasetFields = DatasetFields.description | DatasetFields.authoring | DatasetFields.filesOrder,
            FileFields = FileFields.none,
        };

        public static FieldsFilter DefaultFileIncludes => new()
        {
            AssetFields = AssetFields.files,
            DatasetFields = DatasetFields.none,
            FileFields = FileFields.description | FileFields.authoring | FileFields.userChecksum | FileFields.fileSize
        };

        public static FieldsFilter All => new()
        {
            AssetFields = AssetFields.all,
            DatasetFields = DatasetFields.all,
            FileFields = FileFields.all,
        };
    }
}

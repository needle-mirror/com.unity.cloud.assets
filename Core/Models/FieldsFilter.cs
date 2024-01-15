using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    [Flags]
    public enum AssetFields
    {
        /// <summary>
        /// Only the default fields will be populated
        /// </summary>
        none = 0,
        all = ~none,
        authoring = 1,
        // portalMetadata = 2, Deprecated
        metadata = 4,
        systemMetadata = 8,
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
    public enum DatasetFields
    {
        /// <summary>
        /// Only the default fields will be populated
        /// </summary>
        none = 0,
        all = ~none,
        description = 1,
        authoring = 2,
        // portalMetadata = 4, Deprecated
        metadata = 8,
        systemMetadata = 16,
        files = 32,
        filesOrder = 64,
    }

    [Flags]
    public enum FileFields
    {
        /// <summary>
        /// Only the default fields will be populated
        /// </summary>
        none = 0,
        all = ~none,
        description = 1,
        authoring = 2,
        downloadUrl = 4,
        // portalMetadata = 8, deprecated
        metadata = 16,
        systemMetadata = 32,
        userChecksum = 64,
        fileSize = 128,
        previewUrl = 256,
    }

    public class FieldsFilter
    {
        public AssetFields AssetFields { get; set; } = AssetFields.none;
        public DatasetFields DatasetFields { get; set; } = DatasetFields.none;
        public FileFields FileFields { get; set; } = FileFields.none;
        public List<string> MetadataFields { get; } = new();
        public List<string> SystemMetadataFields { get; } = new();

        public static FieldsFilter Default => new()
        {
            AssetFields = AssetFields.all,
            DatasetFields = DatasetFields.none,
            FileFields = FileFields.none,
        };

        public static FieldsFilter All => new()
        {
            AssetFields = AssetFields.all,
            DatasetFields = DatasetFields.all,
            FileFields = FileFields.all,
        };

        public FieldsFilter WithMetadataFields(params string[] metadataFields)
        {
            MetadataFields.AddRange(metadataFields);
            return this;
        }

        public FieldsFilter WithSystemMetadataFields(params string[] systemMetadataFields)
        {
            SystemMetadataFields.AddRange(systemMetadataFields);
            return this;
        }
    }
}

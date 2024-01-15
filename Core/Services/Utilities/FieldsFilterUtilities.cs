using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    static class FieldsFilterUtilities
    {
        public delegate void OnFieldFilterSelected(string field);

        internal static void Parse(this FieldsFilter fieldsFilter, OnFieldFilterSelected select)
        {
            if (fieldsFilter == null) return;

            if (fieldsFilter.AssetFields.HasFlag(AssetFields.all))
            {
                select("*");
            }
            else
            {
                fieldsFilter.AssetFields.Parse(select);
            }

            fieldsFilter.DatasetFields.Parse(select);
            fieldsFilter.FileFields.Parse(select);

            fieldsFilter.MetadataFields.Select(select, "metadata",
                fieldsFilter.AssetFields.HasFlag(AssetFields.metadata),
                fieldsFilter.DatasetFields.HasFlag(DatasetFields.metadata),
                fieldsFilter.FileFields.HasFlag(FileFields.metadata));

            fieldsFilter.SystemMetadataFields.Select(select, "systemMetadata",
                fieldsFilter.AssetFields.HasFlag(AssetFields.systemMetadata),
                fieldsFilter.DatasetFields.HasFlag(DatasetFields.systemMetadata),
                fieldsFilter.FileFields.HasFlag(FileFields.systemMetadata));
        }

        static void Parse(this FileFields fileFields, OnFieldFilterSelected select)
        {
            if (fileFields.HasFlag(FileFields.all))
            {
                select("files.*");
                // Explicitly include these as they fail to be returned when using the wildcard.
                select("files.downloadURL");
                select("files.previewURL");
                return;
            }

            foreach (FileFields value in Enum.GetValues(typeof(FileFields)))
            {
                if (value is FileFields.all or FileFields.none) continue;
                if (fileFields.HasFlag(value))
                {
                    if (value == FileFields.authoring)
                    {
                        IncludeAuthoringFields("files.", select);
                    }
                    else if (value == FileFields.downloadUrl)
                    {
                        select("files.downloadURL");
                    }
                    else if (value == FileFields.previewUrl)
                    {
                        select("files.previewURL");
                    }
                    else
                    {
                        select($"files.{value.ToString()}");
                    }
                }
            }
        }

        static void Parse(this DatasetFields datasetFields, OnFieldFilterSelected select)
        {
            if (datasetFields.HasFlag(DatasetFields.all))
            {
                select("datasets.*");
                return;
            }

            foreach (DatasetFields value in Enum.GetValues(typeof(DatasetFields)))
            {
                if (value is DatasetFields.all or DatasetFields.none or DatasetFields.files) continue;
                if (datasetFields.HasFlag(value))
                {
                    if (value == DatasetFields.authoring)
                    {
                        IncludeAuthoringFields("datasets.", select);
                    }
                    else
                    {
                        select($"datasets.{value.ToString()}");
                    }
                }
            }
        }

        static void Parse(this AssetFields assetFields, OnFieldFilterSelected select)
        {
            foreach (AssetFields value in Enum.GetValues(typeof(AssetFields)))
            {
                if (value == AssetFields.all || value == AssetFields.none) continue;
                if (assetFields.HasFlag(value))
                {
                    if (value == AssetFields.authoring)
                    {
                        IncludeAuthoringFields("", select);
                    }
                    else
                    {
                        select(value.ToString());
                    }
                }
            }
        }

        static void IncludeAuthoringFields(string prefix, OnFieldFilterSelected action)
        {
            action(prefix + "created");
            action(prefix + "createdBy");
            action(prefix + "updated");
            action(prefix + "updatedBy");
        }

        static void Select(this IEnumerable<string> metadataKeys, OnFieldFilterSelected select, string metadataprefix, bool hasAssetFlag, bool hasDatasetFlag, bool hasFileFlag)
        {
            foreach (var field in metadataKeys)
            {
                if (hasAssetFlag) select($"{metadataprefix}.{field}");
                if (hasDatasetFlag) select($"datasets.{metadataprefix}.{field}");
                if (hasFileFlag) select($"files.{metadataprefix}.{field}");
            }
        }
    }
}

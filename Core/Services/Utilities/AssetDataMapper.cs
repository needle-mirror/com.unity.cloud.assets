using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    static class AssetDataMapper
    {
        internal static void MapFrom(this Asset asset, IAssetDataSource assetDataSource, IAssetData assetData, FieldsFilter includeFields)
        {
            asset.Name = assetData.Name;
            asset.Description = assetData.Description;
            asset.Tags = assetData.Tags ?? Array.Empty<string>();
            asset.Type = assetData.Type;
            asset.Status = assetData.Status;
            asset.StorageId = assetData.StorageId;
            asset.SystemTags = assetData.SystemTags;
            asset.Labels = assetData.Labels;

            if (includeFields.AssetFields.HasFlag(AssetFields.previewFile))
                asset.PreviewFile = assetData.PreviewFile;

            if (includeFields.AssetFields.HasFlag(AssetFields.previewFileUrl))
            {
                Uri.TryCreate(assetData.PreviewFileUrl, UriKind.RelativeOrAbsolute, out var previewFileDownloadUrl);
                asset.PreviewFileUrl = previewFileDownloadUrl;
            }

            FileEntity[] files = null;
            if (includeFields.AssetFields.HasFlag(AssetFields.files))
            {
                // Ignore files not linked to a dataset, many operations will not be supported on these files.
                files = assetData.Files?
                    .Where(fileData => fileData.DatasetIds != null && fileData.DatasetIds.Any())
                    .Select(fileData => fileData.From(assetDataSource, asset.Descriptor, includeFields.FileFields))
                    .ToArray();

                asset.Files = files is {Length: 0} ? null : files;
            }

            if (includeFields.AssetFields.HasFlag(AssetFields.datasets))
            {
                var datasets = assetData.Datasets?
                    .Select(datasetData => datasetData.From(assetDataSource, asset.Descriptor, includeFields.DatasetFields, files))
                    .ToArray();

                asset.Datasets = datasets is {Length: 0} ? null : datasets;
            }

            if (includeFields.AssetFields.HasFlag(AssetFields.authoring))
                asset.AuthoringInfo = new AuthoringInfo(assetData.CreatedBy, assetData.Created, assetData.UpdatedBy, assetData.Updated);

            if (includeFields.AssetFields.HasFlag(AssetFields.portalMetadata))
                asset.PortalMetadata = assetData.PortalMetadata;

            if (includeFields.AssetFields.HasFlag(AssetFields.metadata))
                asset.Metadata = assetData.Metadata;

            if (includeFields.AssetFields.HasFlag(AssetFields.systemMetadata))
                asset.SystemMetadata = assetData.SystemMetadata;
        }

        internal static AssetCreateData From(this IAssetCreation assetCreation)
        {
            return new AssetCreateData
            {
                Name = assetCreation.Name,
                Description = assetCreation.Description,
                Tags = assetCreation.Tags,
                Type = assetCreation.Type,
                PortalMetadata = assetCreation.PortalMetadata,
                Metadata = assetCreation.Metadata,
                SystemMetadata = assetCreation.SystemMetadata,
                Collections = assetCreation.Collections,
            };
        }

        internal static AssetProjectEntity From(this IProjectData data, IAssetDataSource assetDataSource, OrganizationId organizationId)
        {
            return data.From(assetDataSource, new ProjectDescriptor(organizationId, data.Id));
        }

        internal static AssetProjectEntity From(this IProjectData data, IAssetDataSource assetDataSource, ProjectDescriptor projectDescriptor)
        {
            return new AssetProjectEntity(assetDataSource, projectDescriptor)
            {
                Name = data.Name,
                Metadata = data.Metadata,
            };
        }

        internal static Asset From(this IAssetData data, IAssetDataSource assetDataSource, OrganizationId organizationId, IEnumerable<ProjectId> availableProjects)
        {
            data.ValidateSourceProjectId();

            var validProjects = new HashSet<ProjectId>(availableProjects);
            validProjects.IntersectWith(data.LinkedProjectIds ?? Array.Empty<ProjectId>());

            return data.From(assetDataSource, new ProjectDescriptor(organizationId, validProjects.FirstOrDefault()));
        }

        internal static Asset From(this IAssetData data, IAssetDataSource assetDataSource, ProjectDescriptor projectDescriptor)
        {
            data.ValidateSourceProjectId();
            var descriptor = new AssetDescriptor(projectDescriptor, data.Id, data.Version);
            return data.From(assetDataSource, descriptor);
        }

        internal static Asset From(this IAssetData data, IAssetDataSource assetDataSource, AssetDescriptor assetDescriptor)
        {
            var asset = new Asset(assetDataSource, assetDescriptor, data.SourceProjectId, data.LinkedProjectIds);
            asset.MapFrom(assetDataSource, data, FieldsFilter.All);
            return asset;
        }

        internal static IAsset From(this AssetDataWithIdentifiers data, IAssetDataSource dataSource)
        {
            var assetVersionDescriptor = data.Identifier.From();
            return data.Data.From(dataSource, assetVersionDescriptor);
        }

        internal static AssetDescriptor From(this AssetIdentifier ids)
        {
            var projectDescriptor = new ProjectDescriptor(ids.OrganizationId, ids.ProjectId);
            return new AssetDescriptor(projectDescriptor, ids.Id, ids.Version);
        }

        internal static AssetData From(this Asset asset)
        {
            return new AssetData(asset.Descriptor.AssetId, asset.Descriptor.AssetVersion)
            {
                Name = asset.Name,
                Description = asset.Description,
                Tags = asset.Tags?.ToList(),
                Type = asset.Type,
                PreviewFile = asset.PreviewFile,
                PreviewFileUrl = asset.PreviewFileUrl?.ToString(),
                Status = asset.Status,
                Created = asset.AuthoringInfo.Created,
                CreatedBy = asset.AuthoringInfo.CreatedBy,
                Updated = asset.AuthoringInfo.Updated,
                UpdatedBy = asset.AuthoringInfo.UpdatedBy,
                StorageId = asset.StorageId,
                Files = asset.Files?.Select(file => file.From()),
                Datasets = asset.Datasets?.Select(dataset => dataset.From()),
                SourceProjectId = asset.SourceProject.ProjectId,
                LinkedProjectIds = asset.LinkedProjects.Select(project => project.ProjectId).ToList(),
                PortalMetadata = asset.PortalMetadata,
                Metadata = asset.Metadata,
                SystemMetadata = asset.SystemMetadata,
                SystemTags = asset.SystemTags,
                Labels = asset.Labels,
            };
        }

        internal static AssetCollection From(this IAssetCollectionData data, IAssetDataSource dataSource, ProjectDescriptor projectDescriptor)
        {
            return data.From(dataSource, new CollectionDescriptor(projectDescriptor, data.GetFullCollectionPath()));
        }

        internal static AssetCollection From(this IAssetCollectionData data, IAssetDataSource dataSource, CollectionDescriptor collectionDescriptor)
        {
            return new AssetCollection(dataSource, collectionDescriptor, data.Name, data.Description, data.ParentPath);
        }

        internal static IAssetCollectionData From(this AssetCollection assetCollection)
        {
            return new AssetCollectionData(assetCollection.Name, assetCollection.ParentPath)
            {
                Description = assetCollection.Description,
            };
        }

        static void ValidateSourceProjectId(this IAssetData data)
        {
            var sourceProjectId = data.SourceProjectId;
            if (sourceProjectId == ProjectId.None || string.IsNullOrEmpty(sourceProjectId.ToString()))
            {
                sourceProjectId = data.LinkedProjectIds?.FirstOrDefault() ?? ProjectId.None;
            }

            data.SourceProjectId = sourceProjectId;
        }
    }
}

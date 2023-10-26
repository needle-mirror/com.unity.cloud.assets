using System;
using System.Linq;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    static class FileDataMapper
    {
        internal static void MapFrom(this FileEntity file, IFileData fileData, FileFields includeFields)
        {
            file.Tags = fileData.Tags;
            file.SystemTags = fileData.SystemTags;

            if (includeFields.HasFlag(FileFields.description))
                file.Description = fileData.Description;
            if (includeFields.HasFlag(FileFields.authoring))
                file.AuthoringInfo = new AuthoringInfo(fileData.CreatedBy, fileData.Created, fileData.UpdatedBy, fileData.Updated);
            if (includeFields.HasFlag(FileFields.downloadUrl))
            {
                Uri.TryCreate(fileData.DownloadUrl, UriKind.RelativeOrAbsolute, out var downloadUrl);
                file.DownloadUrl = downloadUrl;
            }
            if (includeFields.HasFlag(FileFields.previewUrl))
            {
                Uri.TryCreate(fileData.PreviewUrl, UriKind.RelativeOrAbsolute, out var previewUrl);
                file.PreviewUrl = previewUrl;
            }
            if (includeFields.HasFlag(FileFields.portalMetadata))
                file.PortalMetadata = fileData.PortalMetadata;
            if (includeFields.HasFlag(FileFields.metadata))
                file.Metadata = fileData.Metadata;
            if (includeFields.HasFlag(FileFields.systemMetadata))
                file.SystemMetadata = fileData.SystemMetadata;
            if (includeFields.HasFlag(FileFields.userChecksum))
                file.UserChecksum = fileData.UserChecksum;
            if (includeFields.HasFlag(FileFields.fileSize))
                file.SizeBytes = fileData.SizeBytes;
        }

        internal static FileEntity From(this IFileData fileData, IAssetDataSource assetDataSource, FileDescriptor fileDescriptor, FileFields includeFields)
        {
            var file = new FileEntity(assetDataSource, fileDescriptor, fileData.DatasetIds);
            file.MapFrom(fileData, includeFields);
            return file;
        }

        internal static FileEntity From(this IFileData fileData, IAssetDataSource assetDataSource, AssetDescriptor assetDescriptor, FileFields includeFields)
        {
            // Because actions cannot be performed on files that is not linked to any dataset, we ignore these files.
            if (fileData.DatasetIds == null || !fileData.DatasetIds.Any()) return null;

            var fileDescriptor = new FileDescriptor(new DatasetDescriptor(assetDescriptor, fileData.DatasetIds.First()), fileData.Path);
            return fileData.From(assetDataSource, fileDescriptor, includeFields);
        }

        internal static FileData From(this FileEntity fileEntity)
        {
            return new FileData
            {
                Path = fileEntity.Descriptor.Path,
                Description = fileEntity.Description,
                Tags = fileEntity.Tags,
                SystemTags = fileEntity.SystemTags,
                PortalMetadata = fileEntity.PortalMetadata,
                Metadata = fileEntity.Metadata,
                SystemMetadata = fileEntity.SystemMetadata,
                CreatedBy = fileEntity.AuthoringInfo.CreatedBy,
                Created = fileEntity.AuthoringInfo.Created,
                UpdatedBy = fileEntity.AuthoringInfo.UpdatedBy,
                Updated = fileEntity.AuthoringInfo.Updated,
                SizeBytes = fileEntity.SizeBytes,
                UserChecksum = fileEntity.UserChecksum,
                DownloadUrl = fileEntity.DownloadUrl?.ToString(),
                PreviewUrl = fileEntity.PreviewUrl?.ToString()
            };
        }

        internal static IFileBaseData From(this IFileUpdate fileUpdate)
        {
            return new FileBaseData
            {
                Description = fileUpdate.Description,
                Tags = fileUpdate.Tags,
                PortalMetadata = fileUpdate.PortalMetadata,
                Metadata = fileUpdate.Metadata,
                SystemMetadata = fileUpdate.SystemMetadata
            };
        }
    }
}

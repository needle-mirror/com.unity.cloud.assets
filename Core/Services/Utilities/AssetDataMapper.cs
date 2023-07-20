using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public static class AssetDataMapper
    {
        public static Asset MapFrom(this IAssetCreation assetCreation)
        {
            return new Asset
            {
                Name = assetCreation.Name,
                Description = assetCreation.Description,
                Version = assetCreation.Version,
                VersionName = assetCreation.VersionName,
                Type = assetCreation.Type
            };
        }

        public static AssetFile MapFrom(this IAssetFileCreation fileCreation)
        {
            return new AssetFile
            {
                Name = fileCreation.Name,
                Description = fileCreation.Description,
                Type = fileCreation.Type,
                Tags = fileCreation.Tags ?? new List<string>(),
                FileSize = fileCreation.FileSize,
                Details = fileCreation.Details ?? new Dictionary<string, IDeserializable>(),
                Metadata = fileCreation.Metadata ?? new Dictionary<string, IDeserializable>()
            };
        }

        public static AssetFile MapFrom(this IAssetFile model)
        {
            if (model is AssetFile cloudAssetFile) return cloudAssetFile;

            return new AssetFile
            {
                Name = model.Name,
                Description = model.Description,
                Type = model.Type,
                Status = model.Status,
                StatusDetails = model.StatusDetails,
                Tags = model.Tags,
                FileSize = model.FileSize,
                Id = model.Id,
                UploadUrl = model.UploadUrl,
                DownloadUrl = model.DownloadUrl,
                AssetId = model.AssetId,
                AssetVersion = model.AssetVersion,
                StorageId = model.StorageId,
            };
        }

        public static AssetFile MapFrom(this IAssetAttachment model)
        {
            if (model is AssetFile cloudAssetFile) return cloudAssetFile;

            return new AssetFile
            {
                Name = model.Name,
                Description = model.Description,
                Type = model.Type,
                Status = model.Status,
                StatusDetails = model.StatusDetails,
                Tags = model.Tags,
                FileSize = model.FileSize,
                Id = model.Id,
                UploadUrl = model.UploadUrl,
                DownloadUrl = model.DownloadUrl,
                AssetId = model.AssetId,
                AssetVersion = model.AssetVersion,
                StorageId = model.StorageId,
            };
        }
    }
}

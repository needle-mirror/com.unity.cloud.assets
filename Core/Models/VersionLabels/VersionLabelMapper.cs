using System.Drawing;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    static partial class EntityMapper
    {
        internal static void MapFrom(this VersionLabelEntity label, IVersionLabelData labelData)
        {
            label.Description = labelData.Description;
            label.DisplayColor = labelData.DisplayColor ?? Color.White;
            label.IsSystemLabel = labelData.IsSystemLabel;
            label.IsAssignable = labelData.IsUserAssignable;
            label.AuthoringInfo = new AuthoringInfo(labelData.CreatedBy, labelData.Created, labelData.UpdatedBy, labelData.Updated);
        }

        internal static IVersionLabelBaseData From(this IVersionLabelCreation labelCreation)
        {
            return new VersionLabelBaseData
            {
                Name = labelCreation.Name,
                Description = labelCreation.Description,
                DisplayColor = labelCreation.DisplayColor
            };
        }

        internal static IVersionLabelBaseData From(this IVersionLabelUpdate labelUpdate)
        {
            return new VersionLabelBaseData
            {
                Description = labelUpdate.Description,
                DisplayColor = labelUpdate.DisplayColor
            };
        }

        internal static VersionLabelEntity From(this IVersionLabelData data, IAssetDataSource assetDataSource, OrganizationId organizationId)
        {
            return data.From(assetDataSource, new VersionLabelDescriptor(organizationId, data.Name));
        }

        internal static VersionLabelEntity From(this IVersionLabelData data, IAssetDataSource assetDataSource, VersionLabelDescriptor versionLabelDescriptor)
        {
            var label = new VersionLabelEntity(assetDataSource, versionLabelDescriptor);
            label.MapFrom(data);
            return label;
        }
    }
}

using System;
using System.Linq;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    static partial class EntityMapper
    {
        internal static void MapFrom(this FieldDefinitionEntity entity, IFieldDefinitionData data)
        {
            entity.Type = data.Type;
            entity.Status = data.Status;
            entity.DisplayName = data.DisplayName;
            entity.AuthoringInfo = new AuthoringInfo(data.CreatedBy, data.Created, data.UpdatedBy, data.Updated);
            entity.AcceptedValues = data.AcceptedValues?.ToArray() ?? Array.Empty<string>();
            entity.Multiselection = data.Multiselection;
        }

        internal static FieldDefinitionEntity From(this IFieldDefinitionData data, IAssetDataSource assetDataSource, FieldDefinitionDescriptor fieldDefinitionDescriptor)
        {
            var entity = new FieldDefinitionEntity(assetDataSource, fieldDefinitionDescriptor);
            entity.MapFrom(data);
            return entity;
        }

        internal static FieldDefinitionEntity From(this IFieldDefinitionData data, IAssetDataSource assetDataSource, OrganizationId organizationId)
        {
            return data.From(assetDataSource, new FieldDefinitionDescriptor(organizationId, data.Name));
        }

        internal static FieldDefinitionData From(this FieldDefinitionEntity entity)
        {
            return new FieldDefinitionData
            {
                Type = entity.Type,
                Status = entity.Status,
                DisplayName = entity.DisplayName,
                CreatedBy = entity.AuthoringInfo?.CreatedBy,
                Created = entity.AuthoringInfo?.Created,
                UpdatedBy = entity.AuthoringInfo?.UpdatedBy,
                Updated = entity.AuthoringInfo?.Updated,
                AcceptedValues = entity.AcceptedValues?.ToArray() ?? Array.Empty<string>(),
                Multiselection = entity.Multiselection
            };
        }

        internal static IFieldDefinitionBaseData From(this IFieldDefinitionUpdate update)
        {
            return new FieldDefinitionBaseData
            {
                DisplayName = update.DisplayName,
                AcceptedValues = update.AcceptedValues?.ToArray() ?? Array.Empty<string>()
            };
        }

        internal static IFieldDefinitionCreateData From(this IFieldDefinitionCreation create)
        {
            return new FieldDefinitionCreateData
            {
                Name = create.Key,
                Type = create.Type,
                DisplayName = create.DisplayName,
                AcceptedValues = create.AcceptedValues?.ToArray() ?? Array.Empty<string>(),
                Multiselection = create.Multiselection,
            };
        }
    }
}

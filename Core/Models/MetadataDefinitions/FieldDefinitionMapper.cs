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
            entity.IsDeleted = data.Status == "Deleted";
            entity.DisplayName = data.DisplayName;
            entity.AuthoringInfo = new AuthoringInfo(data.CreatedBy, data.Created, data.UpdatedBy, data.Updated);

            if (entity is SelectionFieldDefinitionEntity selectionFieldDefinition)
            {
                selectionFieldDefinition.AcceptedValues = data.AcceptedValues?.ToArray() ?? Array.Empty<string>();
                selectionFieldDefinition.Multiselection = data.Multiselection ?? false;
            }
        }

        internal static FieldDefinitionEntity From(this IFieldDefinitionData data, IAssetDataSource assetDataSource, FieldDefinitionDescriptor fieldDefinitionDescriptor)
        {
            var entity = data.Type switch
            {
                FieldDefinitionType.Selection => new SelectionFieldDefinitionEntity(assetDataSource, fieldDefinitionDescriptor),
                _ => new FieldDefinitionEntity(assetDataSource, fieldDefinitionDescriptor)
            };

            entity.MapFrom(data);
            return entity;
        }

        internal static FieldDefinitionEntity From(this IFieldDefinitionData data, IAssetDataSource assetDataSource, OrganizationId organizationId)
        {
            return data.From(assetDataSource, new FieldDefinitionDescriptor(organizationId, data.Name));
        }

        internal static FieldDefinitionData From(this FieldDefinitionEntity entity)
        {
            var data = new FieldDefinitionData
            {
                Type = entity.Type,
                Status = entity.IsDeleted ? "Deleted" : "Active",
                DisplayName = entity.DisplayName,
                CreatedBy = entity.AuthoringInfo?.CreatedBy.ToString(),
                Created = entity.AuthoringInfo?.Created,
                UpdatedBy = entity.AuthoringInfo?.UpdatedBy.ToString(),
                Updated = entity.AuthoringInfo?.Updated,
            };
            if (entity is SelectionFieldDefinitionEntity selectionFieldDefinition)
            {
                data.AcceptedValues = selectionFieldDefinition.AcceptedValues?.ToArray() ?? Array.Empty<string>();
                data.Multiselection = selectionFieldDefinition.Multiselection;
            }

            return data;
        }

        internal static IFieldDefinitionBaseData From(this IFieldDefinitionUpdate update)
        {
            return new FieldDefinitionBaseData
            {
                DisplayName = update.DisplayName
            };
        }

        internal static IFieldDefinitionCreateData From(this IFieldDefinitionCreation create)
        {
            var data = new FieldDefinitionCreateData
            {
                Name = create.Key,
                Type = create.Type,
                DisplayName = create.DisplayName,
            };

            if (create is ISelectionFieldDefinitionCreation selectionFieldDefinitionCreation)
            {
                data.Multiselection = selectionFieldDefinitionCreation.Multiselection;
                data.AcceptedValues = selectionFieldDefinitionCreation.AcceptedValues?.ToArray() ?? Array.Empty<string>();

                if (data.AcceptedValues.Length == 0)
                {
                    throw new ArgumentException("Accepted values must not be empty.");
                }
            }
            else if (create.Type == FieldDefinitionType.Selection)
            {
                data.Multiselection = false;
                data.AcceptedValues = new[] {"default"};
            }

            return data;
        }
    }
}

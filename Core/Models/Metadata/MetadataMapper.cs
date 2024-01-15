using System.Collections.Generic;
using System.Linq;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    static partial class EntityMapper
    {
        static MetadataValue From(object obj, IAssetDataSource dataSource, FieldDefinitionDescriptor fieldDefinitionDescriptor)
        {
            return new MetadataValue(IsolatedSerialization.ToObject(obj), dataSource, fieldDefinitionDescriptor);
        }

        static object From(this MetadataValue metadataValue)
        {
            return metadataValue.Value;
        }

        static Dictionary<string, object> From(this IDictionary<string, MetadataValue> metadataDictionary)
        {
            return metadataDictionary.ToDictionary(pair => pair.Key, pair => pair.Value.From());
        }

        static Dictionary<string, object> From(this MetadataContainerEntity metadataContainer)
        {
            return metadataContainer.Properties.From();
        }

        internal static Dictionary<string, MetadataValue> From(this IDictionary<string, object> dictionary, IAssetDataSource dataSource, OrganizationId organizationId)
        {
            return dictionary.ToDictionary(pair => pair.Key, pair => From(pair.Value, dataSource, new FieldDefinitionDescriptor(organizationId, pair.Key)));
        }
    }
}

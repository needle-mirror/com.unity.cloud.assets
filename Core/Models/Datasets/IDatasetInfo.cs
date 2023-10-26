using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IDatasetInfo
    {
        string Name { get; set; }
        string Description { get; set; }
        List<string> Tags { get; }
        IDeserializable PortalMetadata { get; }
        IDeserializable Metadata { get; }
        IDeserializable SystemMetadata { get; }
    }
}

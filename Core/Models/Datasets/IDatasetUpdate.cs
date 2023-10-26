using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IDatasetUpdate : IDatasetInfo
    {
        IReadOnlyList<string> FileOrder { get; }
        bool IsVisible { get; set; }
    }
}

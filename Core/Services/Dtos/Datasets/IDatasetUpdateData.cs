using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    interface IDatasetUpdateData : IDatasetBaseData
    {
        [DataMember(Name = "filesOrder")]
        IEnumerable<string> FileOrder { get; }

        [DataMember(Name = "isVisible")]
        bool IsVisible { get; }
    }
}

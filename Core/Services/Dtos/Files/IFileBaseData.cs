using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    interface IFileBaseData : IMetadataInfo
    {
        [DataMember(Name = "description")]
        string Description { get; }

        [DataMember(Name = "tags")]
        IEnumerable<string> Tags { get; }
    }
}

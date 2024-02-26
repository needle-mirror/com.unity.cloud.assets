using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    interface IMetadataInfo
    {
        /// <summary>
        /// The user metadata.
        /// </summary>
        [DataMember(Name = "metadata")]
        Dictionary<string, object> Metadata { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    interface IFileCreateData : IFileBaseData
    {
        [DataMember(Name = "filePath")]
        string Path { get; }

        [DataMember(Name = "fileSize")]
        long SizeBytes { get; }

        [DataMember(Name = "userChecksum")]
        string UserChecksum { get; }
    }
}

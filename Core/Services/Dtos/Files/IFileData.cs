using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    interface IFileData : IFileCreateData, IAuthoringData
    {
        [DataMember(Name = "previewUrl")]
        string PreviewUrl { get; }

        [DataMember(Name = "datasetIds")]
        IEnumerable<DatasetId> DatasetIds { get; }

        [DataMember(Name = "systemTags")]
        IEnumerable<string> SystemTags { get; }

        [DataMember(Name = "status")]
        string Status { get; }

        [DataMember(Name = "downloadURL")]
        string DownloadUrl { get; }
    }
}

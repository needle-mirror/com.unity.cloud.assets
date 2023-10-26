using System;

namespace Unity.Cloud.Assets
{
    interface IPendingFileData : IFileCreateData
    {
        Uri UploadUrl { get; }
    }
}

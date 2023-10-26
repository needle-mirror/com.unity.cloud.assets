using System;

namespace Unity.Cloud.Assets
{
    class PendingFileData : FileCreateData, IPendingFileData
    {
        public Uri UploadUrl { get; set; }
    }
}

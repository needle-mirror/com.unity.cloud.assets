using System;

namespace Unity.Cloud.Assets
{
    struct FileDownloadUrl
    {
        public string FilePath { get; set; }
        public Uri DownloadUrl { get; set; }
    }
}

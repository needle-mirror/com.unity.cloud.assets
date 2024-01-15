using System;

namespace Unity.Cloud.Assets
{
    struct AssetDownloadUrl
    {
        public string FilePath { get; set; }
        public Uri DownloadUrl { get; set; }
    }
}

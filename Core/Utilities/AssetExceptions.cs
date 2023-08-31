using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [Serializable]
    public sealed class InvalidUploadUrlException : Exception
    {
        public InvalidUploadUrlException(string message)
            : base(message) { }

        InvalidUploadUrlException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public sealed class InvalidDownloadUrlException : Exception
    {
        public InvalidDownloadUrlException(string message)
            : base(message) { }

        InvalidDownloadUrlException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}

using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [Serializable]
    public sealed class UploadFailedException : Exception
    {
        public UploadFailedException(string message)
            : base(message) { }

        UploadFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}

using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [Serializable]
    public sealed class InvalidUrlException : Exception
    {
        public InvalidUrlException(string message)
            : base(message) { }

        InvalidUrlException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public sealed class UploadFailedException : Exception
    {
        public UploadFailedException(string message)
            : base(message) { }

        UploadFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public sealed class CreateCollectionFailedException : Exception
    {
        public CreateCollectionFailedException(string message)
            : base(message) { }

        public CreateCollectionFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        CreateCollectionFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}

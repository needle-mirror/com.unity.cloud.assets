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
}

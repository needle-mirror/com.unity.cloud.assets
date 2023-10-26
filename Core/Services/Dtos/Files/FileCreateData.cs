using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class FileCreateData : FileBaseData, IFileCreateData
    {
        /// <inheritdoc />
        public string Path { get; set; }

        /// <inheritdoc />
        public long SizeBytes { get; set; }

        /// <inheritdoc />
        public string UserChecksum { get; set; }
    }
}

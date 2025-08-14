using System.Runtime.Serialization;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class LibraryData : ILibraryData
    {
        /// <inheritdoc />
        public AssetLibraryId Id { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public bool HasCollection { get; set; }

        public LibraryData() { }

        internal LibraryData(AssetLibraryId id)
        {
            Id = id;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IFileCreation : IFileUpdate
    {
        /// <summary>
        /// The path to the file.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The user metadata of the file.
        /// </summary>
        Dictionary<string, object> Metadata { get; }

        /// <summary>
        /// The system metadata of the file.
        /// </summary>
        Dictionary<string, object> SystemMetadata { get; }
    }

    interface IFileCreationWithDetails: IFileCreation
    {
        string UserChecksum { get; }
        long SizeBytes { get; }
    }
}

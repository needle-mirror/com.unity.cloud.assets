using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// File properties for creation.
    /// </summary>
    public interface IFileCreation
    {
        /// <summary>
        /// The path to the file.
        /// </summary>
        string Path { get; }

        /// <inheritdoc cref="IFile.Description"/>
        string Description { get; }

        /// <inheritdoc cref="IFile.Tags"/>
        IEnumerable<string> Tags { get; }

        /// <inheritdoc cref="IFile.Metadata"/>
        Dictionary<string, MetadataValue> Metadata { get; }
    }
}

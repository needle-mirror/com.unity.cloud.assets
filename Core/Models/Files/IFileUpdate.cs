using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// File properties for updating.
    /// </summary>
    public interface IFileUpdate
    {
        /// <inheritdoc cref="IFile.Description"/>
        string Description { get; }

        /// <inheritdoc cref="IFile.Tags"/>
        IEnumerable<string> Tags { get; }
    }
}

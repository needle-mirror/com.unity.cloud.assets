using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This object contains the information pertaining to an asset collection.
    /// </summary>
    public interface IAssetCollection
    {
        /// <summary>
        /// The organization in which the collection resides.
        /// </summary>
        IOrganization Organization { get; }

        /// <summary>
        /// The project in which the collection resides.
        /// </summary>
        IProject Project { get; }

        /// <summary>
        /// The name of the collection.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Describes the collection.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The path to the parent collection; can be empty.
        /// </summary>
        CollectionPath ParentPath { get; }

        /// <summary>
        /// The id of an associated catalog.
        /// </summary>
        string CatalogId { get; }

        /// <summary>
        /// Additional serialized information about the collection.
        /// </summary>
        Dictionary<string, IDeserializable> Metadata { get; set; }

        /// <summary>
        /// Implement this method to set the <see cref="Name"/> of the collection.
        /// </summary>
        /// <param name="name">The name of the collection. </param>
        void SetName(string name);

        /// <summary>
        /// Implement this method to set the <see cref="Description"/> of the collection.
        /// </summary>
        /// <param name="description">The description of the collection. </param>
        void SetDescription(string description);

        /// <summary>
        /// Returns the full path to the collection.
        /// </summary>
        /// <returns>A path. </returns>
        string GetFullCollectionPath();
    }
}

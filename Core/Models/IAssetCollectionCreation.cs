namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This object contains the information pertaining to an asset collection.
    /// </summary>
    public interface IAssetCollectionCreation
    {
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
        CollectionPath ParentPath { get; set; }
    }
}

namespace Unity.Cloud.Assets
{
    public interface IAssetCollectionUpdate
    {
        /// <inheritdoc cref="IAssetCollection.Name"/>
        string Name { get; }

        /// <inheritdoc cref="IAssetCollection.Description"/>
        string Description { get; }
    }
}

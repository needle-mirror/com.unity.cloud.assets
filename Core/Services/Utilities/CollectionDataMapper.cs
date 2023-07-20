namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Provides the methods necessary to map dtos to data models and vice versa for collections.
    /// </summary>
    static class CollectionDataMapper
    {
        /// <summary>
        /// Maps an <see cref="IAsset"/> to an <see cref="AssetInCollectionElementDto"/>.
        /// </summary>
        /// <param name="model">The <see cref="IAsset"/>.</param>
        /// <returns>An <see cref="AssetInCollectionElementDto"/>. </returns>
        internal static AssetInCollectionElementDto AssetToCollectionElementMapFrom(this IAsset model)
        {
            return new AssetInCollectionElementDto
            {
                StorageId = model.StorageId,
                AssetId = model.Id,
                Version = model.Version,
            };
        }
    }
}

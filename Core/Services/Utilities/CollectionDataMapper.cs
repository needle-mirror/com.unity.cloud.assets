namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Provides the methods necessary to map dtos to data models and vice versa for collections.
    /// </summary>
    static class CollectionDataMapper
    {
        /// <summary>
        /// Maps an <see cref="IAssetData"/> to an <see cref="AssetInCollectionData"/>.
        /// </summary>
        /// <param name="model">The <see cref="IAssetData"/>.</param>
        /// <returns>An <see cref="AssetInCollectionData"/>. </returns>
        internal static AssetInCollectionData AssetToCollectionElementMapFrom(this IAssetData model)
        {
            return new AssetInCollectionData
            {
                StorageId = model.StorageId,
                AssetId = model.Id.ToString(),
                Version = model.Version.ToString(),
            };
        }
    }
}

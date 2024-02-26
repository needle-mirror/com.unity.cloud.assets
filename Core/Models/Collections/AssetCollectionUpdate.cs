namespace Unity.Cloud.Assets
{
    public class AssetCollectionUpdate : IAssetCollectionUpdate
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }
    }
}

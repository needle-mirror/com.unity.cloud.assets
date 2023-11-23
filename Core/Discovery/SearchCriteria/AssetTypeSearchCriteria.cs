namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A simple Asset type search but wrapped in a type.
    /// </summary>
    public sealed class AssetTypeSearchCriteria : NullableSearchCriteria<AssetType>
    {
        /// <summary>
        /// The search key for the AssetType.
        /// </summary>
        public static string SearchKey => "primaryType";

        internal AssetTypeSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }

        protected override bool IsValidType(object input)
        {
            // A string is not valid if it cannot be parsed into an AssetType.
            return base.IsValidType(input) || (input is string && input.ToString().TryGetAssetTypeFromString(out _));
        }

        protected override object TransformValue(AssetType? value)
        {
            return value?.GetValueAsString()!;
        }

        protected override AssetType? TransformValue(object value)
        {
            if (value is AssetType assetType || value.ToString().TryGetAssetTypeFromString(out assetType))
            {
                return assetType;
            }

            return null;
        }
    }
}

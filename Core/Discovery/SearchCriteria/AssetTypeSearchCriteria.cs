namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A simple Asset type search but wrapped in a type.
    /// </summary>
    public sealed class AssetTypeSearchCriteria : SearchCriteria<AssetType>
    {
        /// <summary>
        /// The search key for the AssetType.
        /// </summary>
        public static string SearchKey => "primaryType";

        internal AssetTypeSearchCriteria(string propertyName, string searchKey, AssetType emptyValue = default)
            : base(propertyName, searchKey, emptyValue) { }
        protected override bool IsValidType(object input)
        {
            return input is AssetType || input is string || base.IsValidType(input);
        }

        protected override object TransformValue(AssetType value)
        {
            return value.GetValueAsString();
        }

        protected override AssetType TransformValue(object value)
        {
            if(value is AssetType assetType)
            {
                return assetType;
            }

            string valueAsStr = value?.ToString();
            if (!string.IsNullOrEmpty(valueAsStr))
            {
                return valueAsStr.GetAssetTypeFromString();
            }

            return AssetType.Other;
        }
    }
}

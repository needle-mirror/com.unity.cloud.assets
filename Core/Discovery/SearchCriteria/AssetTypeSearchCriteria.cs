using System.Linq;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A simple Asset type search but wrapped in a type.
    /// </summary>
    public sealed class AssetTypeSearchCriteria : StringSearchCriteria
    {
        /// <summary>
        /// The search key for the AssetType.
        /// </summary>
        public static string SearchKey => "primaryType";

        internal AssetTypeSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }

        protected override bool IsValidType(object input)
        {
            switch (input)
            {
                // A string is not valid if it cannot be broken up and parsed into an AssetType.
                case null:
                case AssetType:
                    return true;
                case string stringInput:
                {
                    var splitInput = stringInput.Split(k_SplitChar);
                    foreach (var inputString in splitInput)
                    {
                        if (!inputString.TryGetAssetTypeFromString(out _))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                default:
                    return false;
            }
        }

        public void Include(params AssetType[] assetTypes)
        {
            Include(string.Join(k_SplitChar, assetTypes.Select(x => x.GetValueAsString())));
        }

        public void Exclude(params AssetType[] assetTypes)
        {
            Exclude(string.Join(k_SplitChar, assetTypes.Select(x => x.GetValueAsString())));
        }

        public void ForAny(params AssetType[] assetTypes)
        {
            ForAny(string.Join(k_SplitChar, assetTypes.Select(x => x.GetValueAsString())));
        }
    }
}

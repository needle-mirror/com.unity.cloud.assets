using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This object contains the information pertaining to an asset collection.
    /// </summary>
    public class AssetCollectionCreation : IAssetCollectionCreation
    {
        public string Name { get; }
        public string Description { get; }
        public CollectionPath ParentPath { get; set; }
        public IDeserializable Metadata { get; set; }

        public AssetCollectionCreation(string name, string description)
        {
            VerifyArguments(name, description);

            Name = name;
            Description = description;
        }

        /// <summary>
        /// Verifies whether the input strings are valid.
        /// </summary>
        /// <param name="name">A string to verify. </param>
        /// <param name="description">A string to verify. </param>
        /// <exception cref="ArgumentNullException">This exception is thrown if the <paramref name="name"/> or <paramref name="description"/> are null or empty. </exception>
        internal static void VerifyArguments(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }
        }
    }
}

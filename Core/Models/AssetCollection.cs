using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This object contains the information pertaining to an asset collection stored on the cloud.
    /// </summary>
    [DataContract]
    public class AssetCollection : IAssetCollection
    {
        /// <inheritdoc/>
        public IOrganization Organization { get; set; }

        /// <inheritdoc/>
        public IProject Project { get; set; }

        /// <inheritdoc/>
        [DataMember(Name = "name")]
        public string Name { get; internal set; }

        /// <inheritdoc/>
        [DataMember(Name = "description")]
        public string Description { get; internal set; }

        /// <inheritdoc/>
        [DataMember(Name = "parentPath")]
        public CollectionPath ParentPath { get; internal set; }

        /// <inheritdoc/>
        [DataMember(Name = "catalogId")]
        public string CatalogId { get; internal set; }

        /// <inheritdoc/>
        [DataMember(Name = "metadata")]
        public Dictionary<string, IDeserializable> Metadata { get; set; }

        [JsonConstructor]
        internal AssetCollection()
        {
            Metadata = new Dictionary<string, IDeserializable>();
        }

        /// <summary>
        /// Creates and initializes a <see cref="AssetCollection"/>.
        /// </summary>
        /// <param name="name">The name of the collection. </param>
        /// <param name="description">The description of the collection. </param>
        /// <param name="parentPath">(Optional) The path to the parent collection. </param>
        /// <param name="metadata">(Optional) The metadata of the collection. </param>
        /// <exception cref="ArgumentNullException">This exception is thrown if the <paramref name="name"/> or <paramref name="description"/> are null or empty. </exception>
        public AssetCollection(string name, string description, string parentPath = null, Dictionary<string, IDeserializable> metadata = null)
        {
            VerifyArguments(name, description);

            Name = name;
            Description = description;
            ParentPath = new CollectionPath(parentPath);
            Metadata = metadata ?? new Dictionary<string, IDeserializable>();
        }

        /// <summary>
        /// Sets the <see cref="Name"/> of the collection.
        /// </summary>
        /// <param name="name">The name of the collection. </param>
        /// <exception cref="ArgumentNullException">This exception is thrown if the <paramref name="name"/> is null or empty. </exception>
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        /// <summary>
        /// Sets the <see cref="Description"/> of the collection.
        /// </summary>
        /// <param name="description">The description of the collection. </param>
        /// <exception cref="ArgumentNullException">This exception is thrown if the <paramref name="description"/> is null or empty. </exception>
        public void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            Description = description;
        }

        public string GetFullCollectionPath()
        {
            return CollectionPath.CombinePaths(ParentPath, Name);
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

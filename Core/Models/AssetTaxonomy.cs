using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class represents the taxonomy of an asset.
    /// </summary>
    [DataContract]
    public class AssetTaxonomy
    {
        internal AssetTaxonomy() { }

        /// <summary>
        /// Taxonomy Level 1.
        /// </summary>
        [DataMember(Name = "level1")]
        public string Level1 { get; set; }

        /// <summary>
        /// Taxonomy Level 2.
        /// </summary>
        [DataMember(Name = "level2")]
        public string Level2 { get; set; }

        /// <summary>
        /// Taxonomy Level 3.
        /// </summary>
        [DataMember(Name = "level3")]
        public string Level3 { get; set; }

        /// <summary>
        /// Taxonomy Level 4.
        /// </summary>
        [DataMember(Name = "level4")]
        public string Level4 { get; set; }

        /// <summary>
        /// Taxonomy Level 5.
        /// </summary>
        [DataMember(Name = "level5")]
        public string Level5 { get; set; }
    }
}

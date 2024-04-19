using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This struct contains the identifiers for a version label.
    /// </summary>
    public readonly struct VersionLabelDescriptor
    {
        /// <summary>
        /// The version label's organization genesis ID.
        /// </summary>
        public readonly OrganizationId OrganizationId;

        /// <summary>
        /// A unique name for the label. Uniqueness is scoped to the organization.
        /// </summary>
        public readonly string LabelName;

        /// <summary>
        /// Creates an instance of the <see cref="VersionLabelDescriptor"/> struct.
        /// </summary>
        /// <param name="organizationId">The version label's organization genesis ID.</param>
        /// <param name="labelName">The unique name of the version label.</param>
        public VersionLabelDescriptor(OrganizationId organizationId, string labelName)
        {
            OrganizationId = organizationId;
            LabelName = labelName;
        }

        /// <summary>
        /// Returns whether two <see cref="VersionLabelDescriptor"/> objects are equals.
        /// </summary>
        /// <param name="other">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(VersionLabelDescriptor other)
        {
            return OrganizationId.Equals(other.OrganizationId) &&
                LabelName.Equals(other.LabelName);
        }

        /// <summary>
        /// Validate <paramref name="obj"/> is a <see cref="VersionLabelDescriptor"/> instance and have the same values as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is VersionLabelDescriptor other && Equals(other);

        /// <summary>
        /// Compute a hash code for the object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <remarks>
        /// * You should not assume that equal hash codes imply object equality.
        /// * You should never persist or use a hash code outside the application domain in which it was created,
        ///   because the same object may hash differently across application domains, processes, and platforms.
        /// </remarks>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = OrganizationId.GetHashCode();
                hashCode = (hashCode * 397) ^ LabelName.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Get if two <see cref="VersionLabelDescriptor"/> represent the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances represent the same;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(VersionLabelDescriptor left, VersionLabelDescriptor right) => left.Equals(right);

        /// <summary>
        /// Get if two <see cref="VersionLabelDescriptor"/> does not represent the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances are not the same;
        /// <see langword="false"/> if both instances are the same.
        /// </returns>
        public static bool operator !=(VersionLabelDescriptor left, VersionLabelDescriptor right) => !left.Equals(right);
    }
}

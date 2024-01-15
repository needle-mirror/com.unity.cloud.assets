using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// The interface for defining a metadata value.
    /// </summary>
    public interface IMetadataValue
    {
        MetadataValueType ValueType { get; }

        /// <summary>
        /// Returns the value as a <see cref="bool"/>.
        /// </summary>
        /// <returns>A <see cref="bool"/>. </returns>
        /// <exception cref="FormatException">If the value is not parsable as a boolean. </exception>
        bool AsBoolean();

        /// <summary>
        /// Returns the value as a <see cref="double"/>.
        /// </summary>
        /// <returns>A <see cref="double"/>. </returns>
        /// <exception cref="FormatException">If the value is not parsable as a number. </exception>
        double AsNumber();

        /// <summary>
        /// Returns the value as a <see cref="DateTime"/>.
        /// </summary>
        /// <returns>A <see cref="DateTime"/>. </returns>
        /// <exception cref="FormatException">If the value is not parsable as a datetime. </exception>
        DateTime AsTimestamp();

        /// <summary>
        /// Returns the value as a <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>. </returns>
        string AsText();

        /// <summary>
        /// Returns the value as a <see cref="SingleSelectionMetadata"/> object.
        /// </summary>
        /// <returns>A <see cref="SingleSelectionMetadata"/> object containing the selected value. </returns>
        SingleSelectionMetadata AsSingleSelection();

        /// <summary>
        /// Returns the value as a <see cref="MultiSelectionMetadata"/> object.
        /// </summary>
        /// <returns>A <see cref="MultiSelectionMetadata"/> object containing a list of selected values. </returns>
        MultiSelectionMetadata AsMultiSelection();

        /// <summary>
        /// Returns the value as a <see cref="UrlMetadata"/> object.
        /// </summary>
        /// <returns>A <see cref="UrlMetadata"/> object containg the url. </returns>
        /// <exception cref="FormatException">If the value is not parsable as a url. </exception>
        UrlMetadata AsUrl();

        /// <summary>
        /// Returns the value as a user id.
        /// </summary>
        /// <returns>A string representing a user id. </returns>
        string AsUser();
    }
}

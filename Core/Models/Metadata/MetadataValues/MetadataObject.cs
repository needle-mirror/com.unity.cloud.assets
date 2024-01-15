using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class that represents a metadata value.
    /// </summary>
    public abstract class MetadataObject
    {
        /// <summary>
        /// Returns the value of the metadata.
        /// </summary>
        /// <returns>An object representing the value of the metadata. </returns>
        /// <remarks>Return values should be limited to the following types: <see cref="string"/>, <see cref="bool"/>, <see cref="DateTime"/>, <see cref="double"/> or other number types, and <c>IEnumerable</c> of string</remarks>
        public abstract object GetValue();

        /// <summary>
        /// Sets the value of the metadata field.
        /// </summary>
        /// <param name="value">The value to apply. </param>
        internal abstract void SetValue(object value);
    }
}

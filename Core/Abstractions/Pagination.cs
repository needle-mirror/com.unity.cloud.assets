using System;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This data structure contains the necessary information to create a page.
    /// </summary>
    public struct Pagination
    {
        /// <summary>
        /// Returns the sorting method for the items of the page.
        /// </summary>
        /// <value>A sorting method. </value>
        public string SortingField { get; }

        /// <summary>
        /// Returns the sorting method for the items of the page.
        /// </summary>
        /// <value>A sorting method. </value>
        public SortingOrder SortingOrder { get; }

        /// <summary>
        /// Returns the number of items per page.
        /// </summary>
        /// <value>A number greater than zero. </value>
        public Range Range { get; set; }

        /// <summary>
        /// Initializes and returns a <see cref="Pagination"/> object.
        /// </summary>
        /// <param name="range">The set of results to retrieve. </param>
        /// <param name="order">The order of the results based on the default sorting field. </param>
        public Pagination(Range range, SortingOrder order = SortingOrder.Ascending)
        {
            SortingField = nameof(IAsset.Name);
            SortingOrder = order;
            Range = range;
        }

        /// <summary>
        /// Initializes and returns a <see cref="Pagination"/> object.
        /// </summary>
        /// <param name="sortingField">The field to sort the elements of the page.</param>
        /// <param name="range">The set of results to retrieve. </param>
        /// <param name="order">The order of the results based on the <paramref name="sortingField"/>. </param>
        /// <exception cref="InvalidArgumentException">Throws if the <paramref name="sortingField"/> is a null or empty string. </exception>
        public Pagination(string sortingField, Range range, SortingOrder order = SortingOrder.Ascending)
        {
            if (string.IsNullOrWhiteSpace(sortingField))
            {
                throw new InvalidArgumentException($"{nameof(sortingField)} cannot be empty.");
            }

            SortingField = sortingField;
            SortingOrder = order;
            Range = range;
        }
    }
}

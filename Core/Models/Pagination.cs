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
        /// Returns the number of items per page.
        /// </summary>
        /// <value>A number greater than zero. </value>
        public int PageSize { get; set; }

        /// <summary>
        /// Implement this property to return the offset window over all items.
        /// </summary>
        /// <value>A number greater than zero. </value>
        public int PageNumber { get; set; }

        /// <summary>
        /// Initializes and returns a <see cref="Pagination"/> object.
        /// </summary>
        /// <param name="sortingField">The field to sort the elements of the page.</param>
        /// <param name="pageSize">The amount of elements per page. </param>
        /// <param name="pageNumber">The number of the page to fetch. </param>
        /// <exception cref="InvalidArgumentException">Throws if the <paramref name="sortingField"/> is a null or empty string. </exception>
        public Pagination(string sortingField, int pageSize = 0, int pageNumber = 1)
        {
            if (string.IsNullOrWhiteSpace(sortingField))
            {
                throw new InvalidArgumentException($"{nameof(sortingField)} cannot be empty.");
            }

            SortingField = sortingField;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }
    }
}

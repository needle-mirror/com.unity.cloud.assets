using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Implement this interface to manage a type of criteria for searches.
    /// </summary>
    public interface ISearchCriteria
    {
        /// <summary>
        /// Implement this property to return the key of the search element.
        /// </summary>
        string SearchKey { get; }

        /// <summary>
        /// Implement this property to return the type of the search element.
        /// </summary>
        Type SearchFieldType { get; }

        /// <summary>
        /// Implement this method to return the included search element.
        /// </summary>
        /// <param name="includedValue">The included search value. </param>
        /// <returns>True if the criteria should be included in the search, false otherwise. </returns>
        bool TryGetIncluded(out object includedValue);

        /// <summary>
        /// Implement this method to return the included search element.
        /// </summary>
        /// <param name="excludedValue">The excluded search value. </param>
        /// <returns>True if the criteria should be included in the search, false otherwise. </returns>
        bool TryGetExcluded(out object excludedValue);

        /// <summary>
        /// Implement this method to return the included search element.
        /// </summary>
        /// <param name="anyValue">The optional search value. </param>
        /// <returns>True if the criteria should be included in the search, false otherwise. </returns>
        bool TryGetAny(out object anyValue);

        /// <summary>
        /// Implement this method to explicitly include results presenting the <paramref name="value"/> in a search.
        /// </summary>
        /// <param name="value">A required value for a search. </param>
        void Include(object value);

        /// <summary>
        /// Implement this method to add the included value to the collection of <paramref name="includedValues"/>.
        /// </summary>
        /// <param name="includedValues">The collection in which to add a value to include in the search. </param>
        /// <param name="prefix">A prefix for the <see cref="SearchKey"/>; may be empty. </param>
        void Include(Dictionary<string, object> includedValues, string prefix = "");

        /// <summary>
        /// Implement this method to explicitly exclude results presenting the <paramref name="value"/> in a search.
        /// </summary>
        /// <param name="value">A value to exclude from a search. </param>
        void Exclude(object value);

        /// <summary>
        /// Implement this method to add the included value to the collection of <paramref name="excludedValues"/>.
        /// </summary>
        /// <param name="excludedValues">The collection in which to add a value to exclude from the search. </param>
        /// <param name="prefix">A prefix for the <see cref="SearchKey"/>; may be empty. </param>
        void Exclude(Dictionary<string, object> excludedValues, string prefix = "");

        /// <summary>
        /// Implement this method to include results presenting the <paramref name="value"/> in a search.
        /// </summary>
        /// <param name="value">An optional value for a search. </param>
        void ForAny(object value);

        /// <summary>
        /// Implement this method to add the included value to the collection of <paramref name="forAnyValues"/>.
        /// </summary>
        /// <param name="forAnyValues">The collection in which to add an optional value for the search. </param>
        /// <param name="prefix">A prefix for the <see cref="SearchKey"/>; may be empty. </param>
        void ForAny(Dictionary<string, object> forAnyValues, string prefix = "");

        /// <summary>
        /// Implement this method to verify whether an input satisfies the search criteria.
        /// </summary>
        /// <param name="input">The element to compare to the criteria. </param>
        /// <returns>True if the <paramref name="input"/> satisfies the criteria, false otherwise. </returns>
        bool IsMatch(object input);

        /// <summary>
        /// Implement this method to verify whether the input satisfies the optional search critiera.
        /// </summary>
        /// <param name="input">The element to compare to the criteria. </param>
        /// <returns>True if the <paramref name="input"/> satisfies the criteria, false otherwise. </returns>
        bool IsAny(object input);

        /// <summary>
        /// Implement this method to return whether the criteria has been populated.
        /// </summary>
        /// <returns>True if there are no requirements to meet, false otherwise. </returns>
        bool IsEmpty();

        /// <summary>
        /// Implement this method to clear the criteria fields.
        /// </summary>
        void Clear();
    }

    public interface ISearchCriteria<in T> : ISearchCriteria
    {
        /// <summary>
        /// Implement this method to explicitly include results presenting the <paramref name="value"/> in a search.
        /// </summary>
        /// <param name="value">A required value for a search. </param>
        void Include(T value);

        /// <summary>
        /// Implement this method to explicitly exclude results presenting the <paramref name="value"/> in a search.
        /// </summary>
        /// <param name="value">A value to exclude from a search. </param>
        void Exclude(T value);

        /// <summary>
        /// Implement this method to include results presenting the <paramref name="value"/> in a search.
        /// </summary>
        /// <param name="value">An optional value for a search. </param>
        void ForAny(T value);
    }
}

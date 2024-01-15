using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public abstract class SearchCriteriaBase : ISearchCriteria
    {
        readonly string m_PropertyName;
        readonly string m_SearchKey;
        readonly Type m_FieldType;

        /// <inheritdoc/>
        string ISearchCriteria.PropertyName => m_PropertyName;

        /// <inheritdoc/>
        Type ISearchCriteria.SearchFieldType => m_FieldType;

        protected SearchCriteriaBase(string propertyName, string searchKey, Type fieldType)
        {
            m_PropertyName = propertyName;
            m_SearchKey = searchKey;
            m_FieldType = fieldType;
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetIncluded(out object includedValue) => TryGetIncluded(out includedValue);

        /// <inheritdoc cref="ISearchCriteria.TryGetIncluded"/>
        protected abstract bool TryGetIncluded(out object includedValue);

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetExcluded(out object excludedValue) => TryGetExcluded(out excludedValue);

        /// <inheritdoc cref="ISearchCriteria.TryGetExcluded"/>
        protected abstract bool TryGetExcluded(out object excludedValue);

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetAny(out object anyValue) => TryGetAny(out anyValue);

        /// <inheritdoc cref="ISearchCriteria.TryGetAny"/>
        protected abstract bool TryGetAny(out object anyValue);

        /// <inheritdoc/>
        void ISearchCriteria.Include(object value) => Include(value);

        /// <inheritdoc cref="ISearchCriteria.Include(object)"/>
        protected abstract void Include(object value);

        /// <inheritdoc/>
        void ISearchCriteria.Include(Dictionary<string, object> includedValues, string prefix)
        {
            if (TryGetIncluded(out var value))
            {
                includedValues.Add(m_SearchKey.BuildSearchKey(prefix), value);
            }
        }

        /// <inheritdoc/>
        void ISearchCriteria.Exclude(object value) => Exclude(value);

        /// <inheritdoc cref="ISearchCriteria.Exclude(object)"/>
        protected abstract void Exclude(object value);

        /// <inheritdoc/>
        void ISearchCriteria.Exclude(Dictionary<string, object> excludedValues, string prefix)
        {
            if (TryGetExcluded(out var value))
            {
                excludedValues.Add(m_SearchKey.BuildSearchKey(prefix), value);
            }
        }

        /// <inheritdoc/>
        void ISearchCriteria.ForAny(object value) => ForAny(value);

        /// <inheritdoc cref="ISearchCriteria.ForAny(object)"/>
        protected abstract void ForAny(object value);

        /// <inheritdoc/>
        void ISearchCriteria.ForAny(Dictionary<string, object> forAnyValues, string prefix)
        {
            if (TryGetAny(out var value))
            {
                forAnyValues.Add(m_SearchKey.BuildSearchKey(prefix), value);
            }
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsMatch(object input)
        {
            return IsValidType(input) && SatisfiesMatch(input);
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsAny(object input)
        {
            return IsValidType(input) && SatisfiesAny(input);
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsEmpty() => IsEmpty();

        /// <inheritdoc cref="ISearchCriteria.IsEmpty"/>
        protected abstract bool IsEmpty();

        /// <inheritdoc/>
        public abstract void Clear();

        protected abstract bool IsValidType(object input);

        protected abstract bool SatisfiesMatch(object input);

        protected abstract bool SatisfiesAny(object input);
    }
}

#nullable enable
using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class NullableSearchCriteria<T> : ISearchCriteria<T?> where T : struct
    {
        readonly string m_PropertyName;
        readonly string m_SearchKey;
        T? m_Included;
        T? m_Excluded;
        T? m_Any;

        /// <inheritdoc/>
        string ISearchCriteria.PropertyName => m_PropertyName;

        /// <inheritdoc/>
        Type ISearchCriteria.SearchFieldType => typeof(T?);

        internal NullableSearchCriteria(string propertyName, string searchKey)
        {
            m_PropertyName = propertyName;
            m_SearchKey = searchKey;
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetIncluded(out object includedValue)
        {
            includedValue = m_Included ?? default(T);
            return m_Included.HasValue;
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetExcluded(out object excludedValue)
        {
            excludedValue = m_Excluded ?? default(T);
            return m_Excluded.HasValue;
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetAny(out object anyValue)
        {
            anyValue = m_Any ?? default(T);
            return m_Any.HasValue;
        }

        /// <inheritdoc/>
        void ISearchCriteria.Include(object value) => Include((T) value);

        /// <inheritdoc/>
        void ISearchCriteria.Include(Dictionary<string, object> includedValues, string prefix)
        {
            if (this.TryGetIncluded(out var value))
            {
                includedValues.Add(m_SearchKey.BuildSearchKey(prefix), value);
            }
        }

        /// <inheritdoc/>
        void ISearchCriteria.Exclude(object value) => Exclude((T) value);

        /// <inheritdoc/>
        void ISearchCriteria.Exclude(Dictionary<string, object> excludedValues, string prefix)
        {
            if (this.TryGetExcluded(out var value))
            {
                excludedValues.Add(m_SearchKey.BuildSearchKey(prefix), value);
            }
        }

        /// <inheritdoc/>
        void ISearchCriteria.ForAny(Dictionary<string, object> forAnyValues, string prefix)
        {
            if (this.TryGetAny(out var value))
            {
                forAnyValues.Add(m_SearchKey.BuildSearchKey(prefix), value);
            }
        }

        /// <inheritdoc/>
        void ISearchCriteria.ForAny(object value) => ForAny((T) value);

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
        bool ISearchCriteria.IsEmpty()
        {
            return !m_Included.HasValue && !m_Excluded.HasValue && !m_Any.HasValue;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            m_Included = null;
            m_Excluded = null;
            m_Any = null;
        }

        /// <inheritdoc/>
        public void Include(T? value)
        {
            m_Included = value;
        }

        /// <inheritdoc/>
        public void Exclude(T? value)
        {
            m_Excluded = value;
        }

        /// <inheritdoc/>
        public void ForAny(T? value)
        {
            m_Any = value;
        }

        protected virtual bool IsValidType(object input)
        {
            return input is T;
        }

        protected virtual bool SatisfiesMatch(object input)
        {
            return (!m_Included.HasValue || m_Included.Equals(input))
                && (!m_Excluded.HasValue || !m_Excluded.Equals(input));
        }

        protected virtual bool SatisfiesAny(object input)
        {
            return m_Any.HasValue && m_Any.Equals(input);
        }
    }
}

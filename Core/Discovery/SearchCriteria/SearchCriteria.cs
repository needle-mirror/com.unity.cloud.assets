using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class SearchCriteria<T> : ISearchCriteria<T>
    {
        readonly string m_PropertyName;
        readonly string m_SearchKey;
        private protected T m_Included;
        private protected T m_Excluded;
        private protected T m_Any;

        readonly T m_EmpyValue;

        /// <inheritdoc/>
        string ISearchCriteria.PropertyName => m_PropertyName;

        /// <inheritdoc/>
        Type ISearchCriteria.SearchFieldType => typeof(T);

        internal SearchCriteria(string propertyName, string searchKey, T emptyValue = default)
        {
            m_PropertyName = propertyName;
            m_SearchKey = searchKey;
            m_EmpyValue = emptyValue;
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetIncluded(out object includedValue)
        {
            includedValue = TransformValue(m_Included);
            return !IsValueEmpty(m_Included);
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetExcluded(out object excludedValue)
        {
            excludedValue = TransformValue(m_Excluded);
            return !IsValueEmpty(m_Excluded);
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetAny(out object anyValue)
        {
            anyValue = TransformValue(m_Any);
            return !IsValueEmpty(m_Any);
        }

        /// <inheritdoc/>
        void ISearchCriteria.Include(object value) => Include(TransformValue(value));

        /// <inheritdoc/>
        void ISearchCriteria.Include(Dictionary<string, object> includedValues, string prefix)
        {
            if (this.TryGetIncluded(out var value))
            {
                includedValues.Add(m_SearchKey.BuildSearchKey(prefix), value);
            }
        }

        /// <inheritdoc/>
        void ISearchCriteria.Exclude(object value) => Exclude(TransformValue(value));

        /// <inheritdoc/>
        void ISearchCriteria.Exclude(Dictionary<string, object> excludedValues, string prefix)
        {
            if (this.TryGetExcluded(out var value))
            {
                excludedValues.Add(m_SearchKey.BuildSearchKey(prefix), value);
            }
        }

        /// <inheritdoc/>
        void ISearchCriteria.ForAny(object value) => ForAny(TransformValue(value));

        /// <inheritdoc/>
        void ISearchCriteria.ForAny(Dictionary<string, object> forAnyValues, string prefix)
        {
            if (this.TryGetAny(out var value))
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
        bool ISearchCriteria.IsEmpty()
        {
            return IsValueEmpty(m_Included) && IsValueEmpty(m_Excluded) && IsValueEmpty(m_Any);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            m_Included = default;
            m_Excluded = default;
            m_Any = default;
        }

        /// <inheritdoc/>
        public void Include(T value)
        {
            m_Included = value;
        }

        /// <inheritdoc/>
        public void Exclude(T value)
        {
            m_Excluded = value;
        }

        /// <inheritdoc/>
        public void ForAny(T value)
        {
            m_Any = value;
        }

        protected virtual bool IsValidType(object input)
        {
            return input is null or T;
        }

        protected virtual bool IsValueEmpty(T value)
        {
            if (value is string s)
            {
                return string.IsNullOrWhiteSpace(s);
            }

            return value == null || value.Equals(m_EmpyValue);
        }

        protected virtual object TransformValue(T value)
        {
            return value;
        }

        protected virtual T TransformValue(object value)
        {
            return (T) value;
        }

        protected virtual bool SatisfiesMatch(object input)
        {
            return (IsValueEmpty(m_Included) || m_Included.Equals(input))
                && (IsValueEmpty(m_Excluded) || !m_Excluded.Equals(input));
        }

        protected virtual bool SatisfiesAny(object input)
        {
            return !IsValueEmpty(m_Any) && m_Any.Equals(input);
        }
    }
}

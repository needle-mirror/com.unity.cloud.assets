using System;

namespace Unity.Cloud.Assets
{
    public class SearchCriteria<T> : SearchCriteriaBase, ISearchCriteria<T>
    {
        private protected T m_Included;
        private protected T m_Excluded;
        private protected T m_Any;

        internal SearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey, typeof(T)) { }

        /// <inheritdoc/>
        protected override bool TryGetIncluded(out object includedValue)
        {
            includedValue = TransformValue(m_Included);
            return !IsValueEmpty(m_Included);
        }

        /// <inheritdoc/>
        protected override bool TryGetExcluded(out object excludedValue)
        {
            excludedValue = TransformValue(m_Excluded);
            return !IsValueEmpty(m_Excluded);
        }

        /// <inheritdoc/>
        protected override bool TryGetAny(out object anyValue)
        {
            anyValue = TransformValue(m_Any);
            return !IsValueEmpty(m_Any);
        }

        /// <inheritdoc/>
        protected override void Include(object value) => Include(TransformValue(value));

        /// <inheritdoc/>
        protected override void Exclude(object value) => Exclude(TransformValue(value));

        /// <inheritdoc/>
        protected override void ForAny(object value) => ForAny(TransformValue(value));

        protected override bool IsEmpty()
        {
            return IsValueEmpty(m_Included) && IsValueEmpty(m_Excluded) && IsValueEmpty(m_Any);
        }

        /// <inheritdoc/>
        public override void Clear()
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

        protected override bool IsValidType(object input)
        {
            return input is null or T;
        }

        protected virtual bool IsValueEmpty(T value)
        {
            if (value is string s)
            {
                return string.IsNullOrWhiteSpace(s);
            }

            return value == null || value.Equals(default(T));
        }

        protected virtual object TransformValue(T value)
        {
            return value;
        }

        protected virtual T TransformValue(object value)
        {
            return (T) value;
        }

        protected override bool SatisfiesMatch(object input)
        {
            return (IsValueEmpty(m_Included) || m_Included.Equals(input))
                && (IsValueEmpty(m_Excluded) || !m_Excluded.Equals(input));
        }

        protected override bool SatisfiesAny(object input)
        {
            return !IsValueEmpty(m_Any) && m_Any.Equals(input);
        }
    }
}

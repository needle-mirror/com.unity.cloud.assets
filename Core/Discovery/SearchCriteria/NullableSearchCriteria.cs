#nullable enable
using System;

namespace Unity.Cloud.Assets
{
    public class NullableSearchCriteria<T> : SearchCriteriaBase, ISearchCriteria<T?> where T : struct
    {
        T? m_Included;
        T? m_Excluded;
        T? m_Any;

        internal NullableSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey, typeof(T?)) { }

        /// <inheritdoc/>
        protected override bool TryGetIncluded(out object includedValue)
        {
            includedValue = TransformValue(m_Included);
            return m_Included.HasValue;
        }

        /// <inheritdoc/>
        protected override bool TryGetExcluded(out object excludedValue)
        {
            excludedValue = TransformValue(m_Excluded);
            return m_Excluded.HasValue;
        }

        /// <inheritdoc/>
        protected override bool TryGetAny(out object anyValue)
        {
            anyValue = TransformValue(m_Any);
            return m_Any.HasValue;
        }

        /// <inheritdoc/>
        protected override void Include(object value) => Include(TransformValue(value));

        /// <inheritdoc/>
        protected override void Exclude(object value) => Exclude(TransformValue(value));

        /// <inheritdoc/>
        protected override void ForAny(object value) => ForAny(TransformValue(value));

        /// <inheritdoc/>
        protected override bool IsEmpty()
        {
            return !m_Included.HasValue && !m_Excluded.HasValue && !m_Any.HasValue;
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            m_Included = null;
            m_Excluded = null;
            m_Any = null;
        }

        protected override bool IsValidType(object input)
        {
            return input is null or T;
        }

        protected override bool SatisfiesMatch(object input)
        {
            return (!m_Included.HasValue || m_Included.Equals(input))
                && (!m_Excluded.HasValue || !m_Excluded.Equals(input));
        }

        protected override bool SatisfiesAny(object input)
        {
            return m_Any.HasValue && m_Any.Equals(input);
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

        protected virtual object TransformValue(T? value)
        {
            return value ?? default(T);
        }

        protected virtual T? TransformValue(object value)
        {
            return (T) value;
        }
    }
}

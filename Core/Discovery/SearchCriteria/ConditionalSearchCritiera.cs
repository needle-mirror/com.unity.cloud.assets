using System;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    public sealed class ConditionalSearchCriteria<T> : SearchCriteriaBase, ISearchCriteria
    {
        readonly SearchConditionData m_Included;
        readonly SearchConditionData m_Excluded;
        readonly SearchConditionData m_Any;

        internal ConditionalSearchCriteria(string propertyName, string searchKey, string type)
            : base(propertyName, searchKey, typeof(SearchConditionData))
        {
            m_Included = new SearchConditionData(type);
            m_Excluded = new SearchConditionData(type);
            m_Any = new SearchConditionData(type);
        }

        /// <inheritdoc/>
        protected override bool TryGetIncluded(out object includedValue)
        {
            m_Included.Clean();
            includedValue = m_Included;
            return !IsValueEmpty(m_Included);
        }

        /// <inheritdoc/>
        protected override bool TryGetExcluded(out object excludedValue)
        {
            m_Excluded.Clean();
            excludedValue = m_Excluded;
            return !IsValueEmpty(m_Excluded);
        }

        /// <inheritdoc/>
        protected override bool TryGetAny(out object anyValue)
        {
            m_Any.Clean();
            anyValue = m_Any;
            return !IsValueEmpty(m_Any);
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
            return IsValueEmpty(m_Included) && IsValueEmpty(m_Excluded) && IsValueEmpty(m_Any);
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            m_Included.Conditions.Clear();
            m_Excluded.Conditions.Clear();
            m_Any.Conditions.Clear();
        }

        /// <inheritdoc/>
        protected override bool SatisfiesMatch(object input)
        {
            return (IsValueEmpty(m_Included) || m_Included.SatisfiesAllConditions(input))
                && (IsValueEmpty(m_Excluded) || !m_Excluded.SatisfiesAllConditions(input));
        }

        protected override bool IsValidType(object input)
        {
            return true; // Each condition will determine validity.
        }

        /// <inheritdoc/>
        protected override bool SatisfiesAny(object input)
        {
            return !IsValueEmpty(m_Any) && m_Any.SatistiesAnyCondition(input);
        }

        void Include(SearchConditionValue value)
        {
            m_Included.AddCondition(value);
        }

        public void Include(SearchConditionType type, T value)
        {
            Include(new SearchConditionValue(type, value));
        }

        void Exclude(SearchConditionValue value)
        {
            m_Excluded.AddCondition(value);
        }

        public void Exclude(SearchConditionType type, T value)
        {
            Exclude(new SearchConditionValue(type, value));
        }

        void ForAny(SearchConditionValue value)
        {
            m_Any.AddCondition(value);
        }

        public void ForAny(SearchConditionType type, T value)
        {
            ForAny(new SearchConditionValue(type, value));
        }

        static bool IsValueEmpty(SearchConditionData value)
        {
            return value.Conditions.Count == 0;
        }

        static SearchConditionValue TransformValue(object value)
        {
            return value switch
            {
                null => null,
                T tValue => new SearchConditionValue(SearchConditionType.GreaterThanOrEqual, tValue),
                SearchConditionValue scv => scv,
                string s => IsolatedSerialization.Deserialize<SearchConditionValue>(s, IsolatedSerialization.defaultSettings),
                _ => throw new InvalidArgumentException($"ConditionalSearchCriteria can only filter {nameof(SearchConditionValue)} or string.")
            };
        }
    }
}

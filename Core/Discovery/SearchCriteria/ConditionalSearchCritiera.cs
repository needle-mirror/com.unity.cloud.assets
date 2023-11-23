using System;
using System.Collections.Generic;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    public sealed class ConditionalSearchCriteria<T> : ISearchCriteria
    {
        readonly string m_PropertyName;
        readonly string m_SearchKey;
        readonly SearchConditionData m_Included;
        readonly SearchConditionData m_Excluded;
        readonly SearchConditionData m_Any;

        /// <inheritdoc/>
        string ISearchCriteria.PropertyName => m_PropertyName;

        /// <inheritdoc/>
        Type ISearchCriteria.SearchFieldType => typeof(SearchConditionData);

        internal ConditionalSearchCriteria(string propertyName, string searchKey, string type)
        {
            m_PropertyName = propertyName;
            m_SearchKey = searchKey;
            m_Included = new SearchConditionData(type);
            m_Excluded = new SearchConditionData(type);
            m_Any = new SearchConditionData(type);
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetIncluded(out object includedValue)
        {
            m_Included.Clean();
            includedValue = IsolatedSerialization.Serialize(m_Included, IsolatedSerialization.defaultSettings);
            return !IsValueEmpty(m_Included);
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetExcluded(out object excludedValue)
        {
            m_Excluded.Clean();
            excludedValue = IsolatedSerialization.Serialize(m_Excluded, IsolatedSerialization.defaultSettings);
            return !IsValueEmpty(m_Excluded);
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetAny(out object anyValue)
        {
            m_Any.Clean();
            anyValue = IsolatedSerialization.Serialize(m_Any, IsolatedSerialization.defaultSettings);
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
            return (IsValueEmpty(m_Included) || m_Included.SatisfiesConditions(input))
                && (IsValueEmpty(m_Excluded) || !m_Excluded.SatisfiesConditions(input));
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsAny(object input)
        {
            return !IsValueEmpty(m_Any) && m_Any.SatisfiesConditions(input);
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsEmpty()
        {
            return IsValueEmpty(m_Included) && IsValueEmpty(m_Excluded) && IsValueEmpty(m_Any);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            m_Included.Conditions.Clear();
            m_Excluded.Conditions.Clear();
            m_Any.Conditions.Clear();
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
                _ => throw new InvalidArgumentException($"ConditionalSearchCriteria can only filter SearchConditionValue or string.")
            };
        }
    }
}

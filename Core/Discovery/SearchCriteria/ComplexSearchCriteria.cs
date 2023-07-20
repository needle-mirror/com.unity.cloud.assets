using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining criteria which represent a reference type.
    /// </summary>
    public abstract class ComplexSearchCriteria<T> : ISearchCriteria
    {
        readonly ISearchCriteria[] m_AllCriteria;

        public abstract string SearchKey { get; }
        public virtual Type SearchFieldType => typeof(T);

        private protected virtual Type InstantiatedType => typeof(T);

        private protected virtual bool IncludeInSearch => true;

        private protected ComplexSearchCriteria()
        {
            m_AllCriteria = GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => typeof(ISearchCriteria).IsAssignableFrom(x.PropertyType))
                .Select(x => x.GetValue(this) as ISearchCriteria)
                .ToArray();
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetIncluded(out object includedValue)
        {
            includedValue = Activator.CreateInstance(InstantiatedType);

            var isValid = false;
            foreach (var criterion in m_AllCriteria)
            {
                if (criterion.TryGetIncluded(out var value))
                {
                    isValid = true;
                    SearchFieldType.GetProperty(criterion.SearchKey)?.SetValue(includedValue, value);
                }
            }

            return isValid;
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetExcluded(out object excludedValue)
        {
            excludedValue = Activator.CreateInstance(InstantiatedType);

            var isValid = false;
            foreach (var criterion in m_AllCriteria)
            {
                if (criterion.TryGetExcluded(out var value))
                {
                    isValid = true;
                    SearchFieldType.GetProperty(criterion.SearchKey)?.SetValue(excludedValue, value);
                }
            }

            return isValid;
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetAny(out object anyValue)
        {
            anyValue = Activator.CreateInstance(InstantiatedType);

            var isValid = false;
            foreach (var criterion in m_AllCriteria)
            {
                if (criterion.TryGetAny(out var value))
                {
                    isValid = true;
                    SearchFieldType.GetProperty(criterion.SearchKey)?.SetValue(anyValue, value);
                }
            }

            return isValid;
        }

        /// <inheritdoc/>
        void ISearchCriteria.Include(object value) => Include(value);

        /// <inheritdoc/>
        void ISearchCriteria.Include(Dictionary<string, object> includedValues, string prefix)
        {
            if (!IncludeInSearch) return;

            var searchKey = BuildSearchKeyPrefix();
            foreach (var criterion in m_AllCriteria)
            {
                criterion.Include(includedValues, searchKey);
            }
        }

        /// <inheritdoc/>
        void ISearchCriteria.Exclude(object value) => Exclude(value);

        /// <inheritdoc/>
        void ISearchCriteria.Exclude(Dictionary<string, object> excludedValues, string prefix)
        {
            if (!IncludeInSearch) return;

            var searchKey = BuildSearchKeyPrefix();
            foreach (var criterion in m_AllCriteria)
            {
                criterion.Exclude(excludedValues, searchKey);
            }
        }

        /// <inheritdoc/>
        void ISearchCriteria.ForAny(object value) => ForAny(value);

        /// <inheritdoc/>
        void ISearchCriteria.ForAny(Dictionary<string, object> forAnyValues, string prefix)
        {
            if (!IncludeInSearch) return;

            var searchKey = BuildSearchKeyPrefix();
            foreach (var criterion in m_AllCriteria)
            {
                criterion.ForAny(forAnyValues, searchKey);
            }
        }

        protected virtual string BuildSearchKeyPrefix()
        {
            return string.IsNullOrEmpty(SearchKey) ? "" : SearchKey + ".";
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsMatch(object input)
        {
            if (input == null) return IsEmpty();

            if (input is IList inputCollection)
            {
                foreach (var entry in inputCollection)
                {
                    if (IsMatch(entry)) return true;
                }

                return IsEmpty();
            }

            return IsMatch(input);
        }

        protected virtual bool IsMatch(object input)
        {
            foreach (var criterion in m_AllCriteria)
            {
                var value = input.GetPropertyValue(criterion.SearchKey);
                if (!criterion.IsMatch(value)) return false;
            }

            return true;
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsAny(object input)
        {
            return input != null && IsAny(input);
        }

        protected virtual bool IsAny(object input)
        {
            foreach (var criterion in m_AllCriteria)
            {
                var value = input.GetPropertyValue(criterion.SearchKey);
                if (criterion.IsAny(value)) return true;
            }

            return false;
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsEmpty() => IsEmpty();

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (var criterion in m_AllCriteria)
            {
                criterion.Clear();
            }
        }

        private protected virtual void Include(object value)
        {
            if (value is T tValue)
            {
                Include(tValue);
            }
        }

        private protected virtual void Exclude(object value)
        {
            if (value is T tValue)
            {
                Exclude(tValue);
            }
        }

        private protected virtual void ForAny(object value)
        {
            if (value is T tValue)
            {
                ForAny(tValue);
            }
        }

        public virtual void Include(T value)
        {
            if (value == null) return;

            foreach (var criterion in m_AllCriteria)
            {
                criterion.Include(value.GetPropertyValue(criterion.SearchKey));
            }
        }

        public virtual void Exclude(T value)
        {
            if (value == null) return;

            foreach (var criterion in m_AllCriteria)
            {
                criterion.Exclude(value.GetPropertyValue(criterion.SearchKey));
            }
        }

        public virtual void ForAny(T value)
        {
            if (value == null) return;

            foreach (var criterion in m_AllCriteria)
            {
                criterion.ForAny(value.GetPropertyValue(criterion.SearchKey));
            }
        }

        bool IsEmpty()
        {
            foreach (var criterion in m_AllCriteria)
            {
                if (!criterion.IsEmpty()) return false;
            }

            return true;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining criteria which represent a reference type.
    /// </summary>
    public abstract class ComplexSearchCriteria<T> : ISearchCriteria
    {
        readonly ISearchCriteria[] m_AllCriteria;

        readonly string m_PropertyName;
        readonly string m_SearchKey;

        string ISearchCriteria.PropertyName => m_PropertyName;
        public virtual Type SearchFieldType => typeof(T);

        private protected virtual Type InstantiatedType => typeof(T);

        private protected virtual bool IncludeInSearch => true;

        private protected ComplexSearchCriteria(string propertyName, string searchKey)
        {
            m_PropertyName = propertyName;
            m_SearchKey = searchKey;
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
                    SearchFieldType.GetProperty(criterion.PropertyName)?.SetValue(includedValue, value);
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
                    SearchFieldType.GetProperty(criterion.PropertyName)?.SetValue(excludedValue, value);
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
                    SearchFieldType.GetProperty(criterion.PropertyName)?.SetValue(anyValue, value);
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

            var searchKey = m_SearchKey.BuildSearchKey(prefix);
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

            var searchKey = m_SearchKey.BuildSearchKey(prefix);
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

            var searchKey = m_SearchKey.BuildSearchKey(prefix);
            foreach (var criterion in m_AllCriteria)
            {
                criterion.ForAny(forAnyValues, searchKey);
            }
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
                var value = input.GetPropertyValue(criterion.PropertyName);
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
                var value = input.GetPropertyValue(criterion.PropertyName);
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
            Include((T) value);
        }

        private protected virtual void Exclude(object value)
        {
            Exclude((T) value);
        }

        private protected virtual void ForAny(object value)
        {
            ForAny((T) value);
        }

        public void Include(T value)
        {
            if (value == null) return;

            foreach (var criterion in m_AllCriteria)
            {
                criterion.Include(value.GetPropertyValue(criterion.PropertyName));
            }
        }

        public void Exclude(T value)
        {
            if (value == null) return;

            foreach (var criterion in m_AllCriteria)
            {
                criterion.Exclude(value.GetPropertyValue(criterion.PropertyName));
            }
        }

        public void ForAny(T value)
        {
            if (value == null) return;

            foreach (var criterion in m_AllCriteria)
            {
                criterion.ForAny(value.GetPropertyValue(criterion.PropertyName));
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

using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public sealed class ConditionalSearchCriteria<T> : BaseSearchCriteria
    {
        readonly SearchConditionData m_Included;

        internal ConditionalSearchCriteria(string propertyName, string searchKey, string type)
            : base(propertyName, searchKey)
        {
            m_Included = new SearchConditionData(type);
        }

        internal override void Include(Dictionary<string, object> includedValues, string prefix = "")
        {
            m_Included.Clean();
            if (m_Included.Conditions.Count > 0)
            {
                includedValues.Add(SearchKey.BuildSearchKey(prefix), m_Included);
            }
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            m_Included.Conditions.Clear();
        }

        /// <summary>
        /// Sets the value of the conditional criteria.
        /// </summary>
        /// <param name="range">The range to consider. </param>
        /// <param name="value">The threshold value. </param>
        public void WithValue(SearchConditionRange range, T value)
        {
            m_Included.AddCondition(new SearchConditionValue(range, value));
        }
    }
}

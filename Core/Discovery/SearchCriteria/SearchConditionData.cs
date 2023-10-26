using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    sealed class SearchConditionData
    {
        [DataMember(Name = "type")]
        public string Type { get; private set; }

        [DataMember(Name = "conditions")]
        public List<SearchConditionValue> Conditions { get; private set; } = new();

        public SearchConditionData(string type)
        {
            Type = type;
        }

        public void Clean()
        {
            Conditions.RemoveAll(x => x.IsEmpty());
        }

        public void AddCondition(SearchConditionValue conditionValue)
        {
            if (conditionValue == null) return;

            var existingCondition = Conditions.FirstOrDefault(x=> x.Type == conditionValue.Type);
            if (existingCondition != null)
            {
                existingCondition.Value = conditionValue.Value;
            }
            else
            {
                Conditions.Add(conditionValue);
            }
        }

        public bool SatisfiesConditions(object value)
        {
            return true;
            // TODO
        }
    }

    [DataContract]
    public class SearchConditionValue
    {
        [DataMember(Name = "conditionType")]
        public string Type { get; private set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }

        public SearchConditionValue(SearchConditionType conditionType, object value)
        {
            Type = conditionType.ToString();
            Value = value.ToString();
        }

        internal bool IsEmpty()
        {
            return string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(Value);
        }
    }
}

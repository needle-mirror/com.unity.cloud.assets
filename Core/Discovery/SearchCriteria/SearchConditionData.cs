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

            var existingCondition = Conditions.FirstOrDefault(x => x.Type == conditionValue.Type);
            if (existingCondition != null)
            {
                existingCondition.Value = conditionValue.Value;
            }
            else
            {
                Conditions.Add(conditionValue);
            }
        }

        public bool SatisfiesAllConditions(object value)
        {
            if (Type == "date-range" && value is DateTime dateTime || DateTime.TryParse(value.ToString(), out dateTime))
            {
                return Conditions.All(condition => condition.IsEmpty() || SatisfiesCondition(condition, dateTime));
            }

            return false;
        }

        public bool SatistiesAnyCondition(object value)
        {
            if (Type == "date-range" && value is DateTime dateTime || DateTime.TryParse(value.ToString(), out dateTime))
            {
                return Conditions.Any(condition => !condition.IsEmpty() && SatisfiesCondition(condition, dateTime));
            }

            return false;
        }

        static bool SatisfiesCondition(SearchConditionValue condition, DateTime dateTime)
        {
            if (condition.Value is not DateTime dateTimeCondition) return false;

            if (condition.Type == SearchConditionType.GreaterThanOrEqual)
            {
                if (dateTime < dateTimeCondition) return false;
            }
            else if (condition.Type == SearchConditionType.LessThan)
            {
                if (dateTime >= dateTimeCondition) return false;
            }

            return true;
        }
    }

    [DataContract]
    public class SearchConditionValue
    {
        [DataMember(Name = "value")]
        string ValueString => Value?.ToString() ?? string.Empty;

        [DataMember(Name = "conditionType")]
        public string Type { get; private set; }

        public object Value { get; set; }

        public SearchConditionValue(SearchConditionType conditionType, object value)
        {
            Type = conditionType.ToString();
            Value = value;
        }

        internal bool IsEmpty()
        {
            return string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(ValueString);
        }
    }
}

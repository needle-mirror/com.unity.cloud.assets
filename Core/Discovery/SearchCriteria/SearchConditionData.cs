using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Unity.Cloud.Assets
{
    [DataContract]
    class SearchConditionData
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
    }

    [DataContract]
    class SearchConditionValue
    {
        [DataMember(Name = "value")]
        string ValueString => ValueToString();

        [DataMember(Name = "conditionType")]
        public string Type { get; private set; }

        public object Value { get; set; }

        public SearchConditionValue(SearchConditionRange conditionRange, object value)
        {
            Type = conditionRange.ToString();
            Value = value;
        }

        internal bool IsEmpty()
        {
            return string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(ValueString);
        }

        string ValueToString()
        {
            if (Value is DateTime dateTime)
            {
                return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }

            return Value?.ToString() ?? string.Empty;
        }
    }
}

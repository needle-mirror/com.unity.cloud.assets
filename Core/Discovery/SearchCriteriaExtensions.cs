using System;

namespace Unity.Cloud.Assets
{
    static class SearchCriteriaExtensions
    {
        internal static object GetPropertyValue(this object input, string propertyName)
        {
            return input.GetType().GetProperty(propertyName)?.GetValue(input);
        }
    }
}

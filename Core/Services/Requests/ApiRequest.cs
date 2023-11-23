using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Base class for constructing requests.
    /// </summary>
    abstract class ApiRequest
    {
        protected string m_PathAndQueryParams;
        readonly List<string> m_QueryParams = new();

        /// <summary>
        /// Method for constructing URL from request base path and query params.
        /// </summary>
        /// <param name="requestBasePath">The start path of a request. </param>
        /// <returns>The full request url. </returns>
        public virtual string ConstructUrl(string requestBasePath)
        {
            var url = requestBasePath + m_PathAndQueryParams;
            if (m_QueryParams.Count > 0)
            {
                url += $"?{string.Join("&", m_QueryParams)}";
            }
            return url;
        }

        /// <summary>
        /// Method for constructing the request headers.
        /// </summary>
        /// <returns>A dictionary containing the request headers.</returns>
        public virtual IEnumerable<(string, string)> GetHeaders()
        {
            yield break;
        }

        /// <summary>
        /// Method for constructing the request body.
        /// </summary>
        /// <returns>The <see cref="HttpContent"/> representing the request body.</returns>
        public virtual HttpContent ConstructBody()
        {
            return new StringContent("", Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Helper function to add a provided key and value to the provided
        /// query params and to escape the values correctly if it is a URL.
        /// </summary>
        /// <param name="key">The key to be added.</param>
        /// <param name="value">The value to be added.</param>
        protected void AddParamToQueryParams(string key, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            m_QueryParams.Add($"{key}={Uri.EscapeDataString(value)}");
        }

        /// <summary>
        /// Constructs a string representing an array path parameter and adds it to the query params.
        /// </summary>
        /// <param name="key">The key to be added.</param>
        /// <param name="pathParam">The list of values to convert to string.</param>
        protected void AddParamToQueryParams(string key, IEnumerable<string> pathParam)
        {
            var paramString = "";
            foreach (var value in pathParam)
            {
                paramString += $"{Uri.EscapeDataString(value)},";
            }

            paramString = paramString.Remove(paramString.Length - 1);

            if (string.IsNullOrEmpty(paramString)) return;

            m_QueryParams.Add($"{key}={paramString}");
        }
    }
}

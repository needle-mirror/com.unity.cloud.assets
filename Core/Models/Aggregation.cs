using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class Aggregation
    {
        readonly Dictionary<string, int> m_Values;

        public int Total { get; }
        public int Unique { get; }
        public IReadOnlyDictionary<string, int> Values => m_Values;

        internal Aggregation(Dictionary<string, int> counters)
        {
            m_Values = counters ?? new Dictionary<string, int>();

            Unique = m_Values.Count;

            Total = 0;
            foreach (var counter in m_Values)
            {
                Total += counter.Value;
            }
        }
    }
}

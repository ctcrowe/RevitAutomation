using System.Collections.Generic;
using System.Linq;

namespace CC_Library.Predictions
{
    internal static class DataFindClosest
    {
        public static string FindClosest
            (this Dictionary<string, Data> set,
            KeyValuePair<string, double[]> point)
        {
            List<KeyValuePair<string, double>> results = new List<KeyValuePair<string, double>>();
            if (set.Any())
            {
                foreach (KeyValuePair<string, Data> dp in set)
                {
                    results.Add(new KeyValuePair<string, double>(dp.Key, dp.CalcDistance(point.Value)));
                }
                results.Sort((x, y) => y.Value.CompareTo(x.Value));
                results.Reverse();
                return results.FirstOrDefault().Key;
            }
            return null;
        }
    }
}

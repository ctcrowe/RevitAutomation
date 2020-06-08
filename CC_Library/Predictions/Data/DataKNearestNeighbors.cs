using System.Collections.Generic;
using System.Linq;

namespace CC_Library.Predictions
{
    internal static class DataKNearestNeighbors
    {
        public static Dictionary<string, Data> FindNClosest
            (this Dictionary<string, Data> set, 
            KeyValuePair<string, double[]> point,
            int NumberToFind)
        {
            Dictionary<string, Data> result = new Dictionary<string, Data>();
            KeyValuePair<string, Data>[] solutions = new KeyValuePair<string, Data>[NumberToFind];
            List<KeyValuePair<string, double>> results = new List<KeyValuePair<string, double>>();
            if (set.Any())
            {
                foreach (KeyValuePair<string, Data> dp in set)
                {
                    results.Add(new KeyValuePair<string, double>(dp.Key, dp.CalcDistance(point.Value)));
                }
                results.Sort((x, y) => y.Value.CompareTo(x.Value));
                results.Reverse();
                for (int i = 0; i < solutions.Count(); i++)
                {
                    string key = results[i].Key;
                    Data data = set[key].Clone();
                    solutions[i] = new KeyValuePair<string, Data>(key, data);
                }
            }
            foreach (var sol in solutions)
            {
                result.Add(sol.Key, sol.Value);
            }
            return result;
        }
    }
}
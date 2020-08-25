using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class PredictionFindClosest
    {
        public static string FindClosest
            (this Dictionary<string, Element> Dataset,
            double[] location)
        {
            Dictionary<string, double> results = new Dictionary<string, double>();
            foreach(var e in Dataset)
            {
                if (!results.ContainsKey(e.Key))
                    results.Add(e.Key, e.Value.Distance(location));
            }
            var res = results.ToList();
            var fin = res.OrderBy(x => x.Value);
            return fin.FirstOrDefault().Key;
        }
        public static int FindClosest
            (this List<double[]> Dataset,
            Element location)
        {
            Dictionary<int, double> results = new Dictionary<int, double>();
            for(int i = 0; i < Dataset.Count(); i++)
            {
                results.Add(i, location.Distance(Dataset[i]));
            }
            var res = results.ToList();
            var fin = res.OrderBy(x => x.Value);
            return fin.FirstOrDefault().Key;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class XmlFindNearest
    {
        public static string FindClosest
            (this List<XDocument> docs,
            double[] point)
        {
            List<KeyValuePair<string, double>> results = new List<KeyValuePair<string, double>>();
            if (docs.Any())
            {
                foreach (XDocument ele in docs)
                    results.Add(new KeyValuePair<string, double>(ele.Root.Name.ToString(), ele.CalcDistance(point)));
                results.Sort((x, y) => y.Value.CompareTo(x.Value));
                results.Reverse();
                return results.FirstOrDefault().Key;
            }
            return null;
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace CC_Library.Predictions
{
    internal static class CombineElements
    {
        public static double[] Combine(this IEnumerable<Element> eles)
        {
            double[] Location = new double[eles.FirstOrDefault().Location.Count()];
            for (int j = 0; j < Location.Count(); j++)
            {
                for (int i = 0; i < eles.Count(); i++)
                {
                    Location[j] += eles.ElementAt(i).Location[j];
                }
            }
            return Location;
        }
        public static double[] Combine(this Dictionary<string, Element> eles)
        {
            double[] Location = new double[eles.FirstOrDefault().Value.Location.Count()];
            for (int j = 0; j < Location.Count(); j++)
            {
                for (int i = 0; i < eles.Count(); i++)
                {
                    Location[j] += eles.ElementAt(i).Value.Location[j];
                }
            }
            return Location;
        }
    }
}

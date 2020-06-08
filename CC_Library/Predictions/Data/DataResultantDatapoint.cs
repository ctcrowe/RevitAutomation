using System.Collections.Generic;
using System.Linq;

namespace CC_Library.Predictions
{
    internal static class DataResultantDatapoint
    {
        public static KeyValuePair<string, double[]> ResultantDatapoint(this IEnumerable<KeyValuePair<string, Data>> datum)
        {
            double[] values = new double[datum.FirstOrDefault().Value.Location.Count()];
            int valuecount = 0;
            foreach (KeyValuePair<string, Data> d in datum)
            {
                if (d.Value.Location.Count() == values.Count())
                {
                    valuecount++;
                    for (int i = 0; i < d.Value.Location.Count(); i++)
                    {
                        values[i] += d.Value.Location[i];
                    }
                }
            }
            for (int i = 0; i < values.Count(); i++)
                values[i] /= valuecount;
            return new KeyValuePair<string, double[]>("Resultant", values);
        }
    }
}

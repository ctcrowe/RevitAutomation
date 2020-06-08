using System;
using System.Collections.Generic;
using System.Linq;

namespace CC_Library.Predictions
{
    internal static class DataCorrelation
    {
        public static double[] Correlation(this KeyValuePair<string, Data> Datum,
            Dictionary<string, Data> Dataset,
            WriteToCMDLine write, bool CorrelationIsPositive)
        {
            double[] results = new double[Datum.Value.Location.Count()];
            double[] Location = new double[Datum.Value.Location.Count()];

            foreach (var Reduced in Dataset)
            {
                double[] norm = Reduced.Value.Location.Normalize();
                double distance = Datum.CalcDistance(Reduced);
                for (int i = 0; i < Location.Count(); i++)
                {
                    if (CorrelationIsPositive)
                        Location[i] += norm[i] * distance;
                    else
                    {
                        double pi = Math.PI / 2;
                        double adjustment = Math.Cos(distance / 2 * pi);
                        Location[i] += norm[i] * adjustment;
                    }
                }
            }
            Location.Divide(Dataset.Count());
            return Location;
        }
    }
}

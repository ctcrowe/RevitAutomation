using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class Distance
    {
        public static double FindMaximum(this double[] values)
        {
            double max = 0;
            for (int i = 0; i < values.Count(); i++)
            {
                if (Math.Abs(values[i]) > max)
                    max = Math.Abs(values[i]);
            }
            return max;
        }

        public static double CalcDistance(this KeyValuePair<string, double[]> point1, KeyValuePair<string, double[]> point2)
        {
            double val = 0;
            for (int i = 0; i < point1.Value.Count(); i++)
            {
                double a = point2.Value[i] - point1.Value[i];
                double b = Math.Pow(a, 2);
                val += b;
            }
            double fin = Math.Sqrt(val);
            return fin;
        }
        public static double CalcDistance(this double[] Values, WriteToCMDLine write)
        {
            double val = 0;
            for(int i = 0; i < Values.Count(); i++)
            {
                double a = Math.Pow(Values[i], 2);
                val += a;
            }
            double fin = Math.Sqrt(val);
            return fin;
        }
        public static double CalcDistance(this double[] Values)
        {
            double val = 0;
            for (int i = 0; i < Values.Count(); i++)
            {
                double a = Math.Pow(Values[i], 2);
                val += a;
            }
            double fin = Math.Sqrt(val);
            return fin;
        }
    }
}

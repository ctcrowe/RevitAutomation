using System;
using System.Collections.Generic;
using System.Linq;

namespace CC_Library.Predictions
{
    internal static class DataDistance
    {
        public static double CalcDistance(this KeyValuePair<string, Data> point1, KeyValuePair<string, Data> point2)
        {
            double val = 0;
            for (int i = 0; i < point1.Value.Location.Count(); i++)
            {
                double a = point2.Value.Location[i] - point1.Value.Location[i];
                double b = Math.Pow(a, 2);
                val += b;
            }
            double fin = Math.Sqrt(val);
            return fin;
        }
        public static double CalcDistance(this KeyValuePair<string, Data> point1, double[] point2)
        {
            double val = 0;
            for (int i = 0; i < point1.Value.Location.Count(); i++)
            {
                double a = point2[i] - point1.Value.Location[i];
                double b = Math.Pow(a, 2);
                val += b;
            }
            double fin = Math.Sqrt(val);
            return fin;
        }
        public static double CalcDistance(this Data point1, Data point2)
        {
            double val = 0;
            for (int i = 0; i < point1.Location.Count(); i++)
            {
                double a = point2.Location[i] - point1.Location[i];
                double b = Math.Pow(a, 2);
                val += b;
            }
            double fin = Math.Sqrt(val);
            return fin;
        }
        public static double CalcDistance(this Data point1, double[] point2)
        {
            double val = 0;
            for (int i = 0; i < point1.Location.Count(); i++)
            {
                double a = point2[i] - point1.Location[i];
                double b = Math.Pow(a, 2);
                val += b;
            }
            double fin = Math.Sqrt(val);
            return fin;
        }
        public static double VectorLength(this double[] point)
        {
            double val = 0;
            for (int i = 0; i < point.Count(); i++)
            {
                double b = Math.Pow(point[i], 2);
                val += b;
            }
            double fin = Math.Sqrt(val);
            return fin;
        }
    }
}

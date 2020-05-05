using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class Distance
    {
        public static KeyValuePair<string, double[]> ResultantDatapoint(this IEnumerable<KeyValuePair<string, double[]>> datum)
        {
            double[] values = new double[Dataset.DataSize];
            int valuecount = 0;
            foreach(KeyValuePair<string, double[]> d in datum)
            {
                if (d.Value.Count() == values.Count())
                {
                    valuecount++;
                    for (int i = 0; i < d.Value.Count(); i++)
                    {
                        values[i] += d.Value[i];
                    }
                }
            }
            for (int i = 0; i < values.Count(); i++)
                values[i] /= valuecount;
            return new KeyValuePair<string, double[]>("Resultant", values);
        }
        public static double[] DirectionBetween(this KeyValuePair<string, double[]> point1, KeyValuePair<string, double[]> point2)
        {
            double[] SpaceBetween = new double[Dataset.DataSize];

            for(int i = 0; i < Dataset.DataSize; i++)
            {
                SpaceBetween[i] = point2.Value[i] - point1.Value[i];
            }
            return SpaceBetween;
        }
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
        public static double[] NormalizeVector(this double[] values)
        {
            double[] newvalues = new double[values.Count()];
            double length = values.CalcDistance();
            for(int i = 0; i < values.Count(); i++)
            {
                newvalues[i] = values[i] / length;
            }
            return newvalues;
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
        public static KeyValuePair<string, double[]> FindClosest(this Dataset set, KeyValuePair<string, double[]> point)
        {
            if (set.Data.Any())
            {
                Dictionary<string, double> results = new Dictionary<string, double>();
                foreach (KeyValuePair<string, double[]> dp in set.Data)
                {
                    results.Add(dp.Key, dp.CalcDistance(point));
                }
                KeyValuePair<string, double> min = results.First();
                foreach (KeyValuePair<string, double> result in results)
                {
                    if (result.Value < min.Value) min = result;
                }

                KeyValuePair<string, double[]> datapoint = set.Data.GetEntry(min.Key);
                return datapoint;
            }
            return Datatype.TextData.GeneratePoint();
        }
        public static Dictionary<string, double[]> FindNClosest(this Dataset set, KeyValuePair<string, double[]> point, int NumberToFind)
        {
            Dictionary<string, double[]> solutions = new Dictionary<string, double[]>();
            if (set.Data.Any())
            {
                Dictionary<string, double> results = new Dictionary<string, double>();
                foreach (KeyValuePair<string, double[]> dp in set.Data)
                {
                    results.Add(dp.Key, dp.CalcDistance(point));
                }
            }
            return solutions;
        }
    }
}

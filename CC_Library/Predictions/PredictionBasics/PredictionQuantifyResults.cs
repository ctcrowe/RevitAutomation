using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class PredictionQuantifyResults
    {
        private static readonly double TotalMultiplier = 2.0;
        private static readonly double ModMultiplier = 1.0;
        private static readonly double DistMultiplier = 4.0;
        public static Dictionary<int, double> Quantify(this KeyValuePair<int, double[]>[] Accuracy)
        {
            Dictionary<int, double> Results = new Dictionary<int, double>();
            for(int i = 0; i < Accuracy.Count(); i++)
            {
                Results.Add(i, 0);
            }

            var TotalClosest = Accuracy.OrderByDescending(x => x.Value[2]);
            var ModClosest = Accuracy.OrderByDescending(x => x.Value[0]);
            var MaxDistance = Accuracy.OrderByDescending(x => x.Value[4]);

            for(int i = 0; i < Accuracy.Count(); i++)
            {
                Results[TotalClosest.ElementAt(i).Key] += i * TotalMultiplier;
                Results[ModClosest.ElementAt(i).Key] += i * ModMultiplier;
                Results[MaxDistance.ElementAt(i).Key] += i * DistMultiplier;
            }

            return Results;
        }
    }
}

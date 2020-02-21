using System;
using System.Collections.Generic;
using System.Linq;

namespace CC_Library
{
    internal static class PredictionData
    {
        public static void AdjustWeight(this List<PredictionElement> PredList, string Word, int digit)
        {
            if (PredList.Any(x => x.Word == ele.Word))
            {
                double StartingWeight = PredList.Where(z => z.Word == Word).First().Weight;
                double x = Math.Pow(10, digit * -1);
                double[] y = new double[10];
                for (int i = -5; i < 5; i++)
                {
                    double v = x * i;
                    PredList.Where(z => z.Word == Word).First().Weight = StartingWeight + v;
                    ele.Weight = StartingWeight + v;
                    y[i + 5] = PredList.CalcAccuracy();
                }
                double change = x * Array.IndexOf(y, y.Max());
                PredList.Where(z => z.Word == Word).First().Weight = StartingWeight + change;
            }
        }
        public static double CalcAccuracy(this List<PredictionElement> Predictions, string Word)
        {
            var Phrases = PredictionPhrase.GetData();
            int total = 0;
            int correct = 0;
            foreach(var P in Phrases.Where(x => x.Elements.Any(y => y == Word))
            {
                var PhraseSet = Predictions.Where(a => P.Elements.Any(b => b == a.Word)).ToList();
                if (PhraseSet.Predict() == P.Prediction)
                    correct++;
                total++;
            }
            return correct / total;
        }
        internal static string Predict(this List<PredictionElement> PEs)
        {
            List<Prediction> data = new List<Prediction>();
            foreach (var p in PEs)
            {
                foreach (var d in p.Options)
                {
                    if (!data.Any(x => x.Name == d.Name))
                    {
                        data.Add(new Prediction(d, p));
                    }
                    else
                    {
                        data.Where(x => x.Name == d.Name).First().Combine(p);
                    }
                }
            }
            if (!data.Any())
                return "No Prediction Found";
            return data[data.IndexOf(data.Where(x => x.Value == data.Max(y => y.Value)).First())].Name;
        }
    }
}

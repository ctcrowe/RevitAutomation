using System;
using System.Collections.Generic;
using System.Linq;

/*
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string Dataset = directory + "\\CuratedData";
        
        string attb = "MFSection"
        string ID = "Name"
        
        GetData(string Dataset, string ID, string Attb)
        CalcAccuracy(this List<PredictionElement> Predictions, string Word, string Dataset, string ID, string Attb)
*/

namespace CC_Library
{
    internal static class PredictionData
    {
        public static void AdjustWeight(this List<PredictionElement> PredList, string Word, int digit)
        {
            if (PredList.Any(x => x.Word == Word))
            {
                double StartingWeight = PredList.Where(z => z.Word == Word).First().Weight;
                double x = Math.Pow(10, digit * -1);
                double[] y = new double[10];
                for (int i = -5; i < 5; i++)
                {
                    double v = x * i;
                    PredList.Where(z => z.Word == Word).First().Weight = StartingWeight + v;
                    y[i + 5] = PredList.CalcAccuracy(Word);
                }
                
                int changept = y.CalcChange();
                double change = x * changept;
                PredList.Where(z => z.Word == Word).First().Weight = StartingWeight + change;
                PredList.Where(z => z.Word == Word).First().Accuracy = y[Array.IndexOf(y, y.Max())];
            }
        }
        public static double CalcAccuracy(this List<PredictionElement> Predictions, string Word, string Dataset, string ID, string Attb)
        {
            var Phrases = PredictionPhrase.GetData(Dataset, ID, Attb);
            double total = 0;
            double correct = 0;
            foreach(var P in Phrases.Where(a => a.Elements.Any(b => b == Word)))
            {
                foreach(var w in P.Elements)
                    if (!Predictions.Any(x => x.Word == w))
                        Predictions.Add(new PredictionElement(w));
                var PhraseSet = Predictions.Where(x => P.Elements.Any(y => y == x.Word)).ToList();
                if (PhraseSet.Predict() == P.Prediction)
                    correct++;
                total++;
            }
            return correct / total;
        }
        public static int CalcChange(this double[] d)
        {
            int maxpoint = Array.IndexOf(d, d.Max());
            int changept = 0;
            int maxcount = 0;
            for (int i = 0; i < d.Count(); i++)
            {
                if (d[i] == d.Max())
                {
                    changept += i;
                    maxcount++;
                }
            }
            double change = changept / maxcount;
            return (int)Math.Ceiling(change);
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

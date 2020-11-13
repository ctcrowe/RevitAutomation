using System.Collections.Generic;
using System.Linq;

using CC_Library.Datatypes;
namespace CC_Library.Predictions
{
    internal static class PrivacyAccuracy
    {
        public static double[] RP_Accuracy
            (this List<Entry> Entries,
            Dictionary<string, Element>[] Datasets,
            NeuralNetwork Network)
        {
            double[] Result = new double[4];
            Result[1] = Entries.Count();
            foreach (var e in Datasets[0])
            {
                foreach (var r in Datasets[0])
                {
                    Result[2] += e.Value.Distance(r.Value);
                }
            }
            foreach (var e in Entries)
            {
                e.correct = false;
                var WordList = e.Keys[0].GetWords();
                Dictionary<string, Element> DictPoints = new Dictionary<string, Element>();
                foreach (var word in WordList)
                {
                    if (!DictPoints.ContainsKey(word))
                        DictPoints.Add(word, Datasets[1][word]);
                }
                if (DictPoints.Any())
                {
                    var WordPoint = DictPoints.Combine();
                    var ResultantPoint = Datasets[0].FindClosest(WordPoint);

                    foreach (string val in e.Values)
                    {
                        Result[3] += Datasets[0][val].Distance(WordPoint);
                        if (ResultantPoint == val)
                        {
                            e.correct = true;
                            Result[0]++;
                        }
                    }
                }
            }
            return Result;
        }
    }
}
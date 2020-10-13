using System.Collections.Generic;
using System.Linq;

using CC_Library.Datatypes;
namespace CC_Library.Predictions
{
    internal static class Stud_Accuracy
    {
        public static double[] Accuracy
            (this List<Entry> Entries,
            Dictionary<string, Element>[] Datasets,
            NeuralNetwork Network)
        {
            double[] Result = new double[4];
            Result[1] = Entries.Count();

            foreach (var e in Entries)
            {
                e.correct = false;
                List<double[]> Positions = new List<double[]>();
                foreach(string Phrase in e.Keys)
                {
                    var WordList = Phrase.SplitTitle();
                    Dictionary<string, Element> DictPoints = new Dictionary<string, Element>();
                    foreach (var word in WordList)
                    {
                        if (!DictPoints.ContainsKey(word))
                            DictPoints.Add(word, Datasets[1][word]);
                    }
                    Positions.Add(DictPoints.Combine());
                }
                int closest = Positions.FindClosest(Datasets[0].First().Value);
                string choice = e.Keys.ElementAt(closest);
                string correct = e.Values[0];
                if(choice == correct)
                {
                    e.correct = true;
                    Result[0]++;

                }
                for(int i = 0; i < Positions.Count(); i++)
                {
                    if (e.Keys[i] == correct)
                        Result[3] += Datasets[0].First().Value.Distance(Positions[i]);
                    else
                        Result[2] += Datasets[0].First().Value.Distance(Positions[i]);
                }
            }

            return Result;
        }
    }
}
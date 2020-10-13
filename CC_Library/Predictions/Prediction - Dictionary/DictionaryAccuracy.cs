using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class DictionaryAccuracy
    {
        public static double[] Dict_Accuracy
            (this List<Entry> Entries,
            NeuralNetwork Network,
            List<string> Words)
        {
            double[] Result = new double[4];
            Result[1] = Entries.Count();
            Parallel.For(0, Entries.Count(), i =>
            {
                Entries[i].correct = false;
                var Word = Entries[i].Keys[0];

                var result = Entries[i].DictInput(Words);
                var Zees = new double[1];
                for (int j = 0; j < Network.Layers.Count(); j++)
                {
                    Zees = Network.Layers[j].ZScore(result);
                    result = Network.Layers[j].Output(Zees);
                }

                int resultnumb = result.ToList().IndexOf(result.Max());
                int correct = Words.IndexOf(Entries[i].Values[0]);
                double incorrect = 0;
                int tot = 0;

                for (int j = 0; j < result.Count(); j++)
                {
                    if (result[j] != double.NaN && result[j] != double.PositiveInfinity && result[j] != double.NegativeInfinity)
                    {
                        if (j != correct)
                        {
                            tot++;
                            incorrect += result[j];
                        }
                        else
                            Result[3] += result[j];
                    }
                }
                Result[2] += incorrect / tot;

                if (correct == resultnumb)
                {
                    Entries[i].correct = true;
                    Result[0]++;
                }
            });
            return Result;
        }
        public static void DictAcc_Output
        (this List<Entry> Entries,
                NeuralNetwork Network,
                List<string> Words,
                bool tf)
        {
        }
    }
}
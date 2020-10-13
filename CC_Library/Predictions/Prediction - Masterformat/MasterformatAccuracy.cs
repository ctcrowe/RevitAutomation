using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class MasterformatAccuracy
    {
        public static double[] MF_Accuracy
            (this List<Entry> Entries,
            NeuralNetwork Network,
            List<string> Words)
        {
            double[] Result = new double[4];
            Result[1] = Entries.Count();

            Parallel.For(0, Entries.Count(), i =>
            {
                Entries[i].correct = false;
                var WordList = Entries[i].Keys[0].SplitTitle();
                if (WordList.Any())
                {
                    var result = WordList.GetInput();
                    var Zees = new double[1];
                    for (int j = 0; j < Network.Layers.Count(); j++)
                    {
                        Zees = Network.Layers[j].ZScore(result);
                        result = Network.Layers[j].Output(Zees);
                    }

                    int resultnumb = result.ToList().IndexOf(result.Max());
                    int correct = int.Parse(Entries[i].Values[0]);
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
                }
            });

            return Result;
        }
        public static void MFAcc_Output
            (this List<Entry> Entries,
            NeuralNetwork Network,
            List<string> Words,
            bool tf)
        {
            List<string> Output = new List<string>();
            string FN = "Masterformat_AccuracyOutput.txt".GetMyDocs();

            for(int i = 0; i < Entries.Count(); i++)
            {
                string output = i.ToString() + " : ";
                Entries[i].correct = false;
                var WordList = Entries[i].Keys[0].SplitTitle();
                if (WordList.Any())
                {
                    var result = WordList.GetInput();
                    string input = "\tInput : " + result[0];
                    for (int j = 1; j < result.Count(); j++)
                    {
                        input += ", " + result[j];
                    }
                    var Zees = new double[1];
                    for (int j = 0; j < Network.Layers.Count(); j++)
                    {
                        Zees = Network.Layers[j].ZScore(result);
                        result = Network.Layers[j].Output(Zees);
                    }
                    string s = result[0].ToString();
                    for(int j = 1; j < result.Count(); j++)
                    {
                        s += ", " + result[j];
                    }

                    int resultnumb = result.ToList().IndexOf(result.Max());
                    int correct = int.Parse(Entries[i].Values[0]);

                    if (correct == resultnumb)
                    {
                        Entries[i].correct = true;
                        output += "Y : ";
                    }
                    else
                    {
                        output += "N : ";
                    }
                    output += "Predicted " + resultnumb + " : Correct " + correct + " : " + Entries[i].Keys[0] + " : ";
                    output += s;
                    Output.Add(input);
                    Output.Add(output);
                }
            }
            File.WriteAllLines(FN, Output);
            if (tf) { System.Diagnostics.Process.Start(@FN); }
        }
    }
}
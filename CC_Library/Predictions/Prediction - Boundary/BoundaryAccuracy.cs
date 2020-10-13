using CC_Library.Datatypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class BoundaryAccuracy
    {
        public static double[] BAccuracy
            (this List<Entry> Entries,
            NeuralNetwork Network,
            List<string> Words)
        {
            double[] Result = new double[4];
            Result[1] = Entries.Count();

            Parallel.For(0, Entries.Count(), i =>
            {
                Entries[i].correct = false;
                var result = Entries[i].BInput(new List<string>());
                var Zees = new double[1];
                for (int j = 0; j < Network.Layers.Count(); j++)
                {
                    Zees = Network.Layers[j].ZScore(result);
                    result = Network.Layers[j].Output(Zees);
                }
                int resultnumb = result.ToList().IndexOf(result.Max());
                int correct;
                if (Entries[i].Values[0] == "Y")
                    correct = 1;
                else
                    correct = 0;

                if (result[0] != double.NaN && result[0] != double.PositiveInfinity && result[0] != double.NegativeInfinity)
                {
                    if (resultnumb == correct)
                    {
                        Entries[i].correct = true;
                        Result[0]++;
                    }
                    if (correct == 1)
                    {
                        Result[3] += result[1];
                        Result[2] += result[0];
                    }
                    else
                    {
                        Result[3] += result[0];
                        Result[2] += result[1];
                    }
                }
            });

            return Result;
        }
        public static void BAcc_Output
            (this List<Entry> Entries,
            NeuralNetwork Network,
            List<string> Words,
            bool tf)
        {
            List<string> Output = new List<string>();
            string FN = "Masterformat_AccuracyOutput.txt".GetMyDocs();

            double[] Result = new double[4];
            Result[1] = Entries.Count();

            for (int i = 0; i < Entries.Count(); i++)
            {
                Entries[i].correct = false;
                var result = Entries[i].BInput(new List<string>());
                var Zees = new double[1];
                for (int j = 0; j < Network.Layers.Count(); j++)
                {
                    Zees = Network.Layers[j].ZScore(result);
                    result = Network.Layers[j].Output(Zees);
                }
                int resultnumb = result.ToList().IndexOf(result.Max());
                int correct;
                if (Entries[i].Values[0] == "Y")
                    correct = 1;
                else
                    correct = 0;

                if (result[0] != double.NaN && result[0] != double.PositiveInfinity && result[0] != double.NegativeInfinity)
                {
                    if (resultnumb == correct)
                    {
                        Entries[i].correct = true;
                        Result[0]++;
                    }
                    if (correct == 1)
                    {
                        Result[3] += result[1];
                        Result[2] += result[0];
                    }
                    else
                    {
                        Result[3] += result[0];
                        Result[2] += result[1];
                    }
                }
                string output = i + " : Correct " + correct + " : Result " + resultnumb + " : Number " + result[0];
                Output.Add(output);
            }
            File.WriteAllLines(FN, Output);
            if (tf) { System.Diagnostics.Process.Start(@FN); }
        }
    }
}
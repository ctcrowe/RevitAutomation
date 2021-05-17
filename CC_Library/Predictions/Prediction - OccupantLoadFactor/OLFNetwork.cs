using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using CC_Library.Datatypes;
using System.Reflection;

/// <summary>
/// This system is specifically for creating and using a vocabulary.
/// For this reason, it doesnt need all of the delegates that a typical network would.
/// </summary>

namespace CC_Library.Predictions
{
    /*
    internal class OccupantLoadFactorNetwork
    {
        private const int RunSize = 16;
        public NeuralNetwork Network { get; }

        public OccupantLoadFactorNetwork(WriteToCMDLine write)
        {
            Network = Datatype.OccupantLoadFactor.LoadNetwork(write);
        }
        
        public static int Predict(string s)
        {
            OccupantLoadFactorNetwork olf = new OccupantLoadFactorNetwork(new WriteToCMDLine(WriteNull));
            double[] Results = Alpha.Predict(Datatype.OccupantLoadFactor, s);
            for(int i = 0; i < olf.Network.Layers.Count(); i++)
            {
                Results = olf.Network.Layers[i].Output(Results);
            }
            int Result = Results.ToList().IndexOf(Results.Max());
            return Result;
        }

        public KeyValuePair<string, bool> Predict(string s, int number, int correct, WriteToCMDLine write)
        {
            string line2 = " : Actual " + s.Split(',').Last() + " : Predicted ";
            double[] Results = Alpha.Predict(Datatype.OccupantLoadFactor, s.Split(',').First());
            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                Results = Network.Layers[i].Output(Results);
            }
            int Result = Results.ToList().IndexOf(Results.Max());
            line2 += Result.ToString();
            line2 += " : Confidence " + Results.Max();
            line2 += " : Actual Confidence " + Results[int.Parse(s.Split(',').Last())];
            line2 += " : " + s.Split(',').First();
            if(Result == int.Parse(s.Split(',').Last()))
            {
                correct++;
            }
            string line = correct + " / " + number + " : " + correct * 1.0 / number;
            return new KeyValuePair<string, bool>(line + line2, Result.ToString() == s.Split(',').Last());
        }

        public static string[] PredictAll(string[] s, WriteToCMDLine write)
        {
            string[] r = new string[s.Count()];
            Alpha a = new Alpha(write);
            OccupantLoadFactorNetwork olf = new OccupantLoadFactorNetwork(write);
            int x = 0;
            for(int i = 0; i < s.Count(); i++)
            {
                var pred = olf.Predict(s[i], i, x, write);
                if (pred.Value)
                    x++;
                r[i] = pred.Key;
                write(r[i]);
            }
            return r;
        }

        internal static void SamplePropogate
            (
            string line,
            int lineno,
            OccupantLoadFactorNetwork olf,
            Alpha a,
            LocalContext ctxt,
            Accuracy acc,
            WriteToCMDLine write
            )
        {
            string input = line.Split(',')[1];
            AlphaMem am = new AlphaMem();

            List<double[]> Results = new List<double[]>();
            Results.Add(a.Forward(input, ctxt, am, write));

            for (int k = 0; k < olf.Network.Layers.Count(); k++)
            {
                Results.Add(olf.Network.Layers[k].Output(Results.Last()));
                if (Results.Last().Any(x => x == double.NaN) ||
                    Results.Last().Any(x => x == double.PositiveInfinity) ||
                    Results.Last().Any(x => x == double.NegativeInfinity))
                {
                    string s = "MF Network Layer " + k + " output";
                    s.show(Results.Last(), write);
                }
            }

            int choice = Results.Last().ToList().IndexOf(Results.Last().Max());
            int correct = int.Parse(line.Split(',').Last());

            double[] res = new double[40];
            res[correct] = 1;
            var result = CategoricalCrossEntropy.Forward(Results.Last(), res);

            acc.Add(input, lineno, Results.Last()[correct], result.Sum(), correct == choice);
            var DValues = res;

            for (int l = olf.Network.Layers.Count() - 1; l >= 0; l--)
            {
                DValues = olf.Network.Layers[l].DActivation(DValues, Results[l + 1]);
                olf.Network.Layers[l].DBiases(DValues);
                olf.Network.Layers[l].DWeights(DValues, Results[l]);
                DValues = olf.Network.Layers[l].DInputs(DValues);
            }
            a.Backward(input, DValues, ctxt, am, write);
        }
        
        internal static void Propogate
            (string filepath,
            WriteToCMDLine write)
        {
            OccupantLoadFactorNetwork olf = new OccupantLoadFactorNetwork(write);
            Alpha a = new Alpha(write);
            LocalContext ctxt = new LocalContext(Datatype.OccupantLoadFactor, write);
            Random random = new Random();
            string[] Lines = File.ReadAllLines(filepath);
            
            olf.Network.Save();
            a.Location.Save();
            ctxt.Save();
            write("Network Saved");

            int[] numbs = new int[Lines.Count()];
            for (int i = 0; i < numbs.Count(); i++)
                numbs[i] = i;

            int count = 1;
            var acc = new Accuracy(Lines.Count());

            while (count < 3000000)
            {
                var numbers = numbs.OrderBy(x => random.Next()).ToList();

                for (int i = 0; i < Lines.Count(); i += RunSize)
                {
                    write("Run Number : " + count);

                    Parallel.For(0, RunSize, j =>
                    {
                        if(i + j < numbers.Count())
                          SamplePropogate(Lines[numbers[i + j]], numbers[i + j], olf, a, ctxt, acc, write);
                    });
                    olf.Network.Update(RunSize, 0.001);
                    a.Location.Update(RunSize, 0.001);
                    ctxt.Network.Update(RunSize, 0.001);
                    acc.Show(write);
                    write("");

                    count++;
                }
                acc.Reset();
                olf.Network.Save();
                a.Location.Save();
                ctxt.Save();
                write("Network Saved");
            }
        }
        private static string WriteNull(string s) { return null; }
        private static int GetResult(string p)
        {
            switch(int.Parse(p))
            {
                default:
                case 0:
                    return 0;
                case 5:
                    return 1;
                case 7:
                    return 2;
                case 15:
                    return 3;
                case 20:
                    return 4;
                case 30:
                    return 5;
                case 35:
                    return 6;
                case 40:
                    return 7;
                case 50:
                    return 8;
            }
        }
    }
    */
}
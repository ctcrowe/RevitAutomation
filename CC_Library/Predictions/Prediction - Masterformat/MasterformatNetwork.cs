using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;

/// <summary>
/// This system is specifically for creating and using a vocabulary.
/// For this reason, it doesnt need all of the delegates that a typical network would.
/// </summary>

namespace CC_Library.Predictions
{

    public class MasterformatNetwork
    {
        private const int MinSamples = 2000;
        private const int RunSize = 16;
        public NeuralNetwork Network { get; }

        public MasterformatNetwork(WriteToCMDLine write)
        {
            Network = Datatype.Masterformat.LoadNetwork(write);
        }
        
        public static int Predict(string s)
        {
            MasterformatNetwork mf = new MasterformatNetwork(new WriteToCMDLine(WriteNull));
            double[] Results = Alpha.Predict(Datatype.Masterformat, s);
            for(int i = 0; i < mf.Network.Layers.Count(); i++)
            {
                Results = mf.Network.Layers[i].Output(Results);
            }
            int Result = Results.ToList().IndexOf(Results.Max());
            return Result;
        }
        public static string[] PredictAll(string[] s, WriteToCMDLine write)
        {
            string[] r = new string[s.Count()];
            Alpha a = new Alpha(write);
            int x = 0;
            for(int i = 0; i < s.Count(); i++)
            {
                var pred = MasterformatNetwork.Predict(s[i].Split(',')[1]);
                if (pred == int.Parse(s[i].Split(',')[2]))
                {
                    r[i] = "cor : ";
                    x++;
                }
                else
                    r[i] = "inc : ";
                r[i] += x + " / " + (i + 1) + " : " + x / (1.0 + i) + " : " + s[i] + " : " + pred;
                write(r[i]);
            }
            return r;
        }
        internal static void SamplePropogate
            (
            string line,
            int lineno,
            Random r,
            MasterformatNetwork mf,
            Alpha a,
            LocalContext lctxt,
            Accuracy acc,
            bool tf,
            WriteToCMDLine write
            )
        {
            string input = line.Split(',')[1];
            AlphaMem am = new AlphaMem(input.ToCharArray());

            List<double[]> Results = new List<double[]>();
            Results.Add(a.Forward(input, lctxt, am, write));

            for (int k = 0; k < mf.Network.Layers.Count(); k++)
            {
                Results.Add(mf.Network.Layers[k].Output(Results.Last()));
            }

            int choice = Results.Last().ToList().IndexOf(Results.Last().Max());
            int correct = int.Parse(line.Split(',').Last());

            double[] res = new double[40];
            res[correct] = 1;
            var result = CategoricalCrossEntropy.Forward(Results.Last(), res);

            acc.Add(input, lineno, Results.Last()[correct], result.Sum(), choice);
            if (tf)
            {
                var DValues = res;

                for (int l = mf.Network.Layers.Count() - 1; l >= 0; l--)
                {
                    DValues = mf.Network.Layers[l].DActivation(DValues, Results[l + 1]);
                    mf.Network.Layers[l].DBiases(DValues);
                    mf.Network.Layers[l].DWeights(DValues, Results[l]);
                    DValues = mf.Network.Layers[l].DInputs(DValues);
                }
                a.Backward(input, DValues, lctxt, am, write);
            }
        }
        
        internal static void Propogate
            (string filepath,
            WriteToCMDLine write)
        {
            MasterformatNetwork mf = new MasterformatNetwork(write);
            Alpha a = new Alpha(write);
            LocalContext lctxt = new LocalContext(Datatype.Masterformat, write);
            Random random = new Random();
            COutput.Clear();
            while (true)
            {
                List<string> LineList = File.ReadAllLines(filepath).ToList();
                int lnno = 0;
                int lnct = LineList.Count();
                while (LineList.Count < MinSamples)
                {
                    string s = "Masterformat,";
                    int Error = Math.Max(1, (int)Math.Round(LineList[lnno].Split(',')[1].Length * 0.1));
                    s += Alpha.GenerateTypo(LineList[lnno].Split(',')[1], random, Error);
                    s += "," + LineList[lnno].Split(',').Last();
                    LineList.Add(s);
                    lnno++;
                    if (lnno >= lnct)
                        lnno = 0;
                }
                var Lines = LineList.ToArray();

                int[] numbs = new int[Lines.Count()];
                for (int i = 0; i < numbs.Count(); i++)
                    numbs[i] = i;

                int samples = 0;
                int cycles = 0;
                int epochs = 0;
                string epoch = "";
                var acc = new Accuracy(Lines);
                while (epochs < 50 && acc.Acc < 0.99999)
                {

                    var numbers = numbs.OrderBy(x => random.Next()).ToList();

                    for (int i = 0; i < Lines.Count(); i += RunSize)
                    {
                        Parallel.For(0, RunSize, j =>
                        {
                            if (i + j < numbers.Count())
                            {
                                SamplePropogate(Lines[numbers[i + j]], numbers[i + j], random, mf, a, lctxt, acc, epochs > 0, write);
                                samples++;
                            }
                        });
                        if (epochs > 0)
                        {
                            mf.Network.Update(RunSize, 0.0001);
                            a.Location.Update(RunSize, 0.0001);
                            lctxt.Network.Update(RunSize, 0.0001);
                        }
                        epoch = "Samples Run : " + samples + " : Cycles : " + cycles + " : Epochs : " + epochs;
                        COutput.Update(epoch, acc);

                        cycles++;
                    }
                    if (acc.CheckError() && epochs > 0)
                    {
                        mf.Network.Save();
                        a.Location.Save();
                        lctxt.Save();
                    }
                    if (epochs == 0)
                        acc.SetStartingError();
                    epochs++;
                    acc.Reset();
                }
            }
        }
        private static string WriteNull(string s) { return null; }
    }
}
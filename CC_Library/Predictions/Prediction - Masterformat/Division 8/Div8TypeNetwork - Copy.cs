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
/*
namespace CC_Library.Predictions
{

    internal class Div8TypeNetwork
    {
        private const int MinSamples = 2000;
        private const int RunSize = 16;
        public NeuralNetwork Network { get; }

        public Div8TypeNetwork(WriteToCMDLine write)
        {
            Network = Datatype.Div8Type.LoadNetwork(write);
        }
        
        public static int Predict(string s)
        {
            Div8TypeNetwork net = new Div8TypeNetwork(new WriteToCMDLine(WriteNull));
            double[] Results = Alpha.Predict(Datatype.Div8Type, s);
            for(int i = 0; i < net.Network.Layers.Count(); i++)
            {
                Results = net.Network.Layers[i].Output(Results);
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
                var pred = Div8TypeNetwork.Predict(s[i].Split(',')[1]);
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
            Div8TypeNetwork net,
            Alpha a,
            LocalContext lctxt,
            GlobalContext gctxt,
            Accuracy acc,
            WriteToCMDLine write
            )
        {
            string input = line.Split(',')[1];
            AlphaMem am = new AlphaMem();

            List<double[]> Results = new List<double[]>();
            Results.Add(a.Forward(input, lctxt, gctxt, am, write));

            for (int k = 0; k < net.Network.Layers.Count(); k++)
            {
                Results.Add(net.Network.Layers[k].Output(Results.Last()));
            }

            int choice = Results.Last().ToList().IndexOf(Results.Last().Max());
            int correct = int.Parse(line.Split(',').Last());

            double[] res = new double[net.Network.Layers.Last().Biases.Count()];
            if(correct < res.Count())
                res[correct] = 1;
            var result = CategoricalCrossEntropy.Forward(Results.Last(), res);

            acc.Add(input, lineno, Results.Last()[correct], result.Sum(), choice);
            var DValues = res;

            for (int l = net.Network.Layers.Count() - 1; l >= 0; l--)
            {
                DValues = net.Network.Layers[l].DActivation(DValues, Results[l + 1]);
                net.Network.Layers[l].DBiases(DValues);
                net.Network.Layers[l].DWeights(DValues, Results[l]);
                DValues = net.Network.Layers[l].DInputs(DValues);
            }
            a.Backward(input, DValues, lctxt, gctxt, am, write);
        }
        
        internal static void Propogate
            (string filepath,
            WriteToCMDLine write)
        {
            Div8TypeNetwork net = new Div8TypeNetwork(write);
            Alpha a = new Alpha(write);
            LocalContext lctxt = new LocalContext(Datatype.Div8Type, write);
            GlobalContext gctxt = new GlobalContext(Datatype.Div8Type, write);
            Random random = new Random();
            COutput.Clear();
            while (true)
            {
                List<string> LineList = File.ReadAllLines(filepath).ToList();
                int lnno = 0;
                int lnct = LineList.Count();
                while (LineList.Count < MinSamples)
                {
                    string s = "Div8Type,";
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
                                SamplePropogate(Lines[numbers[i + j]], numbers[i + j], random, net, a, lctxt, gctxt, acc, write);
                                samples++;
                            }
                        });
                        if (cycles > 1)
                        {
                            net.Network.Update(RunSize, 0.01);
                            a.Location.Update(RunSize, 0.01);
                            lctxt.Network.Update(RunSize, 0.01);
                            gctxt.Network.Update(RunSize, 0.01);
                        }
                        epoch = "Samples Run : " + samples + " : Cycles : " + cycles + " : Epochs : " + epochs;
                        COutput.Update(epoch, acc);

                        cycles++;
                    }
                    epochs++;
                    if (acc.CheckError() && epochs > 1)
                    {
                        net.Network.Save();
                        a.Location.Save();
                        lctxt.Save();
                        gctxt.Save();
                    }
                    acc.Reset();
                }
            }
        }
        private static string WriteNull(string s) { return null; }
    }
}*/
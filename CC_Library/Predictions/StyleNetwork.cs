using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;
using System.Runtime.InteropServices;

/// <summary>
/// This system is specifically for creating and using a vocabulary.
/// For this reason, it doesnt need all of the delegates that a typical network would.
/// </summary>

namespace CC_Library.Predictions
{
    public class ObjectStyleNetwork
    {
        private const int MinSamples = 2000;
        private const int RunSize = 16;
        public NeuralNetwork Network { get; }
        public ObjectStyleNetwork(WriteToCMDLine write)
        {
            Network = Datatype..LoadNetwork(write);
        }
        public static int[] Predict(string s)
        {
            ObjectStyleNetwork net = new ObjectStyleNetwork(new WriteToCMDLine(WriteNull));
            double[] Results = Alpha.Predict(Datatype., s);
            for(int i = 0; i < net.Network.Layers.Count(); i++)
            {
                Results = net.Network.Layers[i].Output(Results);
            }
            int[] Result = Results.ToList().IndexOf(Results.Max());
            return Result;
        }
        public static string[] PredictAll(string[] s, WriteToCMDLine write)
        {
            string[] r = new string[s.Count()];
            Alpha a = new Alpha(write);
            int x = 0;
            for(int i = 0; i < s.Count(); i++)
            {
                var pred = ObjectStyleNetwork.Predict(s[i].Split(',')[1]);
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
            ObjectStyleNetwork net,
            Alpha a,
            AlphaContext lctxt,
            Accuracy acc,
            bool tf,
            NetworkMem AlphaMem,
            NetworkMem CtxtMem,
            NetworkMem ObjMem,
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
                    DValues = MFMem.Layers[l].DActivation(DValues, Results[l + 1]);
                    MFMem.Layers[l].DBiases(DValues);
                    MFMem.Layers[l].DWeights(DValues, Results[l]);
                    DValues = MFMem.Layers[l].DInputs(DValues, mf.Network.Layers[l]);
                }
                a.Backward(input, DValues, lctxt, am, AlphaMem, CtxtMem, write);
            }
        }
        internal static void Propogate
            (string filepath,
            WriteToCMDLine write)
        {
            ObjectStyleNetwork mf = new ObjectStyleNetwork(write);
            Alpha a = new Alpha(write);
            AlphaContext lctxt = new AlphaContext(Datatype.Masterformat, write);
            NetworkMem OBJMem = new NetworkMem(net.Network);
            NetworkMem AlphaMem = new NetworkMem(a.Location);
            NetworkMem CtxtMem = new NetworkMem(lctxt.Network);
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
                                SamplePropogate(Lines[numbers[i + j]], numbers[i + j], random, mf, a, lctxt, acc, epochs > 0, AlphaMem, CtxtMem, MFMem, write);
                                samples++;
                            }
                        });
                        if (epochs > 0)
                        {
                            MFMem.Update(RunSize, 0.0001, mf.Network);
                            AlphaMem.Update(RunSize, 0.0001, a.Location);
                            CtxtMem.Update(RunSize, 0.0001, lctxt.Network);
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
        public static void ActivePropogate
            (List<string> LineList)
        {
            MasterformatNetwork mf = new MasterformatNetwork(WriteNull);
            Alpha a = new Alpha(WriteNull);
            AlphaContext lctxt = new AlphaContext(Datatype.Masterformat, WriteNull);
            NetworkMem MFMem = new NetworkMem(mf.Network);
            NetworkMem AlphaMem = new NetworkMem(a.Location);
            NetworkMem CtxtMem = new NetworkMem(lctxt.Network);

            mf.Network.Save();
            a.Location.Save();
            lctxt.Save();

            Random random = new Random();
            var Lines = LineList.ToArray();
            var acc = new Accuracy(Lines);
            int[] numbs = new int[Lines.Count()];
            for (int i = 0; i < numbs.Count(); i++)
                numbs[i] = i;
            int cycles = 0;

            while (acc.Acc < 0.999 && cycles < 5)
            {
                var numbers = numbs.OrderBy(x => random.Next()).ToList();

                for (int i = 0; i < Lines.Count(); i += RunSize)
                {
                    Parallel.For(0, RunSize, j =>
                    {
                        if (i + j < numbers.Count())
                        {
                            SamplePropogate(Lines[numbers[i + j]], numbers[i + j], random, mf, a, lctxt, acc, true, AlphaMem, CtxtMem, MFMem, WriteNull);
                        }
                    });
                    MFMem.Update(RunSize, 0.0001, mf.Network);
                    AlphaMem.Update(RunSize, 0.0001, a.Location);
                    CtxtMem.Update(RunSize, 0.0001, lctxt.Network);
                }
                acc.SetAcc();
                var output = acc.Get();
                cycles++;
            }
            mf.Network.Save();
            a.Location.Save();
            lctxt.Save();
        }
        private static string WriteNull(string s) { return s; }
        private static string WriteConsole(string s) { Console.WriteLine(s); return s; }
    }
}

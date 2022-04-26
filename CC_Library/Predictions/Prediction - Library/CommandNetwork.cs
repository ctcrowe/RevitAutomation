using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CC_Library.Datatypes;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public class CmdNetwork
    {
        private const double dropout = 0.1;
        public static Datatype datatype { get { return Datatype.Command; } }
        public static NeuralNetwork GetNetwork(WriteToCMDLine write)
        {
            var size = Enum.GetNames(typeof(Command)).Length;
            NeuralNetwork net = datatype.LoadNetwork(write);
            if (net.Datatype == Datatype.None)
            {
                net = new NeuralNetwork(datatype);
                net.Layers.Add(new Layer(Alpha.DictSize, Alpha.DictSize, Activation.LRelu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(Alpha.DictSize, net.Layers.Last().Weights.GetLength(0), Activation.LRelu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(size, net.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));
            }
            return net;
        }
        public static double[] Predict(string s, WriteToCMDLine write)
        {
            NeuralNetwork net = GetNetwork(write);
            Alpha a = new Alpha(write);
            AlphaContext ctxt = new AlphaContext(datatype, write);
            double[] Results = a.Forward(s);
            Results.WriteArray("Alpha Results : ", write);
            for (int i = 0; i < net.Layers.Count(); i++)
            {
                //Results = net.Layers[i].Output(Results);
            }
            return Results;
        }
        public static double Propogate
            (WriteToCMDLine write)
        {
            double error = 0;
            NeuralNetwork net = GetNetwork(write);
            var Samples = ReadVals(24);
            Alpha a = new Alpha(write);
            AlphaContext ctxt = new AlphaContext(datatype, write);
            NetworkMem OLFMem = new NetworkMem(net);
            NetworkMem AlphaMem = new NetworkMem(a.Network);
            NetworkMem CtxtMem = new NetworkMem(ctxt.Network);

            Parallel.For(0, Samples.Count(), j =>
            {
                AlphaMem am = new AlphaMem(Samples.Keys.ToList()[j].Length);
                var output = a.Forward(Samples.Keys.ToList()[j]);
                var F = net.Forward(output, dropout, write);
                var desired = new double[Enum.GetNames(typeof(Command)).Length];
                desired[Samples.Values.ToList()[j]] = 1;
                error += CategoricalCrossEntropy.Forward(F.Last()[0], desired).Max();

                var DValues = net.Backward(F, desired, OLFMem, write);
                //a.Backward(Samples.Keys.ToList()[j], DValues, ctxt, am, AlphaMem, CtxtMem);
            });
            OLFMem.Update(Samples.Count(), 0.0001, net);
            AlphaMem.Update(Samples.Count(), 0.0001, a.Network);
            CtxtMem.Update(Samples.Count(), 0.0001, ctxt.Network);
            write("Pre Training Error : " + error);

            net.Save();
            a.Network.Save();
            ctxt.Network.Save(datatype);

            error = 0;
            Parallel.For(0, Samples.Count(), j =>
            {
                AlphaMem am = new AlphaMem(Samples.Keys.ToList()[j].Length);
                var output = a.Forward(Samples.Keys.ToList()[j]);
                var F = net.Forward(output, dropout, write);
                var desired = new double[Enum.GetNames(typeof(Command)).Length];
                desired[Samples.Values.ToList()[j]] = 1;
                error += CategoricalCrossEntropy.Forward(F.Last()[0], desired).Max();
            });
            write("Post Training Error : " + error);
            return error;
        }
        private static Tuple<string, int> GetIO(string fn)
        {
            // Lines[0] = Datatype Command
            // Lines[1] = DateTime.Now("yyyyMMddhhmmss")
            // Lines[2] = input
            // Lines[3] = output
            if (File.Exists(fn))
            {
                var lines = File.ReadAllLines(fn);
                if (lines[0] == "Datatype Command")
                {
                    var names = Enum.GetNames(typeof(Command)).ToList();
                    var key = lines[2];
                    int val = int.TryParse(lines[3], out int x) ? x :
                        Enum.GetNames(typeof(Command)).Contains(lines[3]) ? Enum.GetNames(typeof(Command)).ToList().IndexOf(lines[3]) : 0;
                    return val == 0 ? null : new Tuple<string, int>(key, val);
                }
            }
            return null;
        }
        private static Dictionary<string, int> ReadVals(int Count = 16)
        {
            var dict = new Dictionary<string, int>();

            string fname = "NetworkSamples";
            string folder = fname.GetMyDocs();
            if (Directory.Exists(folder))
            {
                string subfolder = folder + "\\Command";
                if (Directory.Exists(subfolder))
                {
                    string[] Files = Directory.GetFiles(subfolder);
                    if (Files.Any())
                    {
                        Random r = new Random();
                        for (int i = 1; i < Count; i++)
                        {
                            var kvp = GetIO(Files[r.Next(Files.Count())]);
                            if (kvp != null)
                                if (!dict.ContainsKey(kvp.Item1))
                                    dict.Add(kvp.Item1, kvp.Item2);
                        }
                    }
                }
            }
            return dict;
        }
    }
}

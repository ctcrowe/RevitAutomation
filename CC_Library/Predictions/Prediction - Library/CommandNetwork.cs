using System;
using System.Linq;
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
            double[] Results = a.Forward(s, ctxt);
            Results.WriteArray("Alpha Results : ", write);
            for(int i = 0; i < net.Layers.Count(); i++)
            {
                Results = net.Layers[i].Output(Results);
            }
            return Results;
        }
        public static double Propogate
            (string fn, WriteToCMDLine write, bool tf = false)
        {
            double error = 0;
            var s = GetIO(fn);
            var Pred = Predict(s.Key, CMDLibrary.WriteNull);
            if (s.Value != Pred.ToList().IndexOf(Pred.Max()) || tf)
            {
                NeuralNetwork net = GetNetwork(write);
                var Samples = ReadVals(24);
                Alpha a = new Alpha(write);
                AlphaContext ctxt = new AlphaContext(datatype, write);
                NetworkMem OLFMem = new NetworkMem(net);
                NetworkMem AlphaMem = new NetworkMem(a.Network);
                NetworkMem CtxtMem = new NetworkMem(ctxt.Network);

                Parallel.For(0, Samples.Count(), j =>
                {
                    AlphaMem am = new AlphaMem(Samples[j].TextInput.ToCharArray());
                    var output = a.Forward(Samples[j].TextInput, ctxt, am);
                    var F = net.Forward(output, dropout, write);
                    error += CategoricalCrossEntropy.Forward(F.Last().GetRank(0), Samples[j].DesiredOutput).Max();

                    var DValues = net.Backward(F, Samples[j].DesiredOutput, OLFMem, write);
                    a.Backward(Samples[j].TextInput, DValues, ctxt, am, AlphaMem, CtxtMem);
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
                    AlphaMem am = new AlphaMem(Samples[j].TextInput.ToCharArray());
                    var output = a.Forward(Samples[j].TextInput, ctxt, am);
                    var F = net.Forward(output, dropout, write);
                    error += CategoricalCrossEntropy.Forward(F.Last().GetRank(0), Samples[j].DesiredOutput).Max();
                });
                write("Post Training Error : " + error);

                s.Save();
            }
            return error;
        }
        private static KeyValuePair<string, int> GetIO (string fn)
        {
            // Lines[0] = Datatype Command
            // Lines[1] = DateTime.Now("yyyyMMddhhmmss")
            // Lines[2] = input
            // Lines[3] = output
            if(File.Exists(fn))
            {
                var lines = File.ReadAllLines(fn);
                if(lines[0] == "Datatype Command")
                {
                    var names = Enum.GetNames(typeof(Commands)).ToList();
                    var key = lines[2];
                    var val = int.TryParse(Lines[3], out int x) ? int.Parse(Lines[3]) < names.Count() ? int.Parse(Lines[3]) :
                        Enum.GetNames(typeof(Commands)).Contains(Lines[3]) ? Enum.GetNames(typeof(Commands)).IndexOf(Lines[3]) : null;
                    return val == null ? null : new KeyValuePair<string, int> { key, val };
                }
            }
            return null;
        }
        private static Dictionary<string, int> ReadVals(KeyValuePair<string, int> keypair, int Count = 16)
        {
            var dict = new Dictionary<string, int>();
            dict.Add(keypair.Key, keypair.Value);
            private const string fname = "NetworkSamples";
            private static string folder = fname.GetMyDocs();
            if (Directory.Exists(folder))
            {
                string subfolder = folder + "\\Command";
                if(Directory.Exists(subfolder))
                {
                    string[] Files = Directory.GetFiles(subfolder);
                    if(Files.Any())
                    {
                        Random r = new Random();
                        for(int i = 1; i < Count; i++)
                        {
                            var kvp = GetIO(Files[r.Next(Files.Count())]);
                            if(kvp != null)
                                if(!Dictionary.ContainsKey(kvp.Key))
                                    Dictionary.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
            }
            return dict;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;
using System.Runtime.InteropServices;

namespace CC_Library.Predictions
{
    class CutLineWeightNetwork
    {
        private const double dropout = 0.1;
        public static Datatype datatype { get { return Datatype.CutLineWeight; } }
        public static NeuralNetwork GetNetwork(WriteToCMDLine write)
        {
            Alpha2 a = new Alpha2(CMDLibrary.WriteNull);
            NeuralNetwork net = datatype.LoadNetwork(write);
            if (net.Datatype == Datatype.None)
            {
                net = new NeuralNetwork(datatype);
                net.Layers.Add(new Layer(100, a.GetSize(), Activation.LRelu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(100, net.Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(100, net.Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(100, net.Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(100, net.Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(16, net.Layers.Last(), Activation.CombinedCrossEntropySoftmax));
            }
            return net;
        }
        public static double[] Predict(string s, WriteToCMDLine write)
        {
            NeuralNetwork net = GetNetwork(write);
            Alpha2 a = new Alpha2(write);
            a.Load(write);
            double[] Results = a.Forward(s, write).Key;
            for(int i = 0; i < net.Layers.Count(); i++)
            {
                Results = net.Layers[i].Output(Results);
            }
            return Results;
        }
        public static double[] Propogate
            (string s, WriteToCMDLine write, bool tf = false)
        {
            var results = new double[2];
            NeuralNetwork net = GetNetwork(write);
            var Samples = s.ReadSamples(24);
            Alpha2 a = new Alpha2(write);
            a.Load(write);
            var am = a.CreateMemory();
            NetworkMem mem = new NetworkMem(net);

            try
            {
                Parallel.For(0, Samples.Count(), j =>
                {
                    var sample = Samples.ElementAt(j);
                    var output = a.Forward(sample.Key, write);
                    var F = net.Forward(output.Key, dropout, write, false);

                    var desired = new double[net.Layers.Last().Biases.Count()];
                    desired[sample.Value] = 1;
                    results[0] += CategoricalCrossEntropy.Forward(F.Last().GetRank(0), desired).Max();
                    results[1] += F.Last().GetRank(0).ToList().IndexOf(F.Last().GetRank(0).Max()) == sample.Value ? 1 : 0;

                    var DValues = net.Backward(F, desired, mem, write);
                    a.Backward(DValues, output.Value, am, write, j == 0);
                });
            }
            catch (Exception e) { e.OutputError(); }
            mem.Update(Samples.Count(), 1e-4, net);
            a.Update(am, Samples.Count());
            results[0] /= Samples.Count();
            results[1] /= Samples.Count();

            write("Run Error : " + results[0]);
            write("Run Accuracy : " + results[1]);
            net.Save();
            a.Save();
            return results;
        }
    }
}

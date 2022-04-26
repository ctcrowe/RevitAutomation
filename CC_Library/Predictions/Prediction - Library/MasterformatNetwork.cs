using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class MasterformatNetwork
    {
        private const double dropout = 0.1;
        public static Datatype datatype { get { return Datatype.Masterformat; } }
        public static NeuralNetwork GetNetwork(WriteToCMDLine write)
        {
            Alpha2 a = new Alpha2(CMDLibrary.WriteNull);
            NeuralNetwork net = datatype.LoadNetwork(write);
            if (net.Datatype == Datatype.None)
            {
                net = new NeuralNetwork(datatype);
                net.Layers.Add(new Layer(100, a.GetSize(), Activation.ReLu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(100, net.Layers.Last(), Activation.ReLu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(100, net.Layers.Last(), Activation.ReLu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(100, net.Layers.Last(), Activation.ReLu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(100, net.Layers.Last(), Activation.ReLu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(100, net.Layers.Last(), Activation.ReLu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(40, net.Layers.Last(), Activation.CombinedCrossEntropySoftmax));
            }
            return net;
        }
        public static double[] Predict(string s, WriteToCMDLine write)
        {
            NeuralNetwork net = GetNetwork(write);
            Alpha2 a = new Alpha2(write);
            a.Load(write);
            double[] Results = a.Forward(s, write).Key;
            //Results.WriteArray("Alpha Results : ", write);
            for(int i = 0; i < net.Layers.Count(); i++)
            {
                Results = net.Layers[i].Output(Results);
            }
            return Results;
        }
        public static double[] Propogate
            (Sample s, WriteToCMDLine write, bool tf = false)
        {
            var results = new double[2];
            NeuralNetwork net = GetNetwork(write);
            var Samples = s.ReadSamples(24);
            Alpha2 a = new Alpha2(write);
            a.Load(write);
            var am = a.CreateMemory();
            NetworkMem MFMem = new NetworkMem(net);

            try
            {
                Parallel.For(0, Samples.Count(), j =>
                {
                    var output = a.Forward(Samples[j].TextInput, write);
                    var F = net.Forward(output.Key, dropout, write, false);
                    /*
                    if (j == 0)
                    {
                        output.Key.WriteArray("Alpha Out : ", write);
                        F.Last().GetRank(0).WriteArray("Output[0]", write);
                        Samples[j].DesiredOutput.WriteArray("Desired", write);
                    }*/
                    results[0] += CategoricalCrossEntropy.Forward(F.Last()[0], Samples[j].DesiredOutput).Max();
                    results[1] += F.Last()[0].ToList().IndexOf(F.Last()[0].Max()) ==
                        Samples[j].DesiredOutput.ToList().IndexOf(Samples[j].DesiredOutput.Max()) ? 1 : 0;

                    var DValues = net.Backward(F, Samples[j].DesiredOutput, MFMem, write);
                    a.Backward(DValues, output.Value, am, write, j == 0);
                });
            }
            catch (Exception e) { e.OutputError(); }
            MFMem.Update(Samples.Count(), 1e-4, net);
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

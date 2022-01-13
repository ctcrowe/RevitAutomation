using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public class OLFNetwork
    {
        private const double dropout = 0.1;
        public static Datatype datatype { get { return Datatype.OccupantLoadFactor; } }
        public static NeuralNetwork GetNetwork(WriteToCMDLine write)
        {
            var size = Enum.GetNames(typeof(OccLoadFactor)).Length;
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
            (Sample s, WriteToCMDLine write, bool tf = false)
        {
            double error = 0;
            var Pred = Predict(s.TextInput, CMDLibrary.WriteNull);
            if (s.DesiredOutput.ToList().IndexOf(s.DesiredOutput.Max()) != Pred.ToList().IndexOf(Pred.Max()) || tf)
            {
                NeuralNetwork net = GetNetwork(write);
                var Samples = s.ReadSamples(24);
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
                ctxt.Network.Save(Datatype.OccupantLoadFactor);

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
    }
}

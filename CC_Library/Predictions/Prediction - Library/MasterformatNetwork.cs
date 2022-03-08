using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;
using System.Runtime.InteropServices;

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
                net.Layers.Add(new Layer(100, a.GetSize(), Activation.LRelu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(100, net.Layers.Last().Weights.GetLength(0), Activation.LRelu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(40, net.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));
            }
            return net;
        }
        public static double[] Predict(string s, WriteToCMDLine write)
        {
            NeuralNetwork net = GetNetwork(write);
            Alpha2 a = datatype.LoadAlpha(write);
            var mem = a.CreateAlphaMemory(s);
            double[] Results = a.Forward(s, mem, write);
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

            //var Pred = Predict(s.TextInput, new WriteToCMDLine(CMDLibrary.WriteNull));

            //if (s.DesiredOutput.ToList().IndexOf(s.DesiredOutput.Max()) != Pred.ToList().IndexOf(Pred.Max()) || tf)
            {
                NeuralNetwork net = GetNetwork(write);
                var Samples = s.ReadSamples( 24);
                Alpha2 a = datatype.LoadAlpha(write);
                var am = a.CreateMemory();
                NeuralNetwork DictNet = Predictionary.GetNetwork(write);
                //Alpha a = new Alpha(write);
                //AlphaContext ctxt = new AlphaContext(datatype, write);
                NetworkMem MFMem = new NetworkMem(net);
                //NetworkMem AlphaMem = new NetworkMem(a.Network);
                //NetworkMem CtxtMem = new NetworkMem(ctxt.Network);

                try
                {
                    Parallel.For(0, Samples.Count(), j =>
                    {
                    //AlphaMem am = new AlphaMem(Samples[j].TextInput.ToCharArray());
                    //var output = a.Forward(Samples[j].TextInput, ctxt, am);
                        var AMem = a.CreateAlphaMemory(Samples[j].TextInput, DictNet);
                        var output = a.Forward(Samples[j].TextInput, write, DictNet);
                        var F = net.Forward(output, dropout, write);
                        error += CategoricalCrossEntropy.Forward(F.Last().GetRank(0), Samples[j].DesiredOutput).Max();

                        var DValues = net.Backward(F, Samples[j].DesiredOutput, MFMem, write);
                        a.Backward(Samples[j].TextInput, DValues, AMem, am, write, DictNet);
                    //a.Backward(Samples[j].TextInput, DValues, ctxt, am, AlphaMem, CtxtMem);
                    });
                }
                catch (Exception e) { e.OutputError(); } 
                MFMem.Update(Samples.Count(), 0.00001, net);
                a.Update(am, Samples.Count());
                //AlphaMem.Update(Samples.Count(), 0.00001, a.Network);
                //CtxtMem.Update(Samples.Count(), 0.00001, ctxt.Network);
                write("Pre Training Error : " + error);

                net.Save();
                a.Save();
                //a.Network.Save();
                //ctxt.Network.Save(Datatype.Masterformat);

                error = 0;
                Parallel.For(0, Samples.Count(), j =>
                {
                    var AMem = a.CreateAlphaMemory(Samples[j].TextInput, DictNet);
                    var output = a.Forward(Samples[j].TextInput, AMem, write, DictNet);
                    var F = net.Forward(output, 0, write);
                    error += CategoricalCrossEntropy.Forward(F.Last().GetRank(0), Samples[j].DesiredOutput).Max();
                    //AlphaMem am = new AlphaMem(Samples[j].TextInput.ToCharArray());
                    //var output = a.Forward(Samples[j].TextInput, ctxt, am);
                    //var F = net.Forward(output, dropout, write);
                    //error += CategoricalCrossEntropy.Forward(F.Last().GetRank(0), Samples[j].DesiredOutput).Max();
                });
                write("Post Training Error : " + error);

                //s.Save();
            }
            return error;
        }
    }
}
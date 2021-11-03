using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;
using System.Runtime.InteropServices;

namespace CC_Library.Predictions
{
    public class MasterformatNetwork
    {
        private const double dropout = 0.1;
        public Datatype datatype { get { return Datatype.Masterformat; } }
        public NeuralNetwork Network { get; }
        public MasterformatNetwork(Sample s)
        {
            Network = Datatype.Masterformat.LoadNetwork();
            if(Network.Datatype == Datatype.None.ToString() && s.Datatype == datatype.ToString())
            {
                Network = new NeuralNetwork(Datatype.Masterformat);
                Network.Layers.Add(new Layer(Alpha.DictSize, Alpha.DictSize, Activation.LRelu, 1e-5, 1e-5));
                Network.Layers.Add(new Layer(Alpha.DictSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 1e-5, 1e-5));
                Network.Layers.Add(new Layer(40, Network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));
            }
        }
        public double[] Predict(Sample s)
        {
            Alpha a = new Alpha();
            AlphaContext ctxt = new AlphaContext(Datatype.Masterformat);
            double[] Results = a.Forward(s.TextInput, ctxt);
            for(int i = 0; i < Network.Layers.Count(); i++)
            {
                Results = Network.Layers[i].Output(Results);
            }
            return Results;
        }
        public double Propogate
            (Sample s, WriteToCMDLine write, bool tf = false)
        {
            double error = 0;

            var Pred = Predict(s);

            if (s.DesiredOutput.ToList().IndexOf(s.DesiredOutput.Max()) != Pred.ToList().IndexOf(Pred.Max()) || tf)
            {
                var Samples = s.ReadSamples(24);
                Alpha a = new Alpha();
                AlphaContext ctxt = new AlphaContext(datatype);
                NetworkMem MFMem = new NetworkMem(Network);
                NetworkMem AlphaMem = new NetworkMem(a.Network);
                NetworkMem CtxtMem = new NetworkMem(ctxt.Network);

                Parallel.For(0, Samples.Count(), j =>
                {
                    AlphaMem am = new AlphaMem(Samples[j].TextInput.ToCharArray());
                    var output = a.Forward(Samples[j].TextInput, ctxt, am);
                    var F = Network.Forward(output, dropout, write);
                    error += CategoricalCrossEntropy.Forward(F.Last().GetRank(0), Samples[j].DesiredOutput).Max();

                    var DValues = Network.Backward(F, Samples[j].DesiredOutput, MFMem, write);
                    a.Backward(Samples[j].TextInput, DValues, ctxt, am, AlphaMem, CtxtMem);
                });
                MFMem.Update(Samples.Count(), 0.001, Network);
                AlphaMem.Update(Samples.Count(), 0.0001, a.Network);
                CtxtMem.Update(Samples.Count(), 0.001, ctxt.Network);
                write("Pre Training Error : " + error);
                
                Network.Save();
                a.Network.Save();
                ctxt.Network.Save();
                
                error = 0;
                Parallel.For(0, Samples.Count(), j =>
                {
                    AlphaMem am = new AlphaMem(Samples[j].TextInput.ToCharArray());
                    var output = a.Forward(Samples[j].TextInput, ctxt, am);
                    var F = Network.Forward(output, dropout, write);
                    error += CategoricalCrossEntropy.Forward(F.Last().GetRank(0), Samples[j].DesiredOutput).Max();
                });
                write("Post Training Error : " + error);
            }
            return error;
        }
    }
}

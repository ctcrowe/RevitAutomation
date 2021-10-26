using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;
using System.Runtime.InteropServices;

namespace CC_Library.Predictions
{
    public class MasterformatNetwork : INetworkPredUpdater
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
                Network.Layers.Add(new Layer(Alpha.DictSize, Alpha.DictSize, Activation.LRelu, 2e-5, 2e-5));
                Network.Layers.Add(new Layer(Alpha.DictSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 2e-5, 2e-5));
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
        public List<double[]> Forward(Sample s)
        {
            List<double[]> Results = new List<double[]>();
            Results.Add(s.TextOutput);

            for (int k = 0; k < Network.Layers.Count(); k++)
            {
                Results.Add(Network.Layers[k].Output(Results.Last()));
            }

            return Results;
        }
        public double[] Backward
            (Sample s,
            List<double[]> Results,
             NetworkMem mem)
        {
            var DValues = s.DesiredOutput;

            for (int l = Network.Layers.Count() - 1; l >= 0; l--)
            {
                DValues = mem.Layers[l].DActivation(DValues, Results[l + 1]);
                mem.Layers[l].DBiases(DValues, Network.Layers[l]);
                mem.Layers[l].DWeights(DValues, Results[l], Network.Layers[l]);
                DValues = mem.Layers[l].DInputs(DValues, Network.Layers[l]);
            }
            return DValues.ToList().Take(Alpha.DictSize).ToArray();
        }
        public void Propogate
            (Sample s, WriteToCMDLine write)
        {
            Alpha a = new Alpha();
            AlphaContext ctxt = new AlphaContext(datatype);
            NetworkMem MFMem = new NetworkMem(Network);
            NetworkMem AlphaMem = new NetworkMem(a.Network);
            NetworkMem CtxtMem = new NetworkMem(ctxt.Network);

            AlphaMem am = new AlphaMem(s.TextInput.ToCharArray());
            
            s.TextOutput = a.Forward(s.TextInput, ctxt, am);
            var F = Network.Forward(s.TextOutput, dropout, write);
            write("Predictions : " + F.Last().GetRank(0).GenText());
            write("F Desired : " + s.DesiredOutput.GenText());
            var Error = CategoricalCrossEntropy.Forward(F.Last().GetRank(0), s.DesiredOutput);
            write("Max Error : " + Error.Max());
            write("");

            if (s.DesiredOutput.ToList().IndexOf(s.DesiredOutput.Max()) != F.Last().GetRank(0).ToList().IndexOf(F.Last().GetRank(0).Max()))
            {
                var DValues = Network.Backward(F, s.DesiredOutput, MFMem, write);
                a.Backward(s.TextInput, DValues, ctxt, am, AlphaMem, CtxtMem);
                
                MFMem.Update(1, 1e-3, Network);
                AlphaMem.Update(1, 1e-3, a.Network);
                CtxtMem.Update(1, 1e-3, ctxt.Network);
                
                Network.Save();
                a.Network.Save();
                ctxt.Save();
            }
            s.Save();
        }
    }
}

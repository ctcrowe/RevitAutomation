using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;

//Next Generation
//Reduce outputs to 2, Lower, Higher

namespace CC_Library.Predictions
{
    public class AppleNetwork
    {
        private const double dropout = 0.1;
        public Datatype datatype { get { return Datatype.AAPL; } }
        public NeuralNetwork Network { get; }
        
        public AppleNetwork()
        {
            Network = datatype.LoadNetwork(CMDLibrary.WriteNull);
            if(Network.Datatype == Datatype.None)
            {
                Network = new NeuralNetwork(datatype);
                Network.Layers.Add(new Layer(Stonk.MktSize, Stonk.MktSize, Activation.LRelu));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 2e-5, 2e-5));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 2e-5, 2e-5));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 2e-5, 2e-5));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 2e-5, 2e-5));
                Network.Layers.Add(new Layer(2, Network.Layers.Last().Weights.GetLength(0), Activation.SoftMax));
            }
        }
        public int Predict(ValueSet val, WriteToCMDLine write)
        {
            Stonk st = new Stonk();
            StonkContext ctxt = new StonkContext(Datatype.AAPL);
            var comps = Comparison.GenerateComparisons(val);
            double[] Results = st.Forward(comps, ctxt);
            Results = Network.Forward(Results);
            return Results.ToList().IndexOf(Results.Max());
        }
        public void Propogate
            (ValueSet val, WriteToCMDLine write)
        {

            Stonk stk = new Stonk();
            StonkContext ctxt = new StonkContext(datatype);
            var vals = val.ReadValues(Datatypes.Datatype.AAPL, 24);

            NetworkMem AAPLMem = new NetworkMem(Network);
            NetworkMem StkMem = new NetworkMem(stk.Network);
            NetworkMem CtxtMem = new NetworkMem(ctxt.Network);
            double e = 0;

            Parallel.For(0, vals.Count(), j =>
                         {
                             try
                             {
                                List<Comparison> comps = Comparison.GenerateComparisons(vals[j]);
                                 if (j == 0 || j == 1)
                                     write("Comparisons : " + comps.Count());
                                StonkMem sm = new StonkMem(comps.Count());
            
                                var MktOutput = stk.Forward(comps, ctxt, sm);
                                var F = Network.Forward(MktOutput, dropout, write);
                                var output = new double[2];
                                int opnumb = vals[j].Increase ? 1 : 0;
                                output[opnumb] = 1;
                
                                var Error = CategoricalCrossEntropy.Forward(F.Last().GetRank(0), output);
                                e += Error.Max();
                                var D = Network.Backward(F, output, AAPLMem, write);
                                stk.Backward(D, ctxt, sm, StkMem, CtxtMem);
                             }
                             catch { }
                         });
            write("Samples : " + vals.Count());
            write("Loss : " + e);
            AAPLMem.Update(vals.Count(), 1e-4, Network);
            StkMem.Update(vals.Count(), 1e-4, stk.Network);
            CtxtMem.Update(vals.Count(), 1e-4, ctxt.Network);
            
            Network.Save();
            stk.Network.Save();
            ctxt.Save();
        }
    }
}

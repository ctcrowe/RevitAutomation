using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;

//Next Generation
//Reduce outputs to 3, Mid, High, Low

namespace CC_Library.Predictions
{
    public class AppleNetwork
    {
        private const double dropout = 0.1;
        public Datatype datatype { get { return Datatype.AAPL; } }
        public NeuralNetwork Network { get; }
        
        public AppleNetwork()
        {
            Network = datatype.LoadNetwork();
            if(Network.Datatype == Datatype.None.ToString())
            {
                Network = new NeuralNetwork(datatype);
                Network.Layers.Add(new Layer(Stonk.MktSize, Stonk.MktSize, Activation.LRelu));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 2e-5, 2e-5));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 2e-5, 2e-5));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 2e-5, 2e-5));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 2e-5, 2e-5));
                Network.Layers.Add(new Layer(3, Network.Layers.Last().Weights.GetLength(0), Activation.SoftMax));
            }
        }
        public int Predict(List<StonkValues> vals, WriteToCMDLine write)
        {
            Stonk st = new Stonk();
            StonkContext ctxt = new StonkContext(Datatype.AAPL);
            var comps = Comparison.GenerateComparisons(vals);
            double[] Results = st.Forward(comps, ctxt);
            //double[] MinResults = MaxResults.Duplicate();
            Results = Network.Forward(Results);
            return Results.ToList().IndexOf(Results.Max());
        }
        public void Propogate
            (List<List<StonkValues>> vals, double[] max, WriteToCMDLine write)
        {

            Stonk stk = new Stonk();
            StonkContext ctxt = new StonkContext(datatype);

            NetworkMem AAPLMem = new NetworkMem(Network);
            NetworkMem StkMem = new NetworkMem(stk.Network);
            NetworkMem CtxtMem = new NetworkMem(ctxt.Network);

            Parallel.For(0, vals.Count, j =>
                         {
                            List<Comparison> comps = Comparison.GenerateComparisons(vals[j]);
                            StonkMem sm = new StonkMem(comps.Count());
            
                            var MktOutput = stk.Forward(comps, ctxt, sm);
                            var F = Network.Forward(MktOutput, dropout, write);

                            write("Predictions : " + F.Last().GetRank(0).GenText());

                            write("F Desired : " + max.GenText());
            
                            var Error = CategoricalCrossEntropy.Forward(F.Last().GetRank(0), max);
                            write("Max Error : " + Error.GenText());
                            write("");

                            var D = Network.Backward(F, max, AAPLMem, write);
                            stk.Backward(D, ctxt, sm, StkMem, CtxtMem);
                         });

            AAPLMem.Update(vals.Count(), 0.01, Network);
            StkMem.Update(vals.Count(), 0.01, stk.Network);
            CtxtMem.Update(vals.Count(), 0.01, ctxt.Network);
            
            Network.Save();
            stk.Network.Save();
            ctxt.Save();
        }
    }
}

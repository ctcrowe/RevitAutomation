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
        //public NeuralNetwork MaxNetwork { get; }
        //public NeuralNetwork MinNetwork { get; }
        public AppleNetwork()
        {
            Network = datatype.LoadNetwork();
            if(Network.Datatype == Datatype.None.ToString())
            {
                Network = new NeuralNetwork(datatype);
                Network.Layers.Add(new Layer(Stonk.MktSize, Stonk.MktSize, Activation.LRelu));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(3, Network.Layers.Last().Weights.GetLength(0), Activation.SoftMax));
            }
            /*
            MaxNetwork = Datatype.MaxValue.LoadNetwork(datatype);
            if (MaxNetwork.Datatype == Datatype.None.ToString())
            {
                MaxNetwork = new NeuralNetwork(Datatype.MaxValue);
                MaxNetwork.Layers.Add(new Layer(Stonk.MktSize, Stonk.MktSize, Activation.LRelu));
                MaxNetwork.Layers.Add(new Layer(Stonk.MktSize, MaxNetwork.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                MaxNetwork.Layers.Add(new Layer(Stonk.MktSize, MaxNetwork.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                MaxNetwork.Layers.Add(new Layer(24, MaxNetwork.Layers.Last().Weights.GetLength(0), Activation.SoftMax));
            }
            
            MinNetwork = Datatype.MinValue.LoadNetwork(datatype);
            if (MinNetwork.Datatype == Datatype.None.ToString())
            {
                MinNetwork = new NeuralNetwork(Datatype.MinValue);
                MinNetwork.Layers.Add(new Layer(Stonk.MktSize, Stonk.MktSize, Activation.LRelu));
                MinNetwork.Layers.Add(new Layer(Stonk.MktSize, MinNetwork.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                MinNetwork.Layers.Add(new Layer(Stonk.MktSize, MinNetwork.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                MinNetwork.Layers.Add(new Layer(24, MinNetwork.Layers.Last().Weights.GetLength(0), Activation.SoftMax));
            }
            */
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
            /*
            MinResults = MinNetwork.Forward(MinResults);
            return new int[2]
            {
                MaxResults.ToList().IndexOf(MaxResults.Max()),
                MinResults.ToList().IndexOf(MinResults.Max())
            };
            */
        }
        public void Propogate
            (List<StonkValues> vals, double[] max, WriteToCMDLine write)
        {
            List<Comparison> comps = Comparison.GenerateComparisons(vals);

            Stonk stk = new Stonk();
            StonkContext ctxt = new StonkContext(datatype);
            StonkMem sm = new StonkMem(comps.Count());

            //NetworkMem MaxAAPLMem = new NetworkMem(MaxNetwork);
            //NetworkMem MinAAPLMem = new NetworkMem(MinNetwork);
            NetworkMem AAPLMem = new NetworkMem(Network);
            NetworkMem StkMem = new NetworkMem(stk.Network);
            NetworkMem CtxtMem = new NetworkMem(ctxt.Network);

            var MktOutput = stk.Forward(comps, ctxt, sm);
            var F = Network.Forward(MktOutput, dropout, write);
            //var MaxF = MaxNetwork.Forward(MktOutput, dropout, write);
            //var MinF = MinNetwork.Forward(MktOutput, dropout, write);

            write("Predictions : " + F.Last().GetRank(0).GenText());
            //write("Min Predictions : " + MinF.Last().GetRank(0).GenText());

            write("F Desired : " + max.GenText());
            //write("Min Desired : " + min.GenText());
            
            var Error = CategoricalCrossEntropy.Forward(F.Last().GetRank(0), max);
            write("Max Error : " + Error.GenText());
            /*
            var MinError = CategoricalCrossEntropy.Forward(MinF.Last().GetRank(0), min);
            write("Min Error : " + MinError.GenText());
            */
            write("");

            var D = Network.Backward(F, max, AAPLMem, write);
            //var MinD = MinNetwork.Backward(MinF, min, MinAAPLMem, write);

            stk.Backward(D, ctxt, sm, StkMem, CtxtMem);
            //stk.Backward(MinD, ctxt, sm, StkMem, CtxtMem);

            AAPLMem.Update(1, 0.1, Network);
            //MinAAPLMem.Update(1, 0.1, MinNetwork);
            StkMem.Update(1, 0.1, stk.Network);
            CtxtMem.Update(1, 0.1, ctxt.Network);
            /*
            MaxNetwork.Save();
            MinNetwork.Save();
            */
            Network.Save();
            stk.Network.Save();
            ctxt.Save();
        }
    }
}

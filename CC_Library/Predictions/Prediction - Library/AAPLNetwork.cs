using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public class AppleNetwork
    {
        private const double dropout = 0.2;
        public Datatype datatype { get { return Datatype.AAPL; } }
        public NeuralNetwork MaxNetwork { get; }
        public NeuralNetwork MinNetwork { get; }
        public AppleNetwork()
        {
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
        }
        public int[] Predict(List<StonkValues> vals, WriteToCMDLine write)
        {
            Stonk st = new Stonk();
            StonkContext ctxt = new StonkContext(Datatype.AAPL);
            var comps = Comparison.GenerateComparisons(vals);
            double[] MaxResults = st.Forward(comps, ctxt);
            double[] MinResults = MaxResults.Duplicate();
            MaxResults = MaxNetwork.Forward(MaxResults);
            MinResults = MinNetwork.Forward(MinResults);
            return new int[2]
            {
                MaxResults.ToList().IndexOf(MaxResults.Max()),
                MinResults.ToList().IndexOf(MinResults.Max())
            };
        }
        public double[] MaxBackward
            (List<double[]> Results,
             double[] desired,
             NetworkMem mem,
             WriteToCMDLine write)
        {
            var DValues = desired;

            for (int l = MaxNetwork.Layers.Count() - 1; l >= 0; l--)
            {
                try
                {
                    DValues = mem.Layers[l].DActivation(DValues, Results[l + 1]);
                    mem.Layers[l].DBiases(DValues);
                    mem.Layers[l].DWeights(DValues, Results[l]);
                    DValues = mem.Layers[l].DInputs(DValues, MaxNetwork.Layers[l]);
                }
                catch (Exception e)
                {
                    write("Failed at Layer : " + l);
                    e.OutputError();
                }
            }
            return DValues;
        }
        public double[] MinBackward
            (List<double[]> Results,
             double[] desired,
             NetworkMem mem,
             WriteToCMDLine write)
        {
            var DValues = desired;

            for (int l = MinNetwork.Layers.Count() - 1; l >= 0; l--)
            {
                try
                {
                    DValues = mem.Layers[l].DActivation(DValues, Results[l + 1]);
                    mem.Layers[l].DBiases(DValues);
                    mem.Layers[l].DWeights(DValues, Results[l]);
                    DValues = mem.Layers[l].DInputs(DValues, MinNetwork.Layers[l]);
                }
                catch (Exception e)
                {
                    write("Failed at Layer : " + l);
                    e.OutputError();
                }
            }
            return DValues;
        }
        public void Propogate
            (List<StonkValues> vals, double[] max, double[] min, WriteToCMDLine write)
        {
            List<Comparison> comps = Comparison.GenerateComparisons(vals);

            Stonk stk = new Stonk();
            StonkContext ctxt = new StonkContext(datatype);
            StonkMem sm = new StonkMem(comps.Count());

            NetworkMem MaxAAPLMem = new NetworkMem(MaxNetwork);
            NetworkMem MinAAPLMem = new NetworkMem(MinNetwork);
            NetworkMem StkMem = new NetworkMem(stk.Network);
            NetworkMem CtxtMem = new NetworkMem(ctxt.Network);

            var MktOutput = stk.Forward(comps, ctxt, sm);
            var MaxF = MaxNetwork.Forward(MktOutput, dropout);
            var MinF = MinNetwork.Forward(MktOutput, dropout);

            write("Max Predictions : " + MaxF.Last().GenText());
            write("Min Predictions : " + MinF.Last().GenText());

            write("Max Desired : " + max.GenText());
            write("Min Desired : " + min.GenText());
            
            var MaxError = CategoricalCrossEntropy.Forward(MaxF.Last(), max);
            write("Max Error : " + MaxError.GenText());
            var MinError = CategoricalCrossEntropy.Forward(MinF.Last(), min);
            write("Min Error : " + MinError.GenText());
            write("");

            var MaxD = MaxBackward(MaxF, max, MaxAAPLMem, write);
            var MinD = MinBackward(MinF, min, MinAAPLMem, write);
            stk.Backward(MaxD, ctxt, sm, StkMem, CtxtMem);
            stk.Backward(MinD, ctxt, sm, StkMem, CtxtMem);

            MaxAAPLMem.Update(1, 0.1, MaxNetwork);
            MinAAPLMem.Update(1, 0.1, MinNetwork);
            StkMem.Update(1, 0.1, stk.Network);
            CtxtMem.Update(1, 0.1, ctxt.Network);

            MaxNetwork.Save();
            MinNetwork.Save();
            stk.Network.Save();
            ctxt.Save();
        }
    }
}

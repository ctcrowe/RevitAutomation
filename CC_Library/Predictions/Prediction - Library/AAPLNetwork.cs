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
        public Datatype datatype { get { return Datatype.AAPL; } }
        public NeuralNetwork Network { get; }
        public AppleNetwork()
        {
            Network = datatype.LoadNetwork();
            if (Network.Datatype == Datatype.None.ToString())
            {
                Network = new NeuralNetwork(Datatype.AAPL);
                Network.Layers.Add(new Layer(Stonk.MktSize, Stonk.MktSize, Activation.LRelu));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(Stonk.MktSize, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(2, Network.Layers.Last().Weights.GetLength(0), Activation.SoftMax));
            }
        }
        public int Predict(List<StonkValues> vals)
        {
            Stonk st = new Stonk();
            StonkContext ctxt = new StonkContext(Datatype.AAPL);
            var comps = Comparison.GenerateComparisons(vals);
            double[] Results = st.Forward(comps, ctxt);
            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                Results = Network.Layers[i].Output(Results);
            }
            return Results.ToList().IndexOf(Results.Max());
        }
        public List<double[]> Forward(double[] vals)
        {
            List<double[]> Results = new List<double[]>();
            Results.Add(vals);
            for (int k = 0; k < Network.Layers.Count(); k++)
            {
                Results.Add(Network.Layers[k].Output(Results.Last()));
            }
            return Results;
        }
        public double[] Backward
            (List<double[]> Results,
             double[] desired,
             NetworkMem mem,
             WriteToCMDLine write)
        {
            var DValues = desired;

            for (int l = Network.Layers.Count() - 1; l >= 0; l--)
            {
                try
                {
                    DValues = mem.Layers[l].DActivation(DValues, Results[l + 1]);
                    mem.Layers[l].DBiases(DValues);
                    mem.Layers[l].DWeights(DValues, Results[l]);
                    DValues = mem.Layers[l].DInputs(DValues, Network.Layers[l]);
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
            (List<StonkValues> vals, bool increase, WriteToCMDLine write)
        {
            List<Comparison> comps = Comparison.GenerateComparisons(vals);

            Stonk stk = new Stonk();
            StonkContext ctxt = new StonkContext(datatype);
            StonkMem sm = new StonkMem(comps.Count());

            NetworkMem AAPLMem = new NetworkMem(Network);
            NetworkMem StkMem = new NetworkMem(stk.Network);
            NetworkMem CtxtMem = new NetworkMem(ctxt.Network);

            var MktOutput = stk.Forward(comps, ctxt, sm);
            var F = Forward(MktOutput);
            double[] inc = increase ? new double[2] { 1 , 0 } : new double[2] { 0 , 1};
            write(increase ? "Price Increse" : "Price Decrease");
            var Error = CategoricalCrossEntropy.Forward(F.Last(), inc).Sum();
            write("test Forward Count : " + F.Count());
            write("Test Error : " + Error);

            var DValues = Backward(F, inc, AAPLMem, write);
            stk.Backward(DValues, ctxt, sm, StkMem, CtxtMem);

            AAPLMem.Update(1, 0.1, Network);
            StkMem.Update(1, 0.1, stk.Network);
            CtxtMem.Update(1, 0.1, ctxt.Network);
         
            Network.Save();
            stk.Network.Save();
            ctxt.Save();
        }
    }
}

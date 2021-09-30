﻿using System;
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
            if (Network.Datatype == Datatype.None.ToString() && s.Datatype == datatype.ToString())
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
            StonkContext ctxt = new StonkContext();
            double[] Results = st.Forward(vals, ctxt);
            for(int i = 0; i < Network.Layers.Count(); i++)
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
            (Sample s,
            List<double[]> Results,
             NetworkMem mem)
        {
            var DValues = s.DesiredOutput;

            for (int l = Network.Layers.Count() - 1; l >= 0; l--)
            {
                DValues = mem.Layers[l].DActivation(DValues, Results[l + 1]);
                mem.Layers[l].DBiases(DValues);
                mem.Layers[l].DWeights(DValues, Results[l]);
                DValues = mem.Layers[l].DInputs(DValues, Network.Layers[l]);
            }
            return DValues;
        }
        public void Propogate
            (List<StonkValues> vals, WriteToCMDLine write)
        {
            NetworkMem AAPLMem = new NetworkMem(Network);

            Parallel.For(0, Samples.Count(), j =>
            {
                Samples[j].MktOutput = stk.Forward(Samples[j].MktVals, ctxt, am);
                var F = Forward(Samples[j]);
                var Error = CategoricalCrossEntropy.Forward(F.Last(), Samples[j].DesiredOutput).Sum();
                write("Test Error : " + Error);

                var DValues = Backward(Samples[j], F, AAPLMem);
                stk.Backward(Samples[j].TextInput, DValues, ctxt, am, StkMem, CtxtMem);
            });
            AAPLMem.Update(1, 0.0001, Network);
            StkMem.Update(1, 0.00001, stk.Network);
            CtxtMem.Update(1, 0.0001, ctxt.Network);
         
            Network.Save();
            stk.Network.Save();
            ctxt.Save();

            s.Save();
        }
    }
}

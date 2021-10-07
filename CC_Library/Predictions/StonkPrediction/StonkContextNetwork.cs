using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System;
using CC_Library.Predictions;


namespace CC_Library.Predictions
{
    public class StonkContext
    {
        private Datatype datatype { get; }
        public NeuralNetwork Network { get; }
        public const int SearchRange = 2;
        public StonkContext(Datatype dt)
        {
            datatype = dt;
            Network = Datatype.StonkContext.LoadNetwork(dt);
            if (Network.Datatype == Datatype.None.ToString())
            {
                Network = new NeuralNetwork(Datatype.StonkContext);
                Network.Layers.Add(new Layer(1, 9, Activation.Linear));
            }
        }
        public void Save()
        {
            Network.Save(datatype);
        }
        public void Contextualize(double[] Comparison, int number, StonkMem sm)
        {
            sm.LocalContextOutputs[number].Add(Comparison);
            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                sm.LocalContextOutputs[number].Add(Network.Layers[i].Output(sm.LocalContextOutputs.Last().Last()));
            }
        }
        public double Contextualize(List<Comparison> vals, int c)
        {
            var result = vals[c].Values;
            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                result = Network.Layers[i].Output(result);
            }
            return result.First();
        }
        public void Backward(double[] DValues, int runs, StonkMem sm, NetworkMem mem)
        {
            Parallel.For(0, runs, j =>
            {
                double[] cdv = new double[1] { DValues[j] };
                for (int i = Network.Layers.Count() - 1; i >= 0; i--)
                {
                    try
                    {
                        cdv = mem.Layers[i].DActivation(cdv, sm.LocalContextOutputs[j][i + 1]);
                        mem.Layers[i].DBiases(cdv);
                        mem.Layers[i].DWeights(cdv, sm.LocalContextOutputs[j][i]);
                        cdv = mem.Layers[i].DInputs(cdv, Network.Layers[i]);
                    }
                    catch (Exception e) { e.OutputError(); }
                }
            });
        }
    }
}

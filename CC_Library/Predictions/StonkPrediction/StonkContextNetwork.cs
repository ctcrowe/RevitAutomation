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
            Network = Datatype.StonkContext.LoadNetwork(CMDLibrary.WriteNull, dt);
            if (Network.Datatype == Datatype.None)
            {
                Network = new NeuralNetwork(Datatype.StonkContext);
                Network.Layers.Add(new Layer(1, 8, Activation.Linear));
            }
        }
        public void Save()
        {
            Network.Save(datatype);
        }
        public void Contextualize(Comparison comp, int number, StonkMem sm)
        {
            var result = comp.Values.Duplicate();
            sm.LocalContextOutputs[number].Add(result);
            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                result = Network.Layers[i].Output(result);
                sm.LocalContextOutputs[number].Add(result);
            }
        }
        public double Contextualize(Comparison val)
        {
            double[] result = val.Values.Duplicate();
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
                        mem.Layers[i].DBiases(cdv, Network.Layers[i], runs);
                        mem.Layers[i].DWeights(cdv, sm.LocalContextOutputs[j][i], Network.Layers[i], runs);
                        cdv = mem.Layers[i].DInputs(cdv, Network.Layers[i]);
                    }
                    catch (Exception e) { e.OutputError(); }
                }
            });
        }
    }
}

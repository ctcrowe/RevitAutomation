using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System;
using CC_Library.Predictions;


namespace CC_Library.Predictions
{
    internal class StonkContext
    {
        private Datatype datatype { get; }
        public NeuralNetwork Network { get; }
        public const int SearchRange = 2;
        internal StonkContext(Datatype dt)
        {
            datatype = dt;
            Network = Datatype.StonkContext.LoadNetwork(dt);
            if (Network.Datatype == Datatype.None.ToString())
            {
                Network = new NeuralNetwork(Datatype.StonkContext);
                Network.Layers.Add(new Layer(1, 9 * (1 + (2 * SearchRange)), Activation.Linear));
            }
        }
        public void Save()
        {
            Network.Save(datatype);
        }
        public double Contextualize(string s, int c, StonkMem sm)
        {
            am.LocalContextOutputs[c].Add(CharSet.Locate(s, c, SearchRange));
            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                am.LocalContextOutputs[c].Add(Network.Layers[i].Output(sm.LocalContextOutputs[c].Last()));
            }
            return am.LocalContextOutputs[c].Last().First();
        }
        public double Contextualize(string s, int c)
        {
            var result = CharSet.Locate(s, c, SearchRange);
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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System;
using CC_Library.Predictions;


namespace CC_Library.Predictions
{
    internal class Filter1
    {
        private Datatype datatype { get; }
        public NeuralNetwork Network { get; }
        public const int SearchRange = 3;
        internal Filter1(Datatype dt, WriteToCMDLine write, int numb = 0)
        {
            datatype = dt;
            switch(numb)
            {
                default:
                case 0: Network = Datatype.AlphaContextPrimary.LoadNetwork(write, dt); break;
                case 1: Network = Datatype.AlphaContextSecondary.LoadNetwork(write, dt); break;
                case 2: Network = Datatype.AlphaContextTertiary.LoadNetwork(write, dt); break;
            }
            if(Network.Datatype == Datatype.None)
            {
                switch(numb)
                {
                    default:
                    case 0: Network = new NeuralNetwork(Datatype.AlphaContextPrimary); break;
                    case 1: Network = new NeuralNetwork(Datatype.AlphaContextSecondary); break;
                    case 2: Network = new NeuralNetwork(Datatype.AlphaContextTertiary); break;
                }
                Network.Layers.Add(new Layer(1, CharSet.CharCount * (1 + (2 * SearchRange)), Activation.Linear));
            }
        }
        public void Save()
        {
            Network.Save(datatype);
        }
        public double Contextualize(string s, int c, AlphaMem am)
        {
            am.LocalContextOutputs[c].Add(CharSet.Locate(s, c, SearchRange));
            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                am.LocalContextOutputs[c].Add(Network.Layers[i].Output(am.LocalContextOutputs[c].Last()));
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
        public void Backward(double[] DValues, int runs, AlphaMem am, NetworkMem mem)
        {
            Parallel.For(0, runs, j =>
            {
                double[] cdv = new double[1] { DValues[j] / runs };
                for (int i = Network.Layers.Count() - 1; i >= 0; i--)
                {
                    try
                    {
                        cdv = mem.Layers[i].DActivation(cdv, am.LocalContextOutputs[j][i + 1]);
                        mem.Layers[i].DBiases(cdv, Network.Layers[i], runs);
                        mem.Layers[i].DWeights(cdv, am.LocalContextOutputs[j][i], Network.Layers[i], runs);
                        cdv = mem.Layers[i].DInputs(cdv, Network.Layers[i]);
                    }
                    catch (Exception e) { e.OutputError(); }
                }
            });
        }
    }
}

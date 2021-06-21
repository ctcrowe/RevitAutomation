using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System;
using CC_Library.Predictions;


namespace CC_Library.Predictions
{
    internal class AlphaContext
    {
        private Datatype datatype { get; }
        public NeuralNetwork Network { get; }
        internal AlphaContext(Datatype dt, WriteToCMDLine write)
        {
            datatype = dt;
            Network = Datatype.AlphaContext.LoadSpecialNetwork(dt, write);
        }
        public void Save()
        {
            Network.Save(datatype);
        }
        //This is always 0 or 1... need to fix UPDATE: I think this is fixed. Build and confirm tonight.
        public double Contextualize(char[] phrase, int c, AlphaMem am)
        {
            var result = Locate(phrase, c);
            am.LocalContextOutputs[c].Add(result);
            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                am.LocalContextOutputs[c].Add(Network.Layers[i].Output(am.LocalContextOutputs[c].Last()));
            }
            return am.LocalContextOutputs[c].Last().First();
        }
        public double Contextualize(char[] phrase, int c)
        {
            var result = Locate(phrase, c);
            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                result = Network.Layers[i].Output(result);
            }
            return result.First();
        }
        public void Backward(double[] DValues, int runs, AlphaMem am, NetworkMem mem, WriteToCMDLine write)
        {
            Parallel.For(0, runs, j =>
            {
                double[] cdv = new double[1] { DValues[j] };
                for (int i = Network.Layers.Count() - 1; i >= 0; i--)
                {
                    try
                    {
                        cdv = mem.Layers[i].DActivation(cdv, am.LocalContextOutputs[j][i + 1]);
                        mem.Layers[i].DBiases(cdv);
                        mem.Layers[i].DWeights(cdv, am.LocalContextOutputs[j][i]);
                        cdv = mem.Layers[i].DInputs(cdv, Network.Layers[i]);
                    }
                    catch
                    {
                        write("J is " + j);
                        write("CDV count " + cdv.Count());
                        write("ContextOutputs count " + am.LocalContextOutputs.Count());
                        write("ContextOutputs [j] count " + am.LocalContextOutputs[j].Count());
                        write("ContextOutputs [j][i+1] count " + am.LocalContextOutputs[j][i+1].Count());
                        write("ContextOutputs [j][i] count " + am.LocalContextOutputs[j][i].Count());
                    }
                }
            });
        }
        private static double[] Locate(char[] phrase, int numb)
        {
            double[] result = new double[Alpha.CharCount() * (Alpha.SearchSize + 1)];
            result[Alpha.LocationOf(phrase[numb])] = 1;
            for (int i = 0; i < Alpha.SearchSize / 2; i++)
            {
                if (numb - i >= 0)
                {
                    result[(i + 1 * Alpha.CharCount()) + Alpha.LocationOf(phrase[numb - i])] = 1;
                }
            }
            for (int i = 0; i < Alpha.SearchSize / 2; i++)
            {
                if (numb + i < phrase.Count())
                {
                    result[(((Alpha.SearchSize / 2) + i + 1) * Alpha.CharCount()) + Alpha.LocationOf(phrase[numb + i])] = 1;
                }
            }
            return result;
        }
    }
}

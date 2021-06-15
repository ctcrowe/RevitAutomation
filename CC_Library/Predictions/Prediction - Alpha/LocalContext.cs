using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System;
using CC_Library.Predictions;


namespace CC_Library.Predictions
{
    internal class LocalContext
    {
        private Datatype datatype { get; }
        public NeuralNetwork Network { get; }
        internal LocalContext(Datatype dt, WriteToCMDLine write)
        {
            datatype = dt;
            Network = Datatype.LocalContext.LoadSpecialNetwork(dt, write);
        }
        public void Save()
        {
            Network.Save(datatype);
        }
        public double Contextualize(char[] phrase, int c, AlphaMem am)
        {
            var result = Locate(phrase, c);
            am.LocalContextOutputs[c].Add(result);
            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                am.LocalContextOutputs[c].Add(Network.Layers[i].Output(am.LocalContextOutputs[c].Last()));
            }
            return result.First();
        }
        public void Backward(double[] DValues, int runs, AlphaMem am, WriteToCMDLine write)
        {
            Parallel.For(0, runs, j =>
            {
                double[] cdv = new double[1] { DValues[j] };
                for (int i = Network.Layers.Count() - 1; i >= 0; i--)
                {
                    try
                    {
                        cdv = Network.Layers[i].DActivation(cdv, am.LocalContextOutputs[j][i + 1]);
                        Network.Layers[i].DBiases(cdv);
                        Network.Layers[i].DWeights(cdv, am.LocalContextOutputs[j][i]);
                        cdv = Network.Layers[i].DInputs(cdv);
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

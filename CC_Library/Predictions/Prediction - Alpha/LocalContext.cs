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
        internal AlphaContext(Datatype dt, int numb = 0)
        {
            datatype = dt;
            switch(numb)
            {
                default:
                case 0:
                    Network = Datatype.AlphaContextPrimary.LoadNetwork(new WriteToCMDLine(CMDLibrary.WriteNull), dt);
                    break;
                case 1:
                    Network = Datatype.AlphaContextSecondary.LoadNetwork(new WriteToCMDLine(CMDLibrary.WriteNull), dt);
                    break;
                case 2:
                    Network = Datatype.AlphaContextTertiary.LoadNetwork(new WriteToCMDLine(CMDLibrary.WriteNull), dt);
                    break;
            }
        }
        public void Save()
        {
            Network.Save(datatype);
        }
        public double Contextualize(char[] phrase, int c, AlphaMem am)
        {
            am.LocalContextOutputs[c].Add(Locate(phrase, c));
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
        public void Backward(double[] DValues, int runs, AlphaMem am, NetworkMem mem)
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
                    catch (Exception e)
                    {
                        e.OutputError();
                    }
                }
            });
        }
        private static double[] Locate(char[] phrase, int numb)
        {
            double[] result = new double[Alpha.CharCount() * ((2 * Alpha.SearchSize) + 1)];
            result[Alpha.LocationOf(phrase[numb])] = 1;
            
            int imin = numb < Alpha.SeachSize? Alpha.SearchSize - numb : Alpha.SearchSize;
            int imax = numb + Alpha.SearchSize < phrase.Count()? Alpha.SearchSize : phrase.Count() - (numb + Alpha.SearchSize);
            
            Parallel.For(0, imin, i => result[((i + 1) * Alpha.CharCount()) + Alpha.LocationOf(phrase[numb - i])] = 1);
            Parallel.For(0, imax, i => result[((Alpha.SearchSize + i + 1) * Alpha.CharCount()) + Alpha.LocationOf(phrase[numb + i])] = 1);

            return result;
        }
    }
}

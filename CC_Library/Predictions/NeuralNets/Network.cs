using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    [Serializable]
    public class NeuralNetwork
    {
        public List<Layer> Layers { get; set; }
        public string Datatype { get; }

        public double[] Forward(double[] results)
        {
            for(int i = 0; i < Layers.Count(); i++)
            {
                results = Layers[i].Output(results);
            }
            return results;
        }
        public List<double[,]> Forward(double[] input, double dropout, WriteToCMDLine write)
        {
            List<double[,]> Results = new List<double[,]>();
            double[,] resultinput = new double[2,input.Count()];
            resultinput.SetRank(input, 0);
            resultinput.SetRank(input, 1);
            Results.Add(resultinput);
            for (int k = 0; k < Layers.Count(); k++)
            {
                double[,] output = new double[2, Layers[k].Biases.Count()];
                output.SetRank(Layers[k].Output(Results.Last().GetRank(1)), 0);
                var drop = DropOut(output.GetRank(0), dropout, write);
                output.SetRank(drop, 1);
                Results.Add(output);
            }
            return Results;
        }
        public double[] Backward(List<double[,]> Results, double[] desired, NetworkMem mem, WriteToCMDLine write)
        {
            var DValues = desired;

            for (int l = Layers.Count() - 1; l >= 0; l--)
            {
                try
                {
                    if (l != Layers.Count() - 1)
                        DValues = InverseDropOut(DValues, Results[l].GetRank(1));
                    DValues = mem.Layers[l].DActivation(DValues, Results[l + 1].GetRank(0));
                    mem.Layers[l].DBiases(DValues, Layers[l]);
                    mem.Layers[l].DWeights(DValues, Results[l].GetRank(0), Layers[l]);
                    DValues = mem.Layers[l].DInputs(DValues, Layers[l]);
                }
                catch (Exception e)
                {
                    write("Failed at Layer : " + l);
                    e.OutputError();
                }
            }
            return DValues;
        }
        private static double[] DropOut(double[] input, double rate, WriteToCMDLine write)
        {
            double[] output = new double[input.Count()];
            var DOLayer = input.RandomBinomial(rate);
            for(int i = 0; i < output.Count(); i++)
            {
                output[i] = input[i] * DOLayer[i] / (1 - rate);
            }
            return output;
        }
        private static double[] InverseDropOut(double[] DValues, double[] DropOutRank)
        {
            double[] output = new double[DValues.Count()];
            for(int i = 0; i < DValues.Count(); i++)
            {
                output[i] = DropOutRank[i] == 0 ? 0 : DValues[i];
            }
            return output;
        }
        public NeuralNetwork(Datatype datatype)
        {
            this.Layers = new List<Layer>();
            this.Datatype = datatype.ToString();
        }
    }
}

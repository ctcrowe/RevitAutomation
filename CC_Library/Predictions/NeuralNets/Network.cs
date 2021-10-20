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
        public List<double[,]> Forward(double[] input, double dropout)
        {
            List<double[]> Results = new List<double[]>();
            double[,] resultinput = new double[1,input.Count()];
            resultinput.SetRank(input, 0);
            Results.Add(resultinput);
            for (int k = 0; k < Layers.Count(); k++)
            {
                double[,] output = new double[2, Layers[k].Biases.Count()];
                output.SetRank(Layers[k].Output(Results.Last().GetRank(1)), 0);
                if(k != Layer.Count() - 1)
                    output.SetRank(Dropout(output.GetRank(0), dropout)), 1);
                Results.Add(output);
            }
            return Results;
        }
        public double[] Backkward(List<double[,]> Results, double[] desired, NetworkMem mem, WriteToCMDLine write)
        {
            var DValues = desired;

            for (int l = Layers.Count() - 1; l >= 0; l--)
            {
                try
                {
                    DValues = mem.Layers[l].DActivation(DValues, Results[l + 1].GetRank[0]);
                    mem.Layers[l].DBiases(DValues);
                    mem.Layers[l].DWeights(DValues, Results[l].GetRank[0]);
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
        private static double[] DropOut(double[] input, double rate)
        {
            Random r = new Random();
            double[] output = input.Duplicate();
            while ((double)output.Where(x => x == 0).Count() / (double)output.Count() < rate)
            {
                output[r.Next(0, output.Count() - 1)] = 0;
            }
            for(int i = 0; i < input.Count(); i++)
            {
                output[i] /= (1 - rate);
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

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
        public List<double[]> Forward(double[] input, double dropout)
        {
            List<double[]> Results = new List<double[]>();
            Results.Add(input);
            for (int k = 0; k < Layers.Count(); k++)
            {
                Results.Add(Layers[k].Output(Results.Last()));
                if(k != Layers.Count() - 1)
                {
                    Results.Add(DropOut(Results.Last(), dropout));
                }
            }
            return Results;
        }
        public double[] Backkward(List<double[]> Results, double[] desired, NetworkMem mem, WriteToCMDLine write)
        {
            var DValues = desired;

            for (int l = Layers.Count() - 1; l >= 0; l--)
            {
                try
                {
                    DValues = mem.Layers[l].DActivation(DValues, Results[l + 1]); //no longer Results[ l + 1]
                    //now its going to be something like 2 * l + 1
                    mem.Layers[l].DBiases(DValues);
                    mem.Layers[l].DWeights(DValues, Results[l]);
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

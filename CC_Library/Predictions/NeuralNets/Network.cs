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
        public Datatype Datatype { get; }

        public double[] Forward(double[] results)
        {
            for(int i = 0; i < Layers.Count(); i++)
            {
                results = Layers[i].Output(results);
            }
            return results;
        }
        public double[][][] Forward(double[] input, double dropout, WriteToCMDLine write, bool tf = false)
        {
            double[][][] Results = new double[Layers.Count() + 1][][];
            Results[0] = new double[2][];
            Results[0][0] = input;
            Results[0][1] = input;
            for (int k = 0; k < Layers.Count(); k++)
            {
                Results[k + 1] = new double[2][];
                try { Results[k + 1][0] = Layers[k].Output(Results[k][1]); }
                catch { Console.WriteLine("Failed at Network 0 Layer : " + i + ", inputs : " + Results[k][1].Count() + ", weights : " +
                    Layers[k].Weights.GetLength(0) + ", " + Layers[k].Weights.GetLength(1)); }
                Results[k + 1][1] = Layers[k].DropOut(Results[k + 1][0], dropout);
            }
            return Results;
        }
        public double[] Backward(double[][][] Results, double[] desired, NetworkMem mem, WriteToCMDLine write)
        {
            var DValues = desired;

            for (int l = Layers.Count() - 1; l >= 0; l--)
            {
                try
                {
                    DValues = Layers[l].InverseDropOut(DValues, Results[l + 1][1]);
                }
                catch (Exception e)
                {
                    write("Failed at Inverse Dropout layer " + l);
                    write("DValues : " + DValues.Count());
                    write("Results : " + Results[l + 1].GetRank(1).Count());
                    e.OutputError();
                }
                try { DValues = mem.Layers[l].DActivation(DValues, Results[l + 1][0]); }
                catch
                {
                    write("Failed at DActivation layer " + l);
                    write("DValues : " + DValues.Count());
                    write("Results : " + Results[l + 1].GetRank(0).Count());
                }
                try { mem.Layers[l].DBiases(DValues, Layers[l]); }
                catch
                {
                    write("Failed at DBiases layer " + l);
                    write("DValues : " + DValues.Count());
                    write("Biases : " + Layers[l].Biases.Count());
                }
                try { mem.Layers[l].DWeights(DValues, Results[l][1], Layers[l]); }
                catch
                {
                    write("Failed at DWeights layer " + l);
                    write("DValues : " + DValues.Count());
                    write("Inputs : " + Results[l].GetRank(1).Count());
                    write("Results : " + Results[l + 1].GetRank(0).Count());
                    write("Weights : " + Layers[l].Weights.GetLength(0) + ", " + Layers[l].Weights.GetLength(1));
                }
                try
                {
                    DValues = mem.Layers[l].DInputs(DValues, Layers[l]);
                }
                catch
                {
                    write("Failed at DInputs layer " + l);
                    write("DValues : " + DValues.Count());
                    write("Weights : " + Layers[l].Weights.GetLength(0) + ", " + Layers[l].Weights.GetLength(1));
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
        public NeuralNetwork(Datatype datatype)
        {
            this.Layers = new List<Layer>();
            this.Datatype = datatype;
        }
    }
}

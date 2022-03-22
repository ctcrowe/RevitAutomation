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
        public List<double[,]> Forward(double[] input, double dropout, WriteToCMDLine write, bool tf = false)
        {
            List<double[,]> Results = new List<double[,]>();
            double[,] resultinput = new double[2,input.Count()];
            resultinput.SetRank(input, 0);
            resultinput.SetRank(input, 1);
            Results.Add(resultinput);
            for (int k = 0; k < Layers.Count(); k++)
            {
                double[,] output = new double[2, Layers[k].Biases.Count()];
                var rank = Layers[k].Output(Results.Last().GetRank(1));
                if(rank.Any(x => double.IsNaN(x)))
                {
                    write("Layer " + k + " in " + Datatype.ToString() + " Network  has NaN Values");
                }
                output.SetRank(Layers[k].Output(Results.Last().GetRank(1)), 0);
                var drop = DropOut(output.GetRank(0), dropout, write);
                output.SetRank(drop, 1);
                Results.Add(output);
                if (tf)
                    output.GetRank(1).WriteArray(this.Datatype.ToString() + " Layer " + k + " output : ", write);
            }
            return Results;
        }
        public double[] Backward(List<double[,]> Results, double[] desired, NetworkMem mem, WriteToCMDLine write)
        {
            var DValues = desired;

            for (int l = Layers.Count() - 1; l >= 0; l--)
            {
                try { DValues = DValues.InverseDropOut(Results[l + 1].GetRank(1)); }
                catch (Exception e)
                {
                    write("Failed at Inverse Dropout layer " + l);
                    write("DValues : " + DValues.Count());
                    write("Results : " + Results[l + 1].GetRank(1).Count());
                    e.OutputError();
                }
                try { DValues = mem.Layers[l].DActivation(DValues, Results[l + 1].GetRank(0)); }
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
                try { mem.Layers[l].DWeights(DValues, Results[l].GetRank(0), Layers[l]); }
                catch
                {
                    write("Failed at DWeights layer " + l);
                    write("DValues : " + DValues.Count());
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

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

        public double[] Predict(double[] results)
        {
            for(int i = 0; i < Layers.Count(); i++)
            {
                results = Layers[i].Output(results);
            }
            return results;
        }
        public List<double[]> Forward(double[] input)
        {
            List<double[]> Results = new List<double[]>();
            Results.Add(input);
            for (int k = 0; k < Layers.Count(); k++)
            {
                Results.Add(Layers[k].Output(Results.Last()));
            }
            return Results;
        }
        public NeuralNetwork(Datatype datatype)
        {
            this.Layers = new List<Layer>();
            this.Datatype = datatype.ToString();
        }
    }
}

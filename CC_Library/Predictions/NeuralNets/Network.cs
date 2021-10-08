using System;
using System.Collections.Generic;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    [Serializable]
    public class NeuralNetwork
    {
        public List<Layer> Layers { get; set; }
        public string Datatype { get; }

        public NeuralNetwork(Datatype datatype)
        {
            this.Layers = new List<Layer>();
            this.Datatype = datatype.ToString();
        }
    }
}

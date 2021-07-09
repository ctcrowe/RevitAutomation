using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace CC_Library.Predictions
{
    [Serializable]
    public class NeuralNetwork
    {
        public List<Layer> Layers { get; set; }
        public string Datatype { get; set; }

        public NeuralNetwork(Datatype datatype)
        {
            this.Layers = new List<Layer>();
            this.Datatype = datatype.ToString();
        }
        /*
        internal void Update(int RunSize, double ChangeSize, NetworkMem nm)
        {
            Parallel.For(0, this.Layers.Count(), j =>
            {
                nm.Layers[j].DeltaB.Divide(RunSize);
                nm.Layers[j].DeltaW.Divide(RunSize);
                nm.Layers[j].Update(this.Layers[j], ChangeSize);
            });
        }
        */
    }
}

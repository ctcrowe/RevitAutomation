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
        public Datatype Datatype { get; set; }

        public NeuralNetwork()
        {
            this.Layers = new List<Layer>();
        }
        public void Update(int RunSize, double ChangeSize)
        {
            Parallel.For(0, this.Layers.Count(), j =>
            {
                this.Layers[j].DeltaB.Divide(RunSize);
                this.Layers[j].DeltaW.Divide(RunSize);
                this.Layers[j].Update(ChangeSize);
            });
        }
        internal void Update(int RunSize, double ChangeSize, NetworkMem nm)
        {
            Parallel.For(0, this.Layers.Count(), j =>
            {
                nm.Layers[j].DeltaB.Divide(RunSize);
                nm.Layers[j].DeltaW.Divide(RunSize);
                this.Layers[j].Update(ChangeSize, nm.Layers[j]);
            });
        }
    }
}
using System;
using System.Collections.Generic;
using CC_Library.Datatypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public abstract class NetworkPredUpdater
    {
        public abstract Datatype datatype { get; }
        internal abstract NeuralNetwork Network { get; }
        public abstract Sample Input { get; set; }
        internal abstract List<double[]> Forward(WriteToCMDLine Write);
        internal abstract double[] Backward(List<double[]> Results, NetworkMem mem, WriteToCMDLine Write);
        public abstract void Propogate(WriteToCMDLine write);
    }
}

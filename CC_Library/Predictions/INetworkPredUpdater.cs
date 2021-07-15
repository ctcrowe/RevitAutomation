using System;
using System.Collections.Generic;
using CC_Library.Datatypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public interface INetworkPredUpdater
    {
        public Datatype datatype { get; }
        public NeuralNetwork Network { get; }
        public Sample Input { get; set; }
        public double[] Predict(Sample s)
        public List<double[]> Forward(WriteToCMDLine Write);
        public double[] Backward(List<double[]> Results, NetworkMem mem, WriteToCMDLine Write);
        public void Propogate(WriteToCMDLine write);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public interface INetworkPredUpdater
    {
        NeuralNetwork Network { get; }
        double[] Input { get; set; }
        List<double[]> Forward(WriteToCMDLine Write);
        double[] Backward(List<double[]> Results, int Correct, NetworkMem mem, WriteToCMDLine Write);
        void Propogate(Sample s, WriteToCMDLine Write);
    }
}

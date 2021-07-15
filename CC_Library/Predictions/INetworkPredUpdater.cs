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
        Datatype datatype { get; }
        NeuralNetwork Network { get; }
        Sample Input { get; set; }
        List<double[]> Forward(WriteToCMDLine Write);
        double[] Backward(List<double[]> Results, NetworkMem mem, WriteToCMDLine Write);
        void Propogate(WriteToCMDLine Write);
    }
}

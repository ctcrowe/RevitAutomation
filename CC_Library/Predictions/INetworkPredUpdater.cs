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
        double[] Predict(Sample s);
        List<double[]> Forward(Sample s, WriteToCMDLine Write);
        double[] Backward(Sample s, List<double[]> Results, NetworkMem mem, WriteToCMDLine Write);
        void Propogate(Sample s, WriteToCMDLine write);
    }
}
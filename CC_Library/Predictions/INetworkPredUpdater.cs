using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public interface INetworkPredUpdater
    {
        string TextInput { get; }
        double[] ImgInput { get; }
        double[] OtherInput { get; }
        NeuralNetwork Network { get; }
        List<double[]> Results { get; set; }
        void Forward(WriteToCMDLine Write);
        void Backward();
    }
}

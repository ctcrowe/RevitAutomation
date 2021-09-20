using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;
using System.Runtime.InteropServices;

/// SUMMARY
/// Input : Candlestick data from last 3 days (AAPL) (64 values)
/// Input : Candlestick data from last 3 days Nasdaq (ONEQ) (64 values)
/// Input : Candlestick data from last 3 days (VTI) (64 values)
/// Input : Current value (AAPL)
/// Output : Likelyhood the value is trending down.
/// Output : Likelyhood the value is trending up.
/// Final output used if trending down and higher than purchase value, sell.
/// Final output used if trending up, and funds available, buy.

namespace CC_Library.Predictions
{
    public class AppleNetwork : INetworkPredUpdater
    {
        public Datatype datatype { get { return Datatype.AAPL; } }
        public NeuralNetwork Network { get; }
        public AppleNetwork()
        {
            Network = datatype.LoadNetwork();
            if(Network.Datatype == Datatype.None.ToString())
            {
                Network = new NeuralNetwork(Datatype.AAPL);
                Network.Layers.Add(new Layer(200, 482, Activation.LRelu));
                Network.Layers.Add(new Layer(200, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(200, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(2, Network.Layers.Last().Weights.GetLength(0), Activation.CombinedCrossEntropySoftmax));
            }
        }
        public double[] Predict(Sample s)
        {
            double[] Results = s.ValInput;
            for(int i = 0; i < Network.Layers.Count(); i++)
            {
                Results = Network.Layers[i].Output(Results);
            }
            return Results;
        }
        public List<double[]> Forward(Sample s)
        {
            List<double[]> Results = new List<double[]>();
            Results.Add(s.ValInput);

            for (int k = 0; k < Network.Layers.Count(); k++)
            {
                Results.Add(Network.Layers[k].Output(Results.Last()));
            }

            return Results;
        }
        public double[] Backward
            (Sample s,
            List<double[]> Results,
             NetworkMem mem)
        {
            var DValues = s.DesiredOutput;

            for (int l = Network.Layers.Count() - 1; l >= 0; l--)
            {
                DValues = mem.Layers[l].DActivation(DValues, Results[l + 1]);
                mem.Layers[l].DBiases(DValues);
                mem.Layers[l].DWeights(DValues, Results[l]);
                DValues = mem.Layers[l].DInputs(DValues, Network.Layers[l]);
            }
            return DValues;
        }
        public void Propogate
            (Sample s, WriteToCMDLine write)
        {
            var check = Predict(s);
            if(s.DesiredOutput.ToList().IndexOf(s.DesiredOutput.Max()) != check.ToList().IndexOf(check.Max()))
            {
                List<string> lines = new List<string>();
                
                for(int i = 0; i < 5; i++)
                {
                    var Samples = s.ReadSamples(24);
                    Accuracy Acc = new Accuracy(Samples);
                    NetworkMem mem = new NetworkMem(Network);
                    
                    Parallel.For(0, Samples.Count(), j =>
                    {
                        var F = Forward(Samples[j]);
                        Acc.Add(j,
                            CategoricalCrossEntropy.Forward(F.Last(), Samples[j].DesiredOutput).Sum(),
                            F.Last().ToList().IndexOf(F.Last().Max()),
                            Samples[j].DesiredOutput.ToList().IndexOf(Samples[j].DesiredOutput.Max()));
                    
                        var DValues = Backward(Samples[j], F, mem);
                    });
                    lines.AddRange(Acc.Get());
                    mem.Update(1, 0.000001, Network);
                }
                lines.ShowErrorOutput();
                Network.Save();

                s.Save();
            }
        }
    }
}

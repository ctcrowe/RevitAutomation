using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public class AppleNetwork : INetworkPredUpdater
    {
        public Datatype datatype { get { return Datatype.AAPL; } }
        public NeuralNetwork Network { get; }
        public AppleNetwork(Sample s)
        {
            Network = datatype.LoadNetwork();
            if (Network.Datatype == Datatype.None.ToString() && s.Datatype == datatype)
            {
                Network = new NeuralNetwork(Datatype.AAPL);
                Network.Layers.Add(new Layer(200, 482, Activation.LRelu));
                Network.Layers.Add(new Layer(200, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(200, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Network.Layers.Add(new Layer(16, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu));
                Pretrain(s);
            }
        }
        public double[] Predict(Sample s)
        {
            double[] Results = s.ValInput;
            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                Results = Network.Layers[i].Output(Results);
            }
            return Results;
        }
        public List<double[]> Forward(Sample s)
        {
            var fn = "ForwardTest.txt".GetMyDocs();
            List<string> l = new List<string>();
            List<double[]> Results = new List<double[]>();
            Results.Add(s.ValInput);
            string st1 = Results.Last()[0].ToString();
            for(int i = 1; i < Results.Last().Count(); i++)
            {
                st1 += ", " + Results.Last()[i];
            }
            l.Add(st1);

            for (int k = 0; k < Network.Layers.Count(); k++)
            {
                Results.Add(Network.Layers[k].Output(Results.Last()));
                string st = Results.Last()[0].ToString();
                for(int i = 1; i < Results.Last().Count(); i++)
                {
                    st += ", " + Results.Last()[i];
                }
                l.Add(st);
            }
            try { File.WriteAllLines(fn, l); } catch (Exception e) {  }

            return Results;
        }
        public double[] Backward
            (Sample s,
            List<double[]> Results,
             NetworkMem mem)
        {
            var DValues = s.DesiredOutput;
            DValues = MeanSquared.Backward(Results.Last(), DValues);

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
            var Samples = s.ReadSamples(8);
            NetworkMem mem = new NetworkMem(Network);

            Parallel.For(0, Samples.Count(), j =>
            {
                var F = Forward(Samples[j]);
                write(CategoricalCrossEntropy.Forward(F.Last(), Samples[j].DesiredOutput).Sum().ToString());

                var DValues = Backward(Samples[j], F, mem);
            });
            mem.Update(Samples.Count(), 1e-4, Network);
            Network.Save();

            s.Save();
        }
        public void Pretrain(Sample s)
        {
            NetworkMem mem = new NetworkMem(Network);
            var F = Forward(s);
            var DValues = Backward(s, F, mem);
            mem.Update(1, 1, Network);
            Network.Save();
        }
    }
}

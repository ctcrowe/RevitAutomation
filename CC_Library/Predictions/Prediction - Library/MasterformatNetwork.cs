using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class MasterformatNetwork
    {
        private const double dropout = 0.1;
        private const double rate = 0.1;
        public static Datatype datatype { get { return Datatype.Masterformat; } }
        
        public static NeuralNetwork GetNetwork(WriteToCMDLine write)
        {
            NeuralNetwork net = datatype.LoadNetwork(write);
            if (net.Datatype == Datatype.None)
            {
                net = new NeuralNetwork(datatype);
                net.Layers.Add(new Layer(300, 400, Activation.LRelu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(300, net.Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
                // net.Layers.Add(new Layer(100, net.Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
                //net.Layers.Add(new Layer(100, net.Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
                //net.Layers.Add(new Layer(100, net.Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
                net.Layers.Add(new Layer(40, net.Layers.Last(), Activation.CombinedCrossEntropySoftmax));
            }
            return net;
        }
        
        public static double[] Predict(string s, WriteToCMDLine write)
        {
            var Alpha = "MasterformatXfmr".LoadAlpha(40, write);
            var output = Alpha.Forward(s);
            output = Activations.SoftMax(output);
            return output;
        }
        public static double[] Propogate
            (string[] Samples, WriteToCMDLine write, bool tf = false)
        {
            var results = new double[2];
            var Alpha = "MasterformatXfmr".LoadAlpha(40, write);
            var AlphaRate = new AttentionChange(Alpha);

            try
            {
                double[] outputs = new double[Samples.Count()];
                double[] desouts = new double[Samples.Count()];
                Parallel.For(0, Samples.Count(), j =>
                {
                    AttentionMem atnmem = new AttentionMem();
                    Alpha.Forward(Samples[j].Split(',').First(), atnmem);
                    var F = Activations.SoftMax(atnmem.attention);
                    F = F.Normalize();
                    
                    outputs[j] = F.ToList().IndexOf(F.Max());
                    desouts[j] = int.Parse(Samples[j].Split(',').Last());

                    var DesiredOutput = new double[40];
                    DesiredOutput[int.Parse(Samples[j].Split(',').Last())] = 1;
                    results[0] += CategoricalCrossEntropy.Forward(F, DesiredOutput).Max();
                    results[1] += F.ToList().IndexOf(F.Max()) == int.Parse(Samples[j].Split(',').Last()) ? 1 : 0;

                    var DValues = Activations.InverseCombinedCrossEntropySoftmax(F, DesiredOutput);
                    Alpha.Backward(atnmem, AlphaRate, DValues);
                });
                outputs.WriteArray("Outputs", write);
                desouts.WriteArray("Desired", write);
            }
            catch (Exception e) { e.OutputError(); }
            //MFMem.Update(Samples.Count(), rate, net);
            Alpha.Update(AlphaRate, Samples.Count(), write);
            results[0] /= Samples.Count();
            results[1] /= Samples.Count();

            write("Run Error : " + results[0]);
            write("Run Accuracy : " + results[1]);
            //net.Save();
            string Folder = "NeuralNets".GetMyDocs();
            Alpha.Save(Folder);
            return results;
        }
    }
}

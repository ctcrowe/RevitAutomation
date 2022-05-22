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
        public static double[] Predict(string s, WriteToCMDLine write)
        {
            var Alpha = "MasterformatXfmr3".LoadXfmr(CharSet.CharCount * 3, 200, 400, write);
            var Alpha2 = "MasterformatXfmr4".LoadXfmr(400, 40, 600, write);
            var _input = s.Locate(1);
            var AOut = Alpha.Forward(_input);
            AOut = Alpha2.Forward(AOut);
            var output = AOut.SumRange();
            output = Activations.SoftMax(output);
            return output;
        }
        public static double[] Propogate
            (string[] Samples, WriteToCMDLine write, bool tf = false)
        {
            var results = new double[2];
            var Alpha = "MasterformatXfmr3".LoadXfmr(CharSet.CharCount * 3, 200, 400, write);
            var Alpha2 = "MasterformatXfmr4".LoadXfmr(400, 40, 600, write);
            var AlphaRate = new AttentionChange(Alpha);
            var AlphaRate2 = new AttentionChange(Alpha2);

            try
            {
                double[] max = new double[Samples.Count()];
                double[] final = new double[Samples.Count()];
                double[] outputs = new double[Samples.Count()];
                double[] desouts = new double[Samples.Count()];
                Parallel.For(0, Samples.Count(), j =>
                {
                    AttentionMem atnmem = new AttentionMem();
                    AttentionMem atnmem2 = new AttentionMem();
                    var _input = Samples[j].Split(',').First().Locate(1);
                    Alpha.Forward(_input, atnmem);
                    Alpha2.Forward(atnmem.attn, atnmem2);
                    var F = Activations.SoftMax(atnmem2.attention);

                    max[j] = F[F.ToList().IndexOf(F.Max())];
                    outputs[j] = F.ToList().IndexOf(F.Max());
                    final[j] = F[int.Parse(Samples[j].Split(',').Last())];
                    desouts[j] = int.Parse(Samples[j].Split(',').Last());

                    var DesiredOutput = new double[40];
                    DesiredOutput[int.Parse(Samples[j].Split(',').Last())] = 1;
                    results[0] += CategoricalCrossEntropy.Forward(F, DesiredOutput).Max();
                    results[1] += F.ToList().IndexOf(F.Max()) == int.Parse(Samples[j].Split(',').Last()) ? 1 : 0;

                    var DValues = Activations.InverseCombinedCrossEntropySoftmax(F, DesiredOutput);
                    var dvals = DValues.Dot(atnmem2.attn.Ones()); //returns a vector [s.Length, size]
                    dvals = Alpha2.Backward(atnmem2, AlphaRate2, dvals);
                    Alpha.Backward(atnmem, AlphaRate, dvals);
                });
                final.WriteArray("Desired Output", write);
                max.WriteArray("Max Output", write);
                outputs.WriteArray("Outputs", write);
                desouts.WriteArray("Desired", write);
            }
            catch (Exception e) { e.OutputError(); }
            //MFMem.Update(Samples.Count(), rate, net);
            Alpha.Update(AlphaRate, write);
            Alpha2.Update(AlphaRate2, write);
            results[0] /= Samples.Count();
            results[1] /= Samples.Count();

            write("Run Error : " + results[0]);
            write("Run Accuracy : " + results[1]);
            //net.Save();
            string Folder = "NeuralNets".GetMyDocs();
            Alpha.Save(Folder);
            Alpha2.Save(Folder);
            return results;
        }
    }
}

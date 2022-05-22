using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class MasterformatNetwork2
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
            var Alpha1 = "MasterformatXfmr5".LoadXfmr(CharSet.CharCount * 3, 100, 200, write);
            var Alpha2 = "MasterformatXfmr6".LoadXfmr(CharSet.CharCount * 3, 100, 100, write);
            var MF = "MasterformatXfmr7".LoadXfmr(200, 40, 100, write);
            var AlphaRate = new AttentionChange(Alpha1);
            var AlphaRate2 = new AttentionChange(Alpha2);
            var MFRate = new AttentionChange(MF);

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
                    AttentionMem mfmem = new AttentionMem();
                    var _input = Samples[j].Split(',').First().Locate(1);
                    Alpha1.Forward(_input, atnmem);
                    Alpha2.Forward(_input, atnmem2);
                    var input = atnmem.attn.Merge(atnmem2.attn);
                    MF.Forward(input, mfmem);

                    var F = Activations.SoftMax(mfmem.attention);

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
                    dvals = MF.Backward(mfmem, MFRate, dvals);
                    Alpha1.Backward(atnmem, AlphaRate, dvals);
                });
                final.WriteArray("Desired Output", write);
                max.WriteArray("Max Output", write);
                outputs.WriteArray("Outputs", write);
                desouts.WriteArray("Desired", write);
            }
            catch (Exception e) { e.OutputError(); }
            //MFMem.Update(Samples.Count(), rate, net);
            Alpha1.Update(AlphaRate, write);
            Alpha2.Update(AlphaRate2, write);
            results[0] /= Samples.Count();
            results[1] /= Samples.Count();

            write("Run Error : " + results[0]);
            write("Run Accuracy : " + results[1]);
            //net.Save();
            string Folder = "NeuralNets".GetMyDocs();
            Alpha1.Save(Folder);
            Alpha2.Save(Folder);
            return results;
        }
    }
}

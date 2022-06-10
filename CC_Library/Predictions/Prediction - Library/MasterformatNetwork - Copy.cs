using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class MasterformatNetwork2
    {
        public static double[] Predict(string s, WriteToCMDLine write)
        {
            var Alpha = new Alpha2(write);
            var MF = "Masterformat".LoadXfmr(Alpha2._Outputs, 40, 80, write);

            var AOut = Alpha.Forward(s, write);
            var MFOut = MF.Forward(AOut);
            var output = MFOut.SumRange();
            output = Activations.SoftMax(output);
            return output;
        }
        public static double[] Propogate
            (string[] Samples, WriteToCMDLine write, bool tf = false)
        {
            var results = new double[2];
            var Alpha = new Alpha2(write);
            var Rates = Alpha.GetChange();

            var MF = "Masterformat".LoadXfmr(Alpha2._Outputs, 40, 80, write);
            var MFRate = new AttentionChange(MF);

            try
            {
                double[] max = new double[Samples.Count()];
                double[] final = new double[Samples.Count()];
                double[] outputs = new double[Samples.Count()];
                double[] desouts = new double[Samples.Count()];

                Parallel.For(0, Samples.Count(), j =>
                {
                    var AlphaMem = Alpha.GetMem();
                    var MFMem = new AttentionMem();

                    var AlphaOut = Alpha.Forward(Samples[j].Split(',').First(), AlphaMem, write);
                    MF.Forward(AlphaOut, MFMem);
                    var attention = MFMem.attn.SumRange();
                    var F = Activations.SoftMax(attention);

                    max[j] = F[F.ToList().IndexOf(F.Max())];
                    outputs[j] = F.ToList().IndexOf(F.Max());
                    final[j] = F[int.Parse(Samples[j].Split(',').Last())];
                    desouts[j] = int.Parse(Samples[j].Split(',').Last());

                    var DesiredOutput = new double[40];
                    DesiredOutput[int.Parse(Samples[j].Split(',').Last())] = 1;
                    results[0] += CategoricalCrossEntropy.Forward(F, DesiredOutput).Max();
                    results[1] += F.ToList().IndexOf(F.Max()) == int.Parse(Samples[j].Split(',').Last()) ? 1 : 0;

                    var DValues = Activations.InverseCombinedCrossEntropySoftmax(DesiredOutput, F);
                    var dvals = DValues.Dot(MFMem.attn.Ones()); //returns a vector [s.Length, size]
                    dvals = MF.Backward(MFMem, MFRate, dvals);
                    Alpha.Backward(dvals, AlphaMem, Rates, write);
                });
                final.WriteArray("Desired Output", write);
                max.WriteArray("Max Output", write);
                outputs.WriteArray("Outputs", write);
                desouts.WriteArray("Desired", write);
            }
            catch (Exception e) { e.OutputError(); }
            //MFMem.Update(Samples.Count(), rate, net);
            Alpha.Update(Rates, new int[4] {0,1,2,3}, write);
            MF.Update(MFRate, write);

            results[0] /= Samples.Count();
            results[1] /= Samples.Count();

            write("Run Error : " + results[0]);
            write("Run Accuracy : " + results[1]);
            //net.Save();
            string Folder = "NeuralNets".GetMyDocs();
            MF.Save(Folder);
            return results;
        }
    }
}

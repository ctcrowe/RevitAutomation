using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class ObjStyleCaseworkNetwork
    {
        public static double[] Predict(string s, WriteToCMDLine write)
        {
            var Alpha = new Alpha(write);
            var Obj = "ObjStyle_Casework".LoadXfmr(Alpha._Outputs, 9, 80, write);

            var AOut = Alpha.Forward(s, write);
            var ObjOut = Obj.Forward(AOut);
            var output = ObjOut.SumRange();
            output = Activations.SoftMax(output);
            return output;
        }
        public static double[] Propogate
            (string[] Samples, WriteToCMDLine write, bool tf = false)
        {
            var results = new double[2];
            var Alpha = new Alpha(write);
            var Rates = Alpha.GetChange();

            var Obj = "ObjStyle_Casework".LoadXfmr(Alpha._Outputs, 9, 80, write);
            var ObjRate = new AttentionChange(Obj);

            try
            {
                double[] max = new double[Samples.Count()];
                double[] final = new double[Samples.Count()];
                double[] outputs = new double[Samples.Count()];
                double[] desouts = new double[Samples.Count()];

                Parallel.For(0, Samples.Count(), j =>
                {
                    var AlphaMem = Alpha.GetMem();
                    var ObjMem = new AttentionMem();

                    var AlphaOut = Alpha.Forward(Samples[j].Split(',').First(), AlphaMem, write);
                    Obj.Forward(AlphaOut, ObjMem);

                    var F = Activations.SoftMax(ObjMem.attention);

                    max[j] = F[F.ToList().IndexOf(F.Max())];
                    outputs[j] = F.ToList().IndexOf(F.Max());
                    final[j] = F[int.Parse(Samples[j].Split(',').Last())];
                    desouts[j] = int.Parse(Samples[j].Split(',').Last());

                    var DesiredOutput = new double[9];
                    DesiredOutput[int.Parse(Samples[j].Split(',').Last())] = 1;
                    results[0] += CategoricalCrossEntropy.Forward(F, DesiredOutput).Max();
                    results[1] += F.ToList().IndexOf(F.Max()) == int.Parse(Samples[j].Split(',').Last()) ? 1 : 0;

                    var DValues = Activations.InverseCombinedCrossEntropySoftmax(DesiredOutput, F);
                    var dvals = DValues.Dot(ObjMem.attn.Ones()); //returns a vector [s.Length, size]
                    dvals = Obj.Backward(ObjMem, ObjRate, dvals);
                    Alpha.Backward(dvals, AlphaMem, Rates, write);
                });
                final.WriteArray("Desired Output", write);
                max.WriteArray("Max Output", write);
                outputs.WriteArray("Outputs", write);
                desouts.WriteArray("Desired", write);
            }
            catch (Exception e) { e.OutputError(); }

            //Alpha.Update(Rates, write);
            Obj.Update(ObjRate, write);

            results[0] /= Samples.Count();
            results[1] /= Samples.Count();

            write("Run Error : " + results[0]);
            write("Run Accuracy : " + results[1]);

            string Folder = "NeuralNets".GetMyDocs();
            Obj.Save(Folder);
            return results;
        }
    }
}
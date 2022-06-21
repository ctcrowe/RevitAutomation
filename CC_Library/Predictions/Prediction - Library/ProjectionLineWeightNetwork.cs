using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class ProjectionLineWeightNetwork
    {
        private const int count = 16;
        public static double[] Predict(string s, WriteToCMDLine write, double[,] VInput = null)
        {
            string Name = typeof(ProjectionLineWeightNetwork).Name;
            var Alpha = new Alpha(Name, write);
            var Obj = Name.LoadXfmr(Alpha._Outputs, count, 120, write);
            var AOut = Alpha.Forward(s);
            if (VInput != null)
                AOut = AOut.Append(VInput);

            var ObjOut = Obj.Forward(AOut);
            var output = ObjOut.SumRange();
            output = Activations.SoftMax(output);
            return output;
        }
        public static double[] Propogate
            (string[] Samples, WriteToCMDLine write, bool tf = false)
        {
            var results = new double[2];
            string Name = typeof(ProjectionLineWeightNetwork).Name;
            var Alpha = new Alpha(Name, write);
            var A2 = new Alpha("ViewName", write);
            var Rates = Alpha.GetChange();
            var A2Rates = A2.GetChange();
            var Obj = Name.LoadXfmr(Alpha._Outputs, count, 120, write);
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
                    var A2Mem = A2.GetMem();
                    var ObjMem = new AttentionMem();
                    double[,] AOut = Alpha.Forward(Samples[j].Split(',')[1], AlphaMem, write);

                    if (Samples[j].Split(',').Length == 4)
                    {
                        var A2Out = A2.Forward(Samples[j].Split(',')[2], A2Mem, write);
                        AOut.Append(A2Out);
                    }

                    Obj.Forward(AOut, ObjMem);

                    var attention = ObjMem.attn.SumRange();
                    var F = Activations.SoftMax(attention);

                    max[j] = F[F.ToList().IndexOf(F.Max())];
                    outputs[j] = F.ToList().IndexOf(F.Max());

                    int target =
                        int.TryParse(Samples[j].Split(',').Last(), out int integer) ?
                        int.Parse(Samples[j].Split(',').Last()) : 0;

                    final[j] = F[target];
                    desouts[j] = target;

                    var DesiredOutput = new double[count];
                    DesiredOutput[target] = 1;
                    results[0] += CategoricalCrossEntropy.Forward(F, DesiredOutput).Max();
                    results[1] += F.ToList().IndexOf(F.Max()) == target ? 1 : 0;

                    var DValues = Activations.InverseCombinedCrossEntropySoftmax(DesiredOutput, F);
                    var dvals = DValues.Dot(ObjMem.attn.Ones()); //returns a vector [s1.Length + s2.Length, size]
                    dvals = Obj.Backward(ObjMem, ObjRate, dvals);

                    if(Samples[j].Split(',').Length == 4)
                    {
                        var A2Dvals = dvals.Take(Samples[j].Split(',')[1].Length, Samples[j].Split(',')[1].Length);
                        A2.Backward(A2Dvals, A2Mem, A2Rates, write);
                        dvals = dvals.Take(0, Samples[j].Split(',')[1].Length);
                    }
                    Alpha.Backward(dvals, AlphaMem, Rates, write);
                });
                final.WriteArray("Desired Output", write);
                max.WriteArray("Max Output", write);
                outputs.WriteArray("Outputs", write);
                desouts.WriteArray("Desired", write);
            }
            catch (Exception e) { e.OutputError(); }

            Alpha.Update(Rates, write);
            A2.Update(A2Rates, write);
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

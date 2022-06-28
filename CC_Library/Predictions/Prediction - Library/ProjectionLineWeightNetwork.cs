using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public class Transformers
    {
        public static Transformer ProjectionLineWeightTransformer(WriteToCMDLine write)
        { return "ProjectionLineWeightNetwork".LoadXfmr(Alpha._Outputs, 16, 120, write); }
        public static Transformer CutLineWeightTransformer(WriteToCMDLine write)
        { return "CutLineWeightTransformer".LoadXfmr(Alpha._Outputs, 16, 120, write); }
        public static Transformer ProjectionLineWeightAlpha1(WriteToCMDLine write)
        { return "ProjectionLineWeightNetworkAlpha".LoadXfmr(CharSet.CharCount * 3, Alpha._Outputs, 200, write); }
        public static Transformer ViewNameAlpha(WriteToCMDLine write)
        { return "ViewNameAlpha".LoadXfmr(CharSet.CharCount * 3, Alpha._Outputs, 200, write); }
        public static Transformer Masterformat(WriteToCMDLine write)
        { return "Masterformat".LoadXfmr(Alpha._Outputs, 40, 80, write); }
        public static Transformer FamilyName(WriteToCMDLine write)
        { return "FamilyName".LoadXfmr(CharSet.CharCount * 3, Alpha._Outputs, 200, write); }
    }
    public static class LineWeightNetwork
    {
        private const int count = 16;
        public static double[] Predict(
            string s,
            WriteToCMDLine write,
            Transformer Xfmr,
            Transformer AlphaXfmr1,
            Transformer AlphaXfmr2,
            string vname = null)
        {
            //string Name = typeof(ProjectionLineWeightNetwork).Name;
            //var Alpha = new Alpha(Name, write);
            //var Obj = Name.LoadXfmr(Alpha._Outputs, count, 120, write);
            var AOut = AlphaXfmr1.Forward(s.Locate(1));
            if (vname != null)
                AOut = AOut.Append(AlphaXfmr2.Forward(vname.Locate(1)));

            var ObjOut = Xfmr.Forward(AOut);
            var output = ObjOut.SumRange();
            output = Activations.SoftMax(output);
            return output;
        }
        public static double[] Propogate
            (string[] Samples,
             WriteToCMDLine write,
             Transformer Xfmr,
             Transformer AlphaXfmr1,
             Transformer AlphaXfmr2)
        {
            var results = new double[2];
            
            var A1Rates = new AttentionChange(AlphaXfmr1);
            var A2Rates = new AttentionChange(AlphaXfmr2);
            var ObjRate = new AttentionChange(Xfmr);

            try
            {
                double[] max = new double[Samples.Count()];
                double[] final = new double[Samples.Count()];
                double[] outputs = new double[Samples.Count()];
                double[] desouts = new double[Samples.Count()];

                Parallel.For(0, Samples.Count(), j =>
                {
                    var A1Mem = new AttentionMem();
                    var A2Mem = new AttentionMem();
                    var ObjMem = new AttentionMem();
                    double[,] AOut = AlphaXfmr1.Forward(Samples[j].Split(',')[1].Locate(1), A1Mem);

                    if (Samples[j].Split(',').Count() > 3)
                    {
                        var A2Out = AlphaXfmr2.Forward(Samples[j].Split(',')[2].Locate(1), A2Mem);
                        AOut = AOut.Append(A2Out);
                    }

                    var output = Xfmr.Forward(AOut, ObjMem);

                    var attention = ObjMem.attn.SumRange();
                    var F = Activations.SoftMax(attention);

                    max[j] = F[F.ToList().IndexOf(F.Max())];
                    outputs[j] = F.ToList().IndexOf(F.Max());

                    int target =
                        int.TryParse(Samples[j].Split(',').Last(), out int integer) ?
                        int.Parse(Samples[j].Split(',').Last()) - 1 : -1;

                    final[j] = F[target];
                    desouts[j] = target;

                    var DesiredOutput = new double[count];
                    DesiredOutput[target] = 1;
                    results[0] += CategoricalCrossEntropy.Forward(F, DesiredOutput).Max();
                    results[1] += F.ToList().IndexOf(F.Max()) == target ? 1 : 0;

                    var DValues = Activations.InverseCombinedCrossEntropySoftmax(DesiredOutput, F);
                    var dvals = DValues.Dot(ObjMem.attn.Ones()); //returns a vector [s1.Length + s2.Length, size]
                    dvals = Xfmr.Backward(ObjMem, ObjRate, dvals);

                    if(Samples[j].Split(',').Length == 4)
                    {
                        var A2Dvals = dvals.Take(Samples[j].Split(',')[1].Count(), Samples[j].Split(',')[2].Count());
                        AlphaXfmr2.Backward(A2Mem, A2Rates, A2Dvals);
                        dvals = dvals.Take(0, Samples[j].Split(',')[1].Length);
                    }
                    AlphaXfmr1.Backward(A1Mem, A1Rates, dvals);
                });
                for(int i = 0; i < Samples.Count(); i++)
                {
                    write(Samples[i] + " - Desired : " + final[i] + ", " +  desouts[i] + " - Max " + max[i] + ", " + outputs[i]);
                }
            }
            catch (Exception e) {
                Samples.WriteArray("Failed At this Set", write);
                e.OutputError(); }

            AlphaXfmr1.Update(A1Rates, write);
            AlphaXfmr2.Update(A2Rates, write);
            Xfmr.Update(ObjRate, write);

            results[0] /= Samples.Count();
            results[1] /= Samples.Count();

            write("Run Error : " + results[0]);
            write("Run Accuracy : " + results[1]);

            string Folder = "NeuralNets".GetMyDocs();
            Xfmr.Save(Folder);
            AlphaXfmr1.Save(Folder);
            AlphaXfmr2.Save(Folder);
            return results;
        }
    }
}

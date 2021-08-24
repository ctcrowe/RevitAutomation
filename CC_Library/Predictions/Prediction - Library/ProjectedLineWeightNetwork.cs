using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    class ProjectedLineWeightNetwork : INetworkPredUpdater
    {
        public Datatype datatype { get { return Datatype.ProjectedLineWeight; } }
        public NeuralNetwork Network { get; }

        public ProjectedLineWeightNetwork()
        {
            Network = Datatype.ProjectedLineWeight.LoadNetwork(new WriteToCMDLine(WriteNull));
        }
        public double[] Predict(Sample s)
        {
            Alpha a = new Alpha(new WriteToCMDLine(WriteNull));
            AlphaContext ctxt = new AlphaContext(datatype, new WriteToCMDLine(WriteNull));
            var input = a.Forward(s.TextInput, ctxt, new WriteToCMDLine(WriteNull)).ToList();
            input.AddRange(a.Forward(s.SecondaryText, ctxt, new WriteToCMDLine(WriteNull)));
            input.AddRange(s.ValInput);
            var Results = input.ToArray();

            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                Results = Network.Layers[i].Output(Results);
            }

            return Results;
        }
        public List<double[]> Forward(Sample s, WriteToCMDLine write)
        {
            var input = s.TextOutput.ToList();
            input.AddRange(s.SecondaryTextOutput);
            input.AddRange(s.ValInput);

            List<double[]> Results = new List<double[]>();
            Results.Add(input.ToArray());

            for (int k = 0; k < Network.Layers.Count(); k++)
            {
                Results.Add(Network.Layers[k].Output(Results.Last()));
            }

            return Results;
        }
        public double[] Backward
            (Sample s,
            List<double[]> Results,
             NetworkMem mem,
             WriteToCMDLine Write)
        {
            var DValues = s.DesiredOutput;

            for (int l = Network.Layers.Count() - 1; l >= 0; l--)
            {
                DValues = mem.Layers[l].DActivation(DValues, Results[l + 1]);
                mem.Layers[l].DBiases(DValues);
                mem.Layers[l].DWeights(DValues, Results[l]);
                DValues = mem.Layers[l].DInputs(DValues, Network.Layers[l]);
            }
            return DValues.ToList().Take(2 * Alpha.DictSize).ToArray();
        }
        public void Propogate
            (Sample s, WriteToCMDLine write)
        {
            var check = Predict(s);
            if (s.DesiredOutput.ToList().IndexOf(s.DesiredOutput.Max()) != check.ToList().IndexOf(check.Max()))
            {
                Alpha a = new Alpha(new WriteToCMDLine(WriteNull));
                AlphaContext ctxt1 = new AlphaContext(datatype, new WriteToCMDLine(WriteNull));
                AlphaContext ctxt2 = new AlphaContext(datatype, new WriteToCMDLine(WriteNull), 1);
                var Samples = s.ReadSamples();
                List<string> lines = new List<string>();

                NetworkMem ObjMem = new NetworkMem(Network);
                NetworkMem AlphaMem = new NetworkMem(a.Network);
                NetworkMem CtxtMem1 = new NetworkMem(ctxt1.Network);
                NetworkMem CtxtMem2 = new NetworkMem(ctxt2.Network);

                Parallel.For(0, Samples.Count(), j =>
                {
                    AlphaMem am = new AlphaMem(Samples[j].TextInput.ToCharArray());
                    Samples[j].TextOutput = a.Forward(Samples[j].TextInput, ctxt1, am, write);
                    AlphaMem am2 = new AlphaMem(Samples[j].SecondaryText.ToCharArray());
                    Samples[j].SecondaryTextOutput = a.Forward(Samples[j].SecondaryText, ctxt2, am2, write);
                    var F = Forward(Samples[j], write);
                    lines.AddRange(Samples[j].OutputError(CategoricalCrossEntropy.Forward(F.Last(), Samples[j].DesiredOutput)));

                    var DValues = Backward(Samples[j], F, ObjMem, WriteNull);
                    var DV1 = DValues.ToList().Take(Alpha.DictSize).ToArray();
                    var DV2 = Enumerable.Reverse(DValues).Take(Alpha.DictSize).Reverse().ToArray();
                    a.Backward(Samples[j].TextInput, DV1, ctxt1, am, AlphaMem, CtxtMem1, write);
                    a.Backward(Samples[j].SecondaryText, DV2, ctxt2, am2, AlphaMem, CtxtMem2, write);
                });
                ObjMem.Update(1, 0.0001, Network);
                AlphaMem.Update(1, 0.00001, a.Network);
                CtxtMem1.Update(1, 0.0001, ctxt1.Network);
                CtxtMem2.Update(1, 0.0001, ctxt2.Network);

                lines.ShowErrorOutput();
                Network.Save();
                a.Network.Save();
                ctxt1.Save();
                ctxt2.Save();

                s.Save();
            }
        }
        private static string WriteNull(string s) { return s; }
    }
}

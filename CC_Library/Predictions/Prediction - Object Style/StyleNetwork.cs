using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public class ObjectStyleNetwork : INetworkPredUpdater
    {
        public Datatype datatype { get { return Datatype.ObjectStyle; } }
        public NeuralNetwork Network { get; }
        public ObjectStyleNetwork()
        {
            Network = Datatype.ObjectStyle.LoadNetwork(new WriteToCMDLine(WriteNull));
        }
        public double[] Predict(Sample s)
        {
            Alpha a = new Alpha(new WriteToCMDLine(WriteNull));
            AlphaContext ctxt = new AlphaContext(datatype, new WriteToCMDLine(WriteNull));
            double[] Results = a.Forward(s.TextInput, ctxt, new WriteToCMDLine(WriteNull));
            for(int i = 0; i < Network.Layers.Count(); i++)
            {
                Results = Network.Layers[i].Output(Results);
            }
            return Results;
        }
        public List<double[]> Forward(Sample s, WriteToCMDLine write)
        {
            var input = s.TextOutput.ToList();
            input.AddRange(s.ValInput);

            List<double[]> Results = new List<double[]>();
            Results.Add(input.ToArray()) ;

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
            return DValues.ToList().Take(Alpha.DictSize).ToArray();
        }
        public void Propogate
            (Sample s, WriteToCMDLine write)
        {
            var check = Predict(s);
            if(s.DesiredOutput.ToList().IndexOf(s.DesiredOutput.Max()) != check.ToList().IndexOf(check.Max()))
            {
                Alpha a = new Alpha(new WriteToCMDLine(WriteNull));
                AlphaContext ctxt = new AlphaContext(datatype, new WriteToCMDLine(WriteNull));
                var Samples = s.ReadSamples();
                List<string> lines;
                for(int i = 0; i < 5; i++)
                {
                    NetworkMem ObjMem = new NetworkMem(Network);
                    NetworkMem AlphaMem = new NetworkMem(a.Network);
                    NetworkMem CtxtMem = new NetworkMem(ctxt.Network);
                    
                    Parallel.For(0, Samples.Count(), j =>
                    {
                        AlphaMem am = new AlphaMem(Samples[j].TextInput.ToCharArray());
                        Samples[j].TextOutput = a.Forward(Samples[j].TextInput, ctxt, am, write);
                        var F = Forward(Samples[j], write);
                        lines.AddRange(Samples[j].OutputError(CategoricalCrossEntropy.Forward(F.Last(), Samples[j].DesiredOutput)));
                    
                        var DValues = Backward(Samples[j], F, ObjMem, WriteNull);
                        a.Backward(Samples[j].TextInput, DValues, ctxt, am, AlphaMem, CtxtMem, write);
                    });
                    ObjMem.Update(1, 0.0001, Network);
                    AlphaMem.Update(1, 0.00001, a.Network);
                    CtxtMem.Update(1, 0.0001, ctxt.Network);
                }
                lines.ShowErrorOutput();
                Network.Save();
                a.Network.Save();
                ctxt.Save();
                
                s.Save();
            }
        }
        private static string WriteNull(string s) { return s; }
    }
}

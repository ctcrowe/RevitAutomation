using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;
using System.Runtime.InteropServices;

/// <summary>
/// This system is specifically for creating and using a vocabulary.
/// For this reason, it doesnt need all of the delegates that a typical network would.
/// </summary>

namespace CC_Library.Predictions
{
    public class MasterformatNetwork : INetworkPredUpdater
    {
        public Datatype datatype { get { return Datatype.Masterformat; } }
        public NeuralNetwork Network { get; }
        public MasterformatNetwork()
        {
            Network = Datatype.Masterformat.LoadNetwork(new WriteToCMDLine(WriteNull));
        }
        public MasterformatNetwork(WriteToCMDLine write)
        {
            Network = Datatype.Masterformat.LoadNetwork(write);
        }
        public double[] Predict(Sample s)
        {
            Alpha a = new Alpha(new WriteToCMDLine(WriteNull));
            AlphaContext ctxt = new AlphaContext(Datatype.Masterformat, new WriteToCMDLine(WriteNull));
            double[] Results = a.Forward(s.TextInput, ctxt, new WriteToCMDLine(WriteNull));
            for(int i = 0; i < Network.Layers.Count(); i++)
            {
                Results = Network.Layers[i].Output(Results);
            }
            return Results;
        }
        public List<double[]> Forward(Sample s, WriteToCMDLine write)
        {
            List<double[]> Results = new List<double[]>();
            Results.Add(s.TextOutput);

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
                AlphaContext ctxt = new AlphaContext(Datatype.Masterformat, new WriteToCMDLine(WriteNull));
                List<string> lines = new List<string>();
                
                for(int i = 0; i < 5; i++)
                {
                    var Samples = s.ReadSamples(24);
                    Accuracy Acc = new Accuracy(Samples);
                    NetworkMem MFMem = new NetworkMem(Network);
                    NetworkMem AlphaMem = new NetworkMem(a.Network);
                    NetworkMem CtxtMem = new NetworkMem(ctxt.Network);
                    
                    Parallel.For(0, Samples.Count(), j =>
                    {
                        AlphaMem am = new AlphaMem(Samples[j].TextInput.ToCharArray());
                        Samples[j].TextOutput = a.Forward(Samples[j].TextInput, ctxt, am, write);
                        var F = Forward(Samples[j], write);
                        Acc.Add( j,
                            CategoricalCrossEntropy.Forward(F.Last()),
                            F.Last().ToList().IndexOf(F.Last.Max()),
                            Samples[j].DesiredOutput.ToList().IndexOf(DesiredOutput.Max()));
                    
                        var DValues = Backward(Samples[j], F, MFMem, WriteNull);
                        a.Backward(Samples[j].TextInput, DValues, ctxt, am, AlphaMem, CtxtMem, write);
                    });
                    lines.AddRange(Acc.Get());
                    MFMem.Update(1, 0.0001, Network);
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

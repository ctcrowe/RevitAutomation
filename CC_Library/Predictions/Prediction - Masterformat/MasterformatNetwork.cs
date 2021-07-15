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
//https://stackoverflow.com/questions/26733/getting-all-types-that-implement-an-interface
//var results = from type in someAssembly.GetTypes()
//              where typeof(IFoo).IsAssignableFrom(type)
//              select type;

namespace CC_Library.Predictions
{
    public class MasterformatNetwork : INetworkPredUpdater
    {
        public Datatype datatype = Datatype.Masterformat;
        public NeuralNetwork Network { get; }
        public MasterformatNetwork(WriteToCMDLine write)
        {
            Network = Datatype.Masterformat.LoadNetwork(write);
        }
        public double[] Predict(Sample s = new Sample(Datatype.None))
        {
            Alpha a = new Alpha(new WriteToCMDLine(WriteNull));
            AlphaContext ctxt = new AlphaContext(Datatype.Masterformat, new WriteToCMDLine(WriteNull));
            double[] Results = a.Forward(Input.TextInput, ctxt, am, write);
            for(int i = 0; i < mf.Network.Layers.Count(); i++)
            {
                Results = Network.Layers[i].Output(Results);
            }
            return Results;
        }
        public List<double[]> Forward(WriteToCMDLine write)
        {
            List<double[]> Results = new List<double[]>();
            Results.Add(Input.TextOutput);

            for (int k = 0; k < Network.Layers.Count(); k++)
            {
                Results.Add(Network.Layers[k].Output(Results.Last()));
            }

            return Results;
        }
        public double[] Backward
            (List<double[]> Results,
             NetworkMem mem,
             WriteToCMDLine Write)
        {
            var DValues = Input.DesiredOutput;

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
            (WriteToCMDLine write)
        {
            var check = Predict();
            if(Input.DesiredOutput.ToList().IndexOf(Input.DesiredOutput.Max()) != check.ToList().IndexOf(check.Max())
            {
                var Samples = datatype.ReadSamples();
                Samples[0] = Input;
                for(int i = 0; i < 5; i++)
                {
                    NetworkMem MFMem = new NetworkMem(net.Network);
                    NetworkMem AlphaMem = new NetworkMem(a.Network);
                    NetworkMem CtxtMem = new NetworkMem(ctxt.Network);
                    
                    Parallel.For(0, Samples.Count(), j =>
                    {
                        AlphaMem am = new AlphaMem(Input.TextInput.ToCharArray());
                        Input.TextOutput = a.Forward(Input.TextInput, ctxt, am, write);
                        var F = Forward(write);
                    
                        var DValues = net.Backward(F, MFMem, WriteNull);
                        a.Backward(Input.TextInput, DValues, ctxt, am, AlphaMem, CtxtMem, write);
                    });
                    MFMem.Update(1, 0.0001, net.Network);
                    AlphaMem.Update(1, 0.00001, a.Network);
                    CtxtMem.Update(1, 0.0001, ctxt.Network);
                }
                net.Network.Save();
                a.Network.Save();
                ctxt.Save();
            }
            Input.Save();
        }
        private static string WriteNull(string s) { return s; }
    }
}

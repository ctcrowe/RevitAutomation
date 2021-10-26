using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public class OLFNetwork : INetworkPredUpdater
    {
        public Datatype datatype { get { return Datatype.OccupantLoadFactor; } }
        public NeuralNetwork Network { get; }
        public OLFNetwork(Sample s)
        {
            Network = Datatype.OccupantLoadFactor.LoadNetwork();
        }
        public double[] Predict(Sample s)
        {
            Alpha a = new Alpha();
            AlphaContext ctxt = new AlphaContext(datatype);
            double[] Results = a.Forward(s.TextInput, ctxt);
            for(int i = 0; i < Network.Layers.Count(); i++)
            {
                Results = Network.Layers[i].Output(Results);
            }
            return Results;
        }
        public List<double[]> Forward(Sample s)
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
             NetworkMem mem)
        {
            var DValues = s.DesiredOutput;

            for (int l = Network.Layers.Count() - 1; l >= 0; l--)
            {
                DValues = mem.Layers[l].DActivation(DValues, Results[l + 1]);
                mem.Layers[l].DBiases(DValues, Network.Layers[l]);
                mem.Layers[l].DWeights(DValues, Results[l], Network.Layers[l]);
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
                Alpha a = new Alpha();
                AlphaContext ctxt = new AlphaContext(datatype);
                var Samples = s.ReadSamples();
                for(int i = 0; i < 5; i++)
                {
                    NetworkMem ObjMem = new NetworkMem(Network);
                    NetworkMem AlphaMem = new NetworkMem(a.Network);
                    NetworkMem CtxtMem = new NetworkMem(ctxt.Network);
                    
                    Parallel.For(0, Samples.Count(), j =>
                    {
                        AlphaMem am = new AlphaMem(s.TextInput.ToCharArray());
                        s.TextOutput = a.Forward(s.TextInput, ctxt, am);
                        var F = Forward(s);
                    
                        var DValues = Backward(s, F, ObjMem);
                        a.Backward(s.TextInput, DValues, ctxt, am, AlphaMem, CtxtMem);
                    });
                    ObjMem.Update(1, 0.0001, Network);
                    AlphaMem.Update(1, 0.00001, a.Network);
                    CtxtMem.Update(1, 0.0001, ctxt.Network);
                }
                Network.Save();
                a.Network.Save();
                ctxt.Save();
                
                s.Save();
            }
        }
    }
}

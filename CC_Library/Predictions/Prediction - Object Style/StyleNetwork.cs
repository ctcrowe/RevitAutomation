using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public class ObjectStyleNetwork : INetworkPredUpdater
    {
        public Datatype datatype { get { return Datatype.ObjectStyle; } }
        public NeuralNetwork Network { get; }
        public ObjectStyleNetwork(WriteToCMDLine write)
        {
            Network = Datatype.ObjectStyle.LoadNetwork(write);
        }/*
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
                var Samples = datatype.ReadSamples();
                Samples[0] = s;
                for(int i = 0; i < 5; i++)
                {
                    NetworkMem MFMem = new NetworkMem(Network);
                    NetworkMem AlphaMem = new NetworkMem(a.Network);
                    NetworkMem CtxtMem = new NetworkMem(ctxt.Network);
                    
                    Parallel.For(0, Samples.Count(), j =>
                    {
                        AlphaMem am = new AlphaMem(s.TextInput.ToCharArray());
                        s.TextOutput = a.Forward(s.TextInput, ctxt, am, write);
                        var F = Forward(s, write);
                    
                        var DValues = Backward(s, F, MFMem, WriteNull);
                        a.Backward(s.TextInput, DValues, ctxt, am, AlphaMem, CtxtMem, write);
                    });
                    MFMem.Update(1, 0.0001, Network);
                    AlphaMem.Update(1, 0.00001, a.Network);
                    CtxtMem.Update(1, 0.0001, ctxt.Network);
                }
                Network.Save();
                a.Network.Save();
                ctxt.Save();
                
                s.Save();
            }
        }
        private static string WriteNull(string s) { return s; }*/
        public static int Predict(string s, double[] vals)
        {
            ObjectStyleNetwork net = new ObjectStyleNetwork(new WriteToCMDLine(WriteNull));
            double[] Results = Alpha.Predict(Datatype.ObjectStyle, s);
            var input = Results.ToList();
            input.AddRange(vals);
            Results = input.ToArray();
            for (int i = 0; i < net.Network.Layers.Count(); i++)
            {
                Results = net.Network.Layers[i].Output(Results);
            }
            return Results.ToList().IndexOf(Results.Max());
        }
        internal static KeyValuePair<double, List<double[]>> Forward
            (string Name,
             double[] Numbers,
             int correct,
             ObjectStyleNetwork net,
             Alpha a,
             AlphaContext ctxt,
             AlphaMem am,
             WriteToCMDLine write)
        {
            List<double[]> Results = new List<double[]>();
            var input = a.Forward(Name, ctxt, am, write).ToList();
            input.AddRange(Numbers);
            Results.Add(input.ToArray());

            for (int k = 0; k < net.Network.Layers.Count(); k++)
            {
                Results.Add(net.Network.Layers[k].Output(Results.Last()));
            }

            int choice = Results.Last().ToList().IndexOf(Results.Last().Max());
            double[] res = new double[net.Network.Layers.Last().Biases.Count()];
            res[correct] = 1;
            
            var result = CategoricalCrossEntropy.Forward(Results.Last(), res);
            double error = result.Sum();
            return new KeyValuePair<double, List<double[]>> (error, Results);
        }
        internal static void Backward
            (string Name,
             List<double[]> Results,
             int correct,
             ObjectStyleNetwork net,
             Alpha a,
             AlphaContext ctxt,
             AlphaMem am,
             NetworkMem ObjMem,
             NetworkMem AlphaMem,
             NetworkMem CtxtMem,
             WriteToCMDLine write)
        {
            double[] res = new double[net.Network.Layers.Last().Biases.Count()];
            res[correct] = 1;
            var DValues = res;

            for (int l = net.Network.Layers.Count() - 1; l >= 0; l--)
            {
                DValues = ObjMem.Layers[l].DActivation(DValues, Results[l + 1]);
                ObjMem.Layers[l].DBiases(DValues);
                ObjMem.Layers[l].DWeights(DValues, Results[l]);
                DValues = ObjMem.Layers[l].DInputs(DValues, net.Network.Layers[l]);
            }
            DValues = DValues.ToList().Take(Alpha.DictSize).ToArray();
            a.Backward(Name, DValues, ctxt, am, AlphaMem, CtxtMem, write);
        }
        public static void SinglePropogate
            (string Name, double[] Numbers, int correct, WriteToCMDLine write)
        {
            double error = double.MaxValue;
            int Prediction = -1;
            ObjectStyleNetwork net = new ObjectStyleNetwork(WriteNull);
            Alpha a = new Alpha(WriteNull);
            AlphaContext ctxt = new AlphaContext(Datatype.ObjectStyle, WriteNull);
            
            while(true)
            {
                AlphaMem am = new AlphaMem(Name.ToCharArray());
                var F = Forward(Name, Numbers, correct, net, a, ctxt, am, WriteNull);
                Prediction = F.Value.Last().ToList().IndexOf(F.Value.Last().Max());
                if(Prediction == correct)
                    break;

                error = F.Key;
                //write("Prediction : " + Prediction + " : Actual : " + correct + " : Error : " + error.ToString());
                
                NetworkMem OBJMem = new NetworkMem(net.Network);
                NetworkMem AlphaMem = new NetworkMem(a.Network);
                NetworkMem CtxtMem = new NetworkMem(ctxt.Network);
                
                Backward(Name, F.Value, correct, net, a, ctxt, am, OBJMem, AlphaMem, CtxtMem, WriteNull);
                OBJMem.Update(1, 0.0001, net.Network);
                AlphaMem.Update(1, 0.00001, a.Network);
                CtxtMem.Update(1, 0.0001, ctxt.Network);

            }

            net.Network.Save();
            a.Network.Save();
            ctxt.Save();
        }
        private static string WriteNull(string s) { return s; }
    }
}

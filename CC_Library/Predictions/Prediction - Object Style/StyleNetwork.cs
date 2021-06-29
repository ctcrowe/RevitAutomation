using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CC_Library.Datatypes;
using System.Runtime.InteropServices;

namespace CC_Library.Predictions
{
    public class ObjectStyleNetwork
    {
        public NeuralNetwork Network { get; }
        public ObjectStyleNetwork(WriteToCMDLine write)
        {
            Network = Datatype.ObjectStyle.LoadNetwork(write);
        }
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
            var input = a.Forward(Name, lctxt, am, write).ToList();
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
            error = result.Sum();
            return new KeyValuePair<double, List<double>> (error, Results);
        }
        internal static void Backward
            (string Name,
             List<double[]> Results,
             int correct,
             ObjectStyleNetwork net,
             Alpha a,
             AlphaContext ctxt,
             AlphaMem am,
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
            a.Backward(Name, DValues, lctxt, am, AlphaMem, CtxtMem, write);
        }
        internal static double SamplePropogate
            (
            string Name,
            double[] Numbers,
            int correct,
            ObjectStyleNetwork net,
            Alpha a,
            AlphaContext lctxt,
            NetworkMem AlphaMem,
            NetworkMem CtxtMem,
            NetworkMem ObjMem,
            WriteToCMDLine write
            )
        {
            AlphaMem am = new AlphaMem(Name.ToCharArray());
            double error = 0;

            List<double[]> Results = new List<double[]>();
            var input = a.Forward(Name, lctxt, am, write).ToList();
            input.AddRange(Numbers);
            Results.Add(input.ToArray());

            for (int k = 0; k < net.Network.Layers.Count(); k++)
            {
                Results.Add(net.Network.Layers[k].Output(Results.Last()));
            }

            int choice = Results.Last().ToList().IndexOf(Results.Last().Max());
            double[] res = new double[net.Network.Layers.Last().Biases.Count()];
            if (res.Count() > correct)
            {
                res[correct] = 1;
                var result = CategoricalCrossEntropy.Forward(Results.Last(), res);
                error = result.Sum()
                var DValues = res;

                for (int l = net.Network.Layers.Count() - 1; l >= 0; l--)
                {
                    DValues = ObjMem.Layers[l].DActivation(DValues, Results[l + 1]);
                    ObjMem.Layers[l].DBiases(DValues);
                    ObjMem.Layers[l].DWeights(DValues, Results[l]);
                    DValues = ObjMem.Layers[l].DInputs(DValues, net.Network.Layers[l]);
                }
                DValues = DValues.ToList().Take(Alpha.DictSize).ToArray();
                a.Backward(Name, DValues, lctxt, am, AlphaMem, CtxtMem, write);
            }
            return error;
        }
        public static void SinglePropogate
            (string Name, double[] Numbers, int correct, WriteToCMDLine write)
        {
            double error = Double.MaxValue();
            int Prediction = -1;
            ObjectStyleNetwork net = new ObjectStyleNetwork(WriteNull);
            Alpha a = new Alpha(WriteNull);
            AlphaContext ctxt = new AlphaContext(Datatype.ObjectStyle, WriteNull);
            
            while(Prediction != correct)
            {
                var F = Forward(Name, Numbers, correct, net, a, ctxt, am, write);
                Prediction = F.Value.Last().ToList().IndexOf(F.Value.Last().Max());
                if(F.Key > error)
                    break;
                error = F.Key;
                write(error.ToString());
                
                NetworkMem OBJMem = new NetworkMem(net.Network);
                NetworkMem AlphaMem = new NetworkMem(a.Location);
                NetworkMem CtxtMem = new NetworkMem(lctxt.Network);
                
                Backward(Name. F.Value, correct, net, a, ctxt, am, write);
                OBJMem.Update(1, 0.001, net.Network);
                AlphaMem.Update(1, 0.0001, a.Location);
                CtxtMem.Update(1, 0.001, lctxt.Network);
            }
            
            net.Network.Save();
            a.Location.Save();
            lctxt.Save();
        }
        private static string WriteNull(string s) { return s; }
    }
}

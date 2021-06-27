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
    public class ObjectStyleNetwork
    {
        public NeuralNetwork Network { get; }
        public ObjectStyleNetwork(WriteToCMDLine write)
        {
            Network = Datatype.ObjectStyle.LoadNetwork(write);
        }
        public static double[] Predict(string s, double[] vals)
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
            return Results;
        }
        /*
        public static string[] PredictAll(string[] s, WriteToCMDLine write)
        {
            string[] r = new string[s.Count()];
            Alpha a = new Alpha(write);
            int x = 0;
            for(int i = 0; i < s.Count(); i++)
            {
                var pred = ObjectStyleNetwork.Predict(s[i].Split(',')[1]);
                if (pred[int.Parse(s[i].Split(',')[2])] > 0.65)
                {
                    r[i] = "cor : ";
                    x++;
                }
                else
                    r[i] = "inc : ";
                r[i] += x + " / " + (i + 1) + " : " + x / (1.0 + i) + " : " + s[i] + " : " + pred;
                write(r[i]);
            }
            return r;
        }*/
        internal static void SamplePropogate
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
        }
        public static void SinglePropogate
            (string Name, double[] Numbers, int correct)
        {
            ObjectStyleNetwork net = new ObjectStyleNetwork(WriteNull);
            Alpha a = new Alpha(WriteNull);
            AlphaContext lctxt = new AlphaContext(Datatype.ObjectStyle, WriteNull);
            NetworkMem OBJMem = new NetworkMem(net.Network);
            NetworkMem AlphaMem = new NetworkMem(a.Location);
            NetworkMem CtxtMem = new NetworkMem(lctxt.Network);

            SamplePropogate(Name, Numbers, correct, net, a, lctxt, AlphaMem, CtxtMem, OBJMem, WriteNull);
            OBJMem.Update(1, 0.0001, net.Network);
            AlphaMem.Update(1, 0.0001, a.Location);
            CtxtMem.Update(1, 0.0001, lctxt.Network);

            net.Network.Save();
            a.Location.Save();
            lctxt.Save();
        }
        private static string WriteNull(string s) { return s; }
    }
}

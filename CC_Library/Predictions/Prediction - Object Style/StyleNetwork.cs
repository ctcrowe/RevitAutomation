using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
/// what if network iteration was an extensible type rather than a bunch of types
/// NeuralNetwork Network
/// List<double[]> Results
/// public double Forward -> updates all results and returns the error value.
/// public double Backward -> modifies the underlying Neural Network
/// public double[] Predict -> returns ONLY the final layer of Results
    public interface INetworkPrediction
    {
        public NeuralNetwork Network { get; }
        public double[] Predict;
        private double[,] Results;
        public double Forward;
        public bool Backward;
    }
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
                NetworkMem AlphaMem = new NetworkMem(a.Location);
                NetworkMem CtxtMem = new NetworkMem(ctxt.Network);
                
                Backward(Name, F.Value, correct, net, a, ctxt, am, OBJMem, AlphaMem, CtxtMem, WriteNull);
                OBJMem.Update(1, 0.0001, net.Network);
                AlphaMem.Update(1, 0.00001, a.Location);
                CtxtMem.Update(1, 0.0001, ctxt.Network);

            }

            net.Network.Save();
            a.Location.Save();
            ctxt.Save();
        }
        private static string WriteNull(string s) { return s; }
    }
}

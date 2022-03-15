using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library;
using CC_Library.Datatypes;
using System;
using CC_Library.Predictions;


namespace CC_Library.Predictions
{
    [Serializable]
    internal class AlphaFilter1 : IAlphaFilter
    {
        public NeuralNetwork AttentionNetwork { get; }
        public NeuralNetwork ValueNetwork { get; }
        private const int Radius = 3;
        private const int Size = 100;
        private const double ChangeSize = 1e-5;
        internal AlphaFilter1(WriteToCMDLine write)
        {
            AttentionNetwork = new NeuralNetwork(Datatype.Alpha);
            ValueNetwork = new NeuralNetwork(Datatype.Alpha);
            AttentionNetwork.Layers.Add(new Layer(1, CharSet.CharCount * (1 + (2 * Radius)), Activation.Linear));
            ValueNetwork.Layers.Add(new Layer(Size, CharSet.CharCount * (1 + (2 * Radius)), Activation.LRelu, 1e-5, 1e-5));
            ValueNetwork.Layers.Add(new Layer(Size, ValueNetwork.Layers.Last().Weights.GetLength(0), Activation.LRelu, 1e-5, 1e-5));
        }
        public int GetSize() { return Size; }
        public int GetLength(string s, NeuralNetwork net) { return s.Length; }
        public double GetChangeSize() { return ChangeSize; }
        public double[][][][][] Forward(string s, NeuralNetwork net = null)
        {
            try
            {
                double[][][][][] output = new double[3][][][][];
                output[0] = new double[s.Length][][][];
                output[1] = new double[s.Length][][][];
                output[2] = new double[1][][][];
                output[2][0] = new double[1][][];
                output[2][0][0] = new double[3][];
                output[2][0][0][0] = new double[s.Length];
                output[2][0][0][1] = new double[s.Length];
                output[2][0][0][2] = new double[Size];
                
                List<double[]> locations = new List<double[]>();
                locations = s.LocateWords(locations, Radius, 0, net);
                double[,] loc = new double[s.Length, Size];
                
                Parallel.For(0, locations.Count(), j =>
                {
                    output[0][j] = new double[ValueNetwork.Layers.Count() + 1][][];
                    output[0][j][0] = new double[2][];
                    output[0][j][0][0] = s.Locate([j, Radius];
                    output[0][j][0][1] = output[0][j][0][0];
                    for(int i = 0; i < ValueNetwork.Layers.Count(); i++)
                    {
                        output[0][j][i + 1] = new double[2][];
                        output[0][j][i + 1][0] = ValueNetwork.Layers[i].Output(output[0][j][i][1]);
                        output[0][j][i + 1][1] = Layer.DropOut(output[0][j][i+1][0], dropout);
                    }

                    output[1][j] = new double[AttentionNetwork.Layers.Count() + 1][][];
                    output[1][j][0] = new double[2][];
                    output[1][j][0][0] = s.Locate([j, Radius];
                    output[1][j][0][1] = output[1][j][0][0];
                    for(int i = 0; i < AttentionNetwork.Layers.Count(); i++)
                    {
                        output[1][j][i + 1] = new double[2][];
                        output[1][j][i + 1][0] = AttentionNetwork.Layers[i].Output(output[0][j][i][1]);
                        output[1][j][i + 1][1] = Layer.DropOut(output[1][j][i + 1][0], dropout);
                    }
                    output[2][0][0][0][j] = output[1][j][AttentionNetwork.Layers.Count()][0][0];
                });
                output[2][0][0][1] = Activations.SoftMax(output[2][0][0][0]);
                Parallel.For(0, Size, j =>
                {
                    for(int i = 0; i < locations.Count(); i++)
                    {
                        output[2][0][0][2][j] += output[0][i][ValueNetwork.Layers.Count()][0][j] * output[2][0][0][1][i];
                    }
                });
            }
            catch (Exception e) { e.OutputError(); }
            return output;
        }
        public void Backward
            (string s, double[] DValues,
            AlphaMem am, NetworkMem ValMem, NetworkMem FocMem, NeuralNetwork net)
        {
            var LocDValues = am.DLocation(DValues);
            DValues = am.DGlobalContext(DValues);
            DValues = Activations.InverseSoftMax(DValues, am.GlobalContextOutputs);
            Parallel.For(0, s.Length, j =>
            {
                var ldv = LocDValues[j];
                double[] cdv = new double[1] { DValues[j] / s.Length };
                for (int i = ValueNetwork.Layers.Count() - 1; i >= 0; i--)
                {
                    ldv = ValMem.Layers[i].DActivation(ldv, am.LocationOutputs[j][i + 1].GetRank(1));
                    ValMem.Layers[i].DBiases(ldv, ValueNetwork.Layers[i], s.Length);
                    ValMem.Layers[i].DWeights(ldv, am.LocationOutputs[j][i].GetRank(1), ValueNetwork.Layers[i], s.Length);
                    ldv = ValMem.Layers[i].DInputs(ldv, ValueNetwork.Layers[i]);
                }
                for (int i = AttentionNetwork.Layers.Count() - 1; i >= 0; i--)
                {
                    try
                    {
                        cdv = cdv.InverseDropOut(am.LocalContextOutputs[j][i + 1].GetRank(1));
                        cdv = FocMem.Layers[i].DActivation(cdv, am.LocalContextOutputs[j][i + 1].GetRank(1));
                        FocMem.Layers[i].DBiases(cdv, AttentionNetwork.Layers[i], s.Length);
                        FocMem.Layers[i].DWeights(cdv, am.LocalContextOutputs[j][i].GetRank(1), AttentionNetwork.Layers[i], s.Length);
                        cdv = FocMem.Layers[i].DInputs(cdv, AttentionNetwork.Layers[i]);
                    }
                    catch (Exception e) { e.OutputError(); }
                }
            });
        }
    }
}

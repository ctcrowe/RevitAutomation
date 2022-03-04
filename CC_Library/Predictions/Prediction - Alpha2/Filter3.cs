using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System;
using CC_Library.Predictions;


namespace CC_Library.Predictions
{
    [Serializable]
    internal class AlphaFilter3 : IAlphaFilter
    {
        public NeuralNetwork AttentionNetwork { get; }
        public NeuralNetwork ValueNetwork { get; }
        private const int Radius = 5;
        public const int Size = 50;
        private const double ChangeSize = 1e-5;
        internal AlphaFilter3(WriteToCMDLine write)
        {
            AttentionNetwork = new NeuralNetwork(Datatype.Alpha);
            ValueNetwork = new NeuralNetwork(Datatype.Alpha);
            AttentionNetwork.Layers.Add(new Layer(1, 1 + (2 * Radius), Activation.Linear));
            ValueNetwork.Layers.Add(new Layer(Size, ((2 * Radius) + 1) * CharSet.CharCount, Activation.LRelu, 1e-5, 1e-5));
            ValueNetwork.Layers.Add(new Layer(Size, ValueNetwork.Layers.Last().Weights.GetLength(0), Activation.LRelu, 1e-5, 1e-5));
        }
        public int GetSize() { return Size; }
        public int GetLength(string s, NeuralNetwork net) { return s.Length; }
        public double GetChangeSize() { return ChangeSize; }
        public double[] Forward(string s, AlphaMem am, NeuralNetwork net)
        {
            double[,] loc = new double[s.Length, Size];
            Parallel.For(0, s.Length, j =>
            {
                double[,] CtxtInput = new double[2, AttentionNetwork.Layers[0].Weights.GetLength(1)];
                CtxtInput.SetRank(s.LocatePercent(j, Radius), 0);
                CtxtInput.SetRank(s.LocatePercent(j, Radius), 1);
                am.LocalContextOutputs[j].Add(CtxtInput);
                for (int i = 0; i < AttentionNetwork.Layers.Count(); i++)
                {
                    am.LocalContextOutputs[j].Add
                        (AttentionNetwork.Layers[i].Forward(am.LocalContextOutputs[j].Last().GetRank(1), 0));
                }

                double[,] LocInput = new double[2, ValueNetwork.Layers[0].Weights.GetLength(1)];
                LocInput.SetRank(s.Locate(j, Radius), 0);
                LocInput.SetRank(s.Locate(j, Radius), 1);
                am.LocationOutputs[j].Add(LocInput);
                for (int i = 0; i < ValueNetwork.Layers.Count(); i++)
                {
                    am.LocationOutputs[j].Add
                       (ValueNetwork.Layers[i].Forward(am.LocationOutputs[j].Last().GetRank(1)));
                }

                loc.SetRank(am.LocationOutputs[j].Last().GetRank(1), j);
                am.GlobalContextOutputs[j] = am.LocalContextOutputs[j].Last()[0, 0];
            });
            return loc.Multiply(Activations.SoftMax(am.GlobalContextOutputs));
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

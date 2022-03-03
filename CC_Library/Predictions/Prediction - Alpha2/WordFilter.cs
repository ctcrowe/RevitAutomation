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
    internal class WordFilter : IAlphaFilter
    {
        public NeuralNetwork AttentionNetwork { get; }
        public NeuralNetwork ValueNetwork { get; }
        private const int Radius = 15;
        private const int Size = 80;
        private const double ChangeSize = 1e-5;
        internal WordFilter(WriteToCMDLine write)
        {
            AttentionNetwork = new NeuralNetwork(Datatype.Alpha);
            ValueNetwork = new NeuralNetwork(Datatype.Alpha);
            AttentionNetwork.Layers.Add(new Layer(1, CharSet.CharCount * Radius, Activation.Linear));
            ValueNetwork.Layers.Add(new Layer(Size, CharSet.CharCount * Radius, Activation.LRelu, 1e-5, 1e-5));
            ValueNetwork.Layers.Add(new Layer(Size, ValueNetwork.Layers.Last().Weights.GetLength(0), Activation.LRelu, 1e-5, 1e-5));
        }
        public int GetSize() { return Size; }
        public double GetChangeSize() { return ChangeSize; }
        public double[] Forward(string s, AlphaMem am)
        {
            List<double[]> locations = new List<double[]>();
            locations = s.LocateWords(locations, Radius);
            try
            {
                double[,] loc = new double[s.Length, Size];
                Parallel.For(0, locations.Count(), j =>
                {
                    double[,] CtxtInput = new double[2, AttentionNetwork.Layers[0].Weights.GetLength(1)];
                    CtxtInput.SetRank(locations[j], 0);
                    CtxtInput.SetRank(locations[j], 1);
                    am.LocalContextOutputs[j].Add(CtxtInput);
                    for (int i = 0; i < AttentionNetwork.Layers.Count(); i++)
                    {
                        am.LocalContextOutputs[j].Add
                            (AttentionNetwork.Layers[i].Forward(am.LocalContextOutputs[j].Last().GetRank(1), 0));
                    }

                    double[,] LocInput = new double[2, ValueNetwork.Layers[0].Weights.GetLength(1)];
                    LocInput.SetRank(locations[j], 0);
                    LocInput.SetRank(locations[j], 1);
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
            catch (Exception e) { e.OutputError(); }
            return null;
        }
        public void Backward
            (string s, double[] DValues,
            AlphaMem am, NetworkMem ValMem, NetworkMem FocMem)
        {
            var LocDValues = am.DLocation(DValues);
            DValues = am.DGlobalContext(DValues);
            //DValues = Activations.InverseSoftMax(DValues, am.GlobalContextOutputs);
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

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
        private const int Radius = 3;
        public const int Size = 50;
        internal AlphaFilter3(WriteToCMDLine write)
        {
            AttentionNetwork = new NeuralNetwork(Datatype.Alpha);
            ValueNetwork = new NeuralNetwork(Datatype.Alpha);
            AttentionNetwork.Layers.Add(new Layer(1, 1 + (2 * Radius), Activation.Linear));
            ValueNetwork.Layers.Add(new Layer(Size, ((2 * Radius) + 1) * CharSet.CharCount, Activation.LRelu, 1e-5, 1e-5));
            ValueNetwork.Layers.Add(new Layer(Size, ValueNetwork.Layers.Last().Weights.GetLength(0), Activation.LRelu, 1e-5, 1e-5));
        }
        public int GetSize() { return Size; }
        public double[] Forward(string s, AlphaMem am)
        {
            double[,] loc = new double[s.Length, Size];
            Parallel.For(0, s.Length, j =>
            {
                am.LocalContextOutputs[j].Add(s.LocatePercent(j, Radius));
                for (int i = 0; i < AttentionNetwork.Layers.Count(); i++)
                {
                    am.LocalContextOutputs[j].Add
                        (AttentionNetwork.Layers[i].Output(am.LocalContextOutputs[j].Last()));
                }
                
                am.LocationOutputs[j].Add(s.Locate(j, Radius));
                for(int i = 0; i < ValueNetwork.Layers.Count(); i++)
                {
                     am.LocationOutputs[j].Add
                        (ValueNetwork.Layers[i].Output(am.LocationOutputs[j].Last()));
                }
                
                loc.SetRank(am.LocationOutputs[j].Last(), j);
                am.GlobalContextOutputs[j] = am.LocalContextOutputs[j].Last().First();
            });
            return loc.Multiply(Activations.SoftMax(am.GlobalContextOutputs));
        }
        public void Backward
            (string s, double[] DValues,
            AlphaMem am, NetworkMem ValMem, NetworkMem FocMem)
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
                    ldv = ValMem.Layers[i].DActivation(ldv, am.LocationOutputs[j][i + 1]);
                    ValMem.Layers[i].DBiases(ldv, ValueNetwork.Layers[i], s.Length);
                    ValMem.Layers[i].DWeights(ldv, am.LocationOutputs[j][i], ValueNetwork.Layers[i], s.Length);
                    ldv = ValMem.Layers[i].DInputs(ldv, ValueNetwork.Layers[i]);
                }
                for (int i = AttentionNetwork.Layers.Count() - 1; i >= 0; i--)
                {
                    try
                    {
                        cdv = FocMem.Layers[i].DActivation(cdv, am.LocalContextOutputs[j][i + 1]);
                        FocMem.Layers[i].DBiases(cdv, AttentionNetwork.Layers[i], s.Length);
                        FocMem.Layers[i].DWeights(cdv, am.LocalContextOutputs[j][i], AttentionNetwork.Layers[i], s.Length);
                        cdv = FocMem.Layers[i].DInputs(cdv, AttentionNetwork.Layers[i]);
                    }
                    catch (Exception e) { e.OutputError(); }
                }
            });
        }
    }
}

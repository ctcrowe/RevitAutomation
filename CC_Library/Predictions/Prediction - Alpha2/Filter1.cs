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
        private const int Size = 60;
        private const double dropout = 0.1;
        private const double ChangeSize = 1e-5;
        internal AlphaFilter1(WriteToCMDLine write)
        {
            AttentionNetwork = new NeuralNetwork(Datatype.Alpha);
            ValueNetwork = new NeuralNetwork(Datatype.Alpha);
            AttentionNetwork.Layers.Add(new Layer(1, CharSet.CharCount * (1 + (2 * Radius)), Activation.Linear));
            ValueNetwork.Layers.Add(new Layer(Size, CharSet.CharCount * (1 + (2 * Radius)), Activation.LRelu, 1e-5, 1e-5));
            ValueNetwork.Layers.Add(new Layer(Size, ValueNetwork.Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
        }
        public int GetSize() { return Size; }
        public int GetLength(string s, NeuralNetwork net) { return s.Length; }
        public double GetChangeSize() { return ChangeSize; }
        public double[][][][][] Forward(string s, NeuralNetwork net = null)
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

            Parallel.For(0, s.Length, j =>
            {
                output[0][j] = new double[ValueNetwork.Layers.Count() + 1][][];
                output[0][j][0] = new double[2][];
                output[0][j][0][0] = s.Locate(j, Radius, true);
                output[0][j][0][1] = output[0][j][0][0];
                for (int i = 0; i < ValueNetwork.Layers.Count(); i++)
                {
                    output[0][j][i + 1] = new double[2][];
                    try { output[0][j][i + 1][0] = ValueNetwork.Layers[i].Output(output[0][j][i][1]); }
                    catch (Exception exc) { Console.WriteLine("Failed at Value Net Layer : " + i + ", inputs : " + output[0][j][i][1].Count() + ", weights : " +
                        ValueNetwork.Layers[i].Weights.GetLength(0) + ", " + ValueNetwork.Layers[i].Weights.GetLength(1)); }
                    output[0][j][i + 1][1] = Layer.DropOut(output[0][j][i + 1][0], dropout);
                }

                output[1][j] = new double[AttentionNetwork.Layers.Count() + 1][][];
                output[1][j][0] = new double[2][];
                output[1][j][0][0] = s.Locate(j, Radius, true);
                output[1][j][0][1] = output[1][j][0][0];
                for (int i = 0; i < AttentionNetwork.Layers.Count(); i++)
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
                for (int i = 0; i < s.Length; i++)
                {
                    output[2][0][0][2][j] += output[0][i][ValueNetwork.Layers.Count()][0][j] * output[2][0][0][1][i];
                }
            });
            return output;
        }
        public void Backward
            (double[] DValues, double[][][][][] outputs, NetworkMem ValMem, NetworkMem FocMem)
        {
            var ContextualDVals = new double[outputs[0].Count()]; //output[0].Count() => Locations.Count()
            for(int i = 0; i < ContextualDVals.Count(); i++)
            {
                for(int j = 0; j < DValues.Count(); j++) //DValues.Count() => Size
                {
                    ContextualDVals[i] += DValues[j] * outputs[0][i][ValueNetwork.Layers.Count()][1][j];
                }
            }
            ContextualDVals = Activations.InverseSoftMax(ContextualDVals, outputs[2][0][0][0]);
            Parallel.For(0, outputs[0].Count(), j =>
            {
                try
                {
                    double[] LocalDVals = new double[Size];
                    for(int i = 0; i < Size; i++) { LocalDVals[i] = DValues[i] * outputs[2][0][0][0][j]; }
                    for (int i = ValueNetwork.Layers.Count() - 1; i >= 0; i--)
                    {
                        LocalDVals = LocalDVals.InverseDropOut(outputs[0][j][i+1][1]);
                        LocalDVals = ValMem.Layers[i].DActivation(LocalDVals, outputs[0][j][i + 1][0]);
                        ValMem.Layers[i].DBiases(LocalDVals, ValueNetwork.Layers[i], outputs[0].Count());
                        try { ValMem.Layers[i].DWeights(LocalDVals, outputs[0][j][i][1], ValueNetwork.Layers[i], outputs[0].Count()); }
                        catch (Exception e) { Console.WriteLine("Layer Numb : " + i + "Local DVals : " + LocalDVals.Count() + ", outputs : " +
                            outputs[0][j][i][1].Count() + ", Weights : " + ValMem.Layers[i].DeltaW.GetLength(0) + "," + ValMem.Layers[i].DeltaW.GetLength(1)); }
                        LocalDVals = ValMem.Layers[i].DInputs(LocalDVals, ValueNetwork.Layers[i]);
                    }
                    double[] cdv = new double[1] { ContextualDVals[j] / outputs[0].Count() };
                    for (int i = AttentionNetwork.Layers.Count() - 1; i >= 0; i--)
                    {
                        cdv = cdv.InverseDropOut(outputs[1][j][i+1][1]);
                        cdv = FocMem.Layers[i].DActivation(cdv, outputs[1][j][i+1][0]);
                        FocMem.Layers[i].DBiases(cdv, AttentionNetwork.Layers[i], outputs[0].Count());
                        FocMem.Layers[i].DWeights(cdv, outputs[1][j][i][1], AttentionNetwork.Layers[i], outputs[0].Count());
                        cdv = FocMem.Layers[i].DInputs(cdv, AttentionNetwork.Layers[i]);
                    }
                }
                catch (Exception e) { e.OutputError(); }
            });
        }
    }
}

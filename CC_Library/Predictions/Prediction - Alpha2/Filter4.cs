using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System;
using CC_Library.Predictions;

/// <summary>
/// step 1 locate trigram
/// step 2 pull values for Query from trigram (can be a NN or a single layer)
///     This will be set up as a NN that has a couple layers (max)
/// step 3 pull values for Keys from each trigram (can be a NN or a single layer)
///     This will be set up as a NN that has a couple layers (max)
/// step 4 pull values for Values from each trigram (can be a NN or a single layer)
///     This will be set up as a NN that has a couple layers (max)
/// step 5 dot product the Query and the Transpose of the Keys
///     This will be accomplished by multiplying the Query Result with each of the key results across all sets rather than as a single dot product.
/// step 6 softmax each index in the output
/// step 7 multiply the softmax score by the values for the trigram.
/// </summary>

namespace CC_Library.Predictions
{
    [Serializable]
    internal class AlphaFilter4 : IAlphaFilter
    {
        public NeuralNetwork[] Networks { get; }
        private const int Radius = 1;
        private const int Size = 400;
        private const double dropout = 0.1;
        private const double ChangeSize = 0.1;
        internal AlphaFilter4(WriteToCMDLine write)
        {
            this.Networks = new NeuralNetwork[3];
            Networks[0] = new NeuralNetwork(Datatype.Alpha);
            Networks[1] = new NeuralNetwork(Datatype.Alpha);
            Networks[2] = new NeuralNetwork(Datatype.Alpha);
            Networks[0].Layers.Add(new Layer(200, (CharSet.CharCount * (1 + (2 * Radius))), Activation.LRelu, 1e-5, 1e-5));
            Networks[0].Layers.Add(new Layer(Size, Networks[0].Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
            Networks[1].Layers.Add(new Layer(200, (CharSet.CharCount * (1 + (2 * Radius))), Activation.LRelu, 1e-5, 1e-5));
            Networks[1].Layers.Add(new Layer(Size, Networks[1].Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
            Networks[2].Layers.Add(new Layer(200, (CharSet.CharCount * (1 + (2 * Radius))), Activation.LRelu, 1e-5, 1e-5));
            Networks[2].Layers.Add(new Layer(Size, Networks[2].Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
        }
        public string Name { get { return "AlphaFilterv4"; } }
        public int GetSize() { return Size; }
        public double GetChangeSize() { return ChangeSize; }
        public double[][][][][] Forward(string s)
        {
            double[][][][][] output = new double[4][][][][];
            output[0] = new double[s.Length][][][];
            output[1] = new double[s.Length][][][];
            output[2] = new double[s.Length][][][];
            output[3] = new double[1][][][];

            output[3][0] = new double[3][][];
            output[3][0][0] = new double[s.Length][];
            output[3][0][1] = new double[s.Length][];
            output[3][0][2] = new double[s.Length + 1][];
            output[3][0][2][s.Length] = new double[Size];

            Parallel.For(0, s.Length, j =>
            {
                output[0][j] = new double[Networks[0].Layers.Count() + 1][][];
                output[0][j][0] = new double[2][];
                output[0][j][0][0] = s.Locate(j, Radius);
                output[0][j][0][1] = output[0][j][0][0];
                for (int i = 0; i < Networks[0].Layers.Count(); i++)
                {
                    output[0][j][i + 1] = new double[2][];
                    try { output[0][j][i + 1][0] = Networks[0].Layers[i].Output(output[0][j][i][1]); }
                    catch { Console.WriteLine("Failed at Network 0 Layer : " + i + ", inputs : " + output[0][j][i][1].Count() + ", weights : " +
                        Networks[0].Layers[i].Weights.GetLength(0) + ", " + Networks[0].Layers[i].Weights.GetLength(1)); }
                    output[0][j][i + 1][1] = Networks[0].Layers[i].DropOut(output[0][j][i + 1][0], dropout);
                }

                output[1][j] = new double[Networks[1].Layers.Count() + 1][][];
                output[1][j][0] = new double[2][];
                output[1][j][0][0] = s.Locate(j, Radius);
                output[1][j][0][1] = output[1][j][0][0];
                for (int i = 0; i < Networks[1].Layers.Count(); i++)
                {
                    output[1][j][i + 1] = new double[2][];
                    try { output[1][j][i + 1][0] = Networks[1].Layers[i].Output(output[1][j][i][1]); }
                    catch {
                        Console.WriteLine("Failed at Network 1 Layer : " + i + ", inputs : " + output[1][j][i][1].Count() + ", weights : " +
                    Networks[1].Layers[i].Weights.GetLength(0) + ", " + Networks[1].Layers[i].Weights.GetLength(1)); }
                    output[1][j][i + 1][1] = Networks[1].Layers[i].DropOut(output[1][j][i + 1][0], dropout);
                }

                output[2][j] = new double[Networks[2].Layers.Count() + 1][][];
                output[2][j][0] = new double[2][];
                output[2][j][0][0] = s.Locate(2, Radius);
                output[2][j][0][1] = output[2][j][0][0];
                for (int i = 0; i < Networks[2].Layers.Count(); i++)
                {
                    output[2][j][i + 1] = new double[2][];
                    try { output[2][j][i + 1][0] = Networks[2].Layers[i].Output(output[2][j][i][1]); }
                    catch
                    {
                        Console.WriteLine("Failed at Network 1 Layer : " + i + ", inputs : " + output[2][j][i][1].Count() + ", weights : " +
                    Networks[2].Layers[i].Weights.GetLength(0) + ", " + Networks[2].Layers[i].Weights.GetLength(1));
                    }
                    output[2][j][i + 1][1] = Networks[1].Layers[i].DropOut(output[2][j][i + 1][0], dropout);
                }
            });
            
            Parallel.For(0, s.Length, j =>
            {
                try
                {
                    output[3][0][0][j] = new double[s.Length];
                    output[3][0][2][j] = new double[Size];
                    Parallel.For(0, Size, k =>
                    {
                        Parallel.For(0, s.Length, l =>
                        {
                            output[3][0][0][j][l] += output[0][j][Networks[0].Layers.Count()][1][k] * output[1][l][Networks[1].Layers.Count()][1][k];
                        });
                    });
                    output[3][0][1][j] = Activations.SoftMax(output[3][0][0][j]);
                    Parallel.For(0, Size, k =>
                    {
                        Parallel.For(0, s.Length, l =>
                        {
                            var attention = output[3][0][1][j][l] * output[2][l][Networks[2].Layers.Count()][1][k];
                            output[3][0][2][j][k] += attention;
                            output[3][0][2][s.Length][k] += attention;
                        });
                    });
                }
                catch (Exception ex) { ex.OutputError(); }
            });
            return output;
        }
        public void Backward
            (double[] DValues, double[][][][][] outputs, NetworkMem[] mem, WriteToCMDLine write, bool tf = false)
        {
            //first step in  backward pass is to derive each of the softmax layers (there are kind of a lot)
            
            Parallel.For(0, outputs[0].Count(), j=> //relates to s.Length -> this is the number of softmax sets there are.
            {
                var dvals = new double[DValues.Count()]; //this later get feds into the query and keys (Network[0] and Network[1])
                var VDVals = new double[Networks[2].Layers.Last().Biases.Count()]; //this will be fed into the Values (Network[2])
                var KDVals = new double[Networks[1].Layers.Last().Biases.Count()];
                var QDVals = new double[Networks[0].Layers.Last().Biases.Count()];
                
                Parallel.For(0, DValues.Count(), k =>
                {
                    dvals[k] += DValues[k] * outputs[2][j][Networks[2].Layers.Count()][1][k];
                    VDVals[k] += DValues[k] * outputs[3][0][2][j][k]; //TODO Confirm this is correct. Seems wrong.
                });
                for(int i = Networks[2].Layers.Count() - 1; i >= 0; i--)
                {
                    VDVals = Networks[2].Layers[i].InverseDropOut(VDVals, outputs[2][j][i+1][1]);
                    VDVals = mem[2].Layers[i].DActivation(VDVals, outputs[2][j][i+1][0]);
                    mem[2].Layers[i].DBiases(VDVals, Networks[2].Layers[i], outputs[2].Count());
                    mem[2].Layers[i].DWeights(VDVals, outputs[2][j][i][1], Networks[2].Layers[i], outputs[2].Count());
                    VDVals = mem[2].Layers[i].DInputs(VDVals, Networks[2].Layers[i]);
                }
                
                dvals = Activations.InverseSoftMax(dvals, outputs[3][0][0][j]);

                Parallel.For(0, dvals.Count(), k =>
                {
                    KDVals[k] += dvals[k] * outputs[0][j][Networks[0].Layers.Count()][1][k]
                    Parallel.For(0, outputs[0].Count(), l =>
                    {
                        QDVals[k] += dvals[k] * outputs[1][l][Networks[1].Layers.Count()][1][k]
                    });
                });
                for(int i = Networks[1].Layers.Count() - 1; i >= 0; i--)
                {
                    KDVals = Networks[1].Layers[i].InverseDropOut(KDVals, outputs[1][j][i+1][1]);
                    KDVals = mem[1].Layers[i].DActivation(KDVals, outputs[1][j][i+1][0]);
                    mem[1].Layers[i].DBiases(KDVals, Networks[1].Layers[i], outputs[1].Count());
                    mem[1].Layers[i].DWeights(KDVals, outputs[1][j][i][1], Networks[1].Layers[i], outputs[1].Count());
                    KDVals = mem[1].Layers[i].DInputs(KDVals, Networks[1].Layers[i]);
                }
                for(int i = Networks[0].Layers.Count() - 1; i >= 0; i--)
                {
                    QDVals = Networks[0].Layers[i].InverseDropOut(QDVals, outputs[0][j][i+1][1]);
                    QDVals = mem[0].Layers[i].DActivation(QDVals, outputs[0][j][i+1][0]);
                    mem[0].Layers[i].DBiases(QDVals, Networks[0].Layers[i], outputs[0].Count());
                    mem[0].Layers[i].DWeights(QDVals, outputs[0][j][i][1], Networks[0].Layers[i], outputs[0].Count());
                    QDVals = mem[0].Layers[i].DInputs(QDVals, Networks[0].Layers[i]);
                }
            });
        }
    }
}

using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library;
using CC_Library.Datatypes;

        //[][][][][x] => value set
        //[][][][x] => dropout / no. 2 wide always
        //[][][x] => layer number - based on Filter Size (# of layers)
        //[][x] => location location location - this version will make a list and then convert it to an array.
        //[x] => layer group - 3 wide always - 0 = locations, 1 = context, 2 = combined output

        //[2][1][1][3][x] =>
        //  x = [0] = locations, ,[1] = locations, [2] = const int Size above

namespace CC_Library.Predictions
{
    [Serializable]
    internal class WordFilter2 : IAlphaFilter
    {
        public NeuralNetwork[] Networks { get; }
        private const int Radius = 15;
        private const int Size = 50;
        private const double ChangeSize = 1e-3;
        private const double dropout = 0.1;
        internal WordFilter2(WriteToCMDLine write)
        {
            this.Networks = new NeuralNetwork[2];
            Networks[0] = new NeuralNetwork(Datatype.Alpha);
            Networks[0].Layers.Add(new Layer(40, CharSet.CharCount * Radius, Activation.LRelu, 1e-5, 1e-5));
            Networks[0].Layers.Add(new Layer(40, Networks[0].Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
            Networks[0].Layers.Add(new Layer(40, Networks[0].Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
            Networks[0].Layers.Add(new Layer(40, Networks[0].Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
            Networks[0].Layers.Add(new Layer(Radius, Networks[0].Layers.Last(), Activation.SoftMax));
            Networks[1] = new NeuralNetwork(Datatype.Alpha);
            Networks[1].Layers.Add(new Layer(40, CharSet.CharCount * Radius, Activation.LRelu, 1e-5, 1e-5));
            Networks[1].Layers.Add(new Layer(40, Networks[1].Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
            Networks[1].Layers.Add(new Layer(40, Networks[1].Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
            Networks[1].Layers.Add(new Layer(Size, Networks[1].Layers.Last(), Activation.Tangential, 1e-5, 1e-5));
        }
        public string Name { get { return "UndefinedWordFilter"; } }
        public int GetSize() { return Size; }
        public int GetLength(string s, NeuralNetwork net)
        {
            List<double[]> l = new List<double[]>();
            l = s.LocateWords(l, Radius, 0, net);
            return l.Count() + 1;
        }
        public double GetChangeSize() { return ChangeSize; }
        public double[][][][][] Forward(string s)
        {
            int start = 0;
            int length = 0;
            List<double[][][]> output1 = new List<double[][][]>();
            List<double[][][]> output2 = new List<double[][][]>();
            do
            {
                start += length;
                double[][][] LocOut = new double[Networks[0].Layers.Count() + 1][][];
                LocOut[0] = new double[2][];
                LocOut[0][0] = s.LocateSingle(start, Radius);
                LocOut[0][1] = LocOut[0][0];
                for (int i = 0; i < Networks[0].Layers.Count(); i++)
                {
                    LocOut[i + 1] = new double[2][];
                    LocOut[i + 1][0] = Networks[0].Layers[i].Output(LocOut[i][1]);
                    LocOut[i + 1][1] =
                        Networks[0].Layers[i].Function != Activation.SoftMax &&
                        Networks[0].Layers[i].Function != Activation.CombinedCrossEntropySoftmax &&
                        Networks[0].Layers[i].Function != Activation.Tangential &&
                    Layers[k].Function != Activation.Sigmoid ?
                        Layer.DropOut(LocOut[i + 1][0], dropout) : LocOut[i + 1][0];
                }
                length = LocOut.Last().Last().ToList().IndexOf(LocOut.Last().Last().Max()) + 1;
                output1.Add(LocOut);

                double[][][] ValOut = new double[Networks[1].Layers.Count() + 1][][];
                ValOut[0] = new double[2][];
                ValOut[0][0] = s.LocateWord(start, Radius, length);
                ValOut[0][1] = ValOut[0][0];

                for(int i = 0; i < Networks[1].Layers.Count(); i++)
                {
                    ValOut[i + 1] = new double[2][];
                    ValOut[i + 1][0] = Networks[1].Layers[i].Output(ValOut[i][1]);
                    ValOut[i + 1][1] =
                        Networks[1].Layers[i].Function != Activation.SoftMax &&
                        Networks[1].Layers[i].Function != Activation.CombinedCrossEntropySoftmax ?
                        Layer.DropOut(ValOut[i + 1][0], dropout) : ValOut[i + 1][0];
                }
                output2.Add(ValOut);
            }
            while (start + length < s.Length);

            double[][][][][] output = new double[3][][][][];
            output[0] = output1.ToArray();
            output[1] = output2.ToArray();
            output[2] = new double[1][][][];
            output[2][0] = new double[1][][];
            output[2][0][0] = new double[1][];
            output[2][0][0][0] = new double[Size];

            Parallel.For(0, Size, j =>
            {
                for(int i = 0; i < output[0].Count(); i++)
                {
                    output[2][0][0][0][j] += output[1][i][Networks[1].Layers.Count()][0][j];
                }
            });

            return output;
        }
        public void Backward
            (double[] DValues, double[][][][][] outputs, NetworkMem[] mem, WriteToCMDLine write, bool tf = false)
        {
            Parallel.For(0, outputs[1].Count(), j =>
            {
                try
                {
                    var dvals = DValues.Duplicate();
                    for(int i = Networks[1].Layers.Count() - 1; i >= 0; i--)
                    {
                        dvals =
                            Networks[1].Layers[i].Function != Activation.SoftMax &&
                            Networks[1].Layers[i].Function != Activation.CombinedCrossEntropySoftmax ?
                            dvals.InverseDropOut(outputs[1][j][i + 1][1]) : dvals;
                        dvals = mem[1].Layers[i].DActivation(dvals, outputs[1][j][i + 1][0]);
                        mem[1].Layers[i].DBiases(dvals, Networks[1].Layers[i], outputs[1].Count());
                        try
                        { mem[1].Layers[i].DWeights(dvals, outputs[1][j][i][1], Networks[1].Layers[i], outputs[1].Count()); }
                        catch
                        {
                            write("Failed at Layer " + i);
                            write("dvals " + dvals.Count());
                            write("outputs " + outputs[1][j][i][1].Count());
                            write("weights " + Networks[1].Layers[i].Weights.GetLength(0) + ", " + Networks[1].Layers[i].Weights.GetLength(1));
                        }
                        dvals = mem[1].Layers[i].DInputs(dvals, Networks[1].Layers[i]);
                    }
                    for(int i = Networks[0].Layers.Count() - 1; i >= 0; i--)
                    {
                        dvals =
                            Networks[0].Layers[i].Function != Activation.SoftMax &&
                            Networks[0].Layers[i].Function != Activation.CombinedCrossEntropySoftmax ?
                            dvals.InverseDropOut(outputs[0][j][i + 1][1]) : dvals;
                        dvals = mem[0].Layers[i].DActivation(dvals, outputs[0][j][i + 1][0]);
                        mem[0].Layers[i].DBiases(dvals, Networks[0].Layers[i], outputs[0].Count());
                        mem[0].Layers[i].DWeights(dvals, outputs[0][j][i][1], Networks[0].Layers[i], outputs[0].Count());
                        dvals = mem[0].Layers[i].DInputs(dvals, Networks[0].Layers[i]);
                    }
                }
                catch (Exception e) { e.OutputError(); }
            });
        }
    }
}

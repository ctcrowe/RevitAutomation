﻿using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library;
using CC_Library.Datatypes;


namespace CC_Library.Predictions
{
    [Serializable]
    internal class WordFilter : IAlphaFilter
    {
        public NeuralNetwork AttentionNetwork { get; }
        public NeuralNetwork ValueNetwork { get; }
        private const int Radius = 15;
        private const int Size = 50;
        private const double ChangeSize = 1e-3;
        private const double dropout = 0.1;
        internal WordFilter(WriteToCMDLine write)
        {
            AttentionNetwork = new NeuralNetwork(Datatype.Alpha);
            ValueNetwork = new NeuralNetwork(Datatype.Alpha);
            AttentionNetwork.Layers.Add(new Layer(40, CharSet.CharCount * (1 + Radius), Activation.LRelu, 1e-5, 1e-5));
            AttentionNetwork.Layers.Add(new Layer(1, AttentionNetwork.Layers.Last(), Activation.Linear));
            ValueNetwork.Layers.Add(new Layer(40, CharSet.CharCount * (1 + Radius), Activation.LRelu, 1e-5, 1e-5));
            ValueNetwork.Layers.Add(new Layer(40, ValueNetwork.Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
            ValueNetwork.Layers.Add(new Layer(Size, ValueNetwork.Layers.Last(), Activation.LRelu, 1e-5, 1e-5));
        }
        public string Name { get { return "WordFilter"; } }
        public int GetSize() { return Size; }
        public int GetLength(string s, NeuralNetwork net)
        {
            List<double[]> l = new List<double[]>();
            l = s.LocateWords(l, Radius, 0, net);
            return l.Count() + 1;
        }
        public double GetChangeSize() { return ChangeSize; }
        //[][][][][x] => value set
        //[][][][x] => dropout / no. 2 wide always
        //[][][x] => layer number - based on Filter Size
        //[][x] => location location location - varies
        //[x] => layer group - 3 wide always - 0 = locations, 1 = context, 2 = combined output

        //[2][1][1][3][x] =>
        //  x = [0] = locations, ,[1] = locations, [2] = const int Size above
        public double[][][][][] Forward(string s, NeuralNetwork net = null)
        {
            net = net == null ? Predictionary.GetNetwork(CMDLibrary.WriteNull) : net;
            List<double[]> locations = new List<double[]>();
            locations = s.LocateWords(locations, Radius, 0, net);

            double[][][][][] output = new double[3][][][][];
            output[0] = new double[locations.Count()][][][];
            output[1] = new double[locations.Count()][][][];
            output[2] = new double[1][][][];
            output[2][0] = new double[1][][];
            output[2][0][0] = new double[3][];
            output[2][0][0][0] = new double[locations.Count()];
            output[2][0][0][1] = new double[locations.Count()];
            output[2][0][0][2] = new double[Size];

            Parallel.For(0, locations.Count(), j =>
            {
                output[0][j] = new double[ValueNetwork.Layers.Count() + 1][][];
                output[0][j][0] = new double[2][];
                output[0][j][0][0] = locations[j];
                output[0][j][0][1] = locations[j];
                for(int i = 0; i < ValueNetwork.Layers.Count(); i++)
                {
                    output[0][j][i + 1] = new double[2][];
                    output[0][j][i + 1][0] = ValueNetwork.Layers[i].Output(output[0][j][i][1]);
                    output[0][j][i + 1][1] =
                        ValueNetwork.Layers[i].Function != Activation.SoftMax &&
                        ValueNetwork.Layers[i].Function != Activation.CombinedCrossEntropySoftmax ?
                        Layer.DropOut(output[0][j][i + 1][0], dropout) : output[0][j][i + 1][0];
                }

                output[1][j] = new double[AttentionNetwork.Layers.Count() + 1][][];
                output[1][j][0] = new double[2][];
                output[1][j][0][0] = locations[j];
                output[1][j][0][1] = locations[j];
                for(int i = 0; i < AttentionNetwork.Layers.Count(); i++)
                {
                    output[1][j][i + 1] = new double[2][];
                    output[1][j][i + 1][0] = AttentionNetwork.Layers[i].Output(output[0][j][i][1]);
                    output[1][j][i + 1][1] =
                        AttentionNetwork.Layers[i].Function != Activation.SoftMax &&
                        AttentionNetwork.Layers[i].Function != Activation.CombinedCrossEntropySoftmax ?
                        Layer.DropOut(output[1][j][i + 1][0], dropout) : output[1][j][i + 1][0];
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

            return output;
        }
        public void Backward
            (double[] DValues, double[][][][][] outputs, NetworkMem ValMem, NetworkMem FocMem, WriteToCMDLine write, bool tf = false)
        {
            //if (tf) DValues.WriteArray("DValues :", write);
            //if (tf) outputs[0][0][ValueNetwork.Layers.Count()][1].WriteArray("Outputs : ", write);
            var ContextualDVals = new double[outputs[0].Count()]; //output[0].Count() => Locations.Count()
            for(int i = 0; i < ContextualDVals.Count(); i++)
            {
                for(int j = 0; j < DValues.Count(); j++) //DValues.Count() => Size
                {
                    ContextualDVals[i] += DValues[j] * outputs[0][i][ValueNetwork.Layers.Count()][1][j];
                }
            }
            //if (tf) ContextualDVals.WriteArray("Context DVals : ", write);
            ContextualDVals = Activations.InverseSoftMax(ContextualDVals, outputs[2][0][0][0]);
            Parallel.For(0, outputs[0].Count(), j =>
            {
                try
                {
                    double[] LocalDVals = new double[Size];
                    for(int i = 0; i < Size; i++) { LocalDVals[i] = DValues[i] * outputs[2][0][0][1][j]; }
                    for (int i = ValueNetwork.Layers.Count() - 1; i >= 0; i--)
                    {
                        LocalDVals =
                            ValueNetwork.Layers[i].Function != Activation.SoftMax &&
                            ValueNetwork.Layers[i].Function != Activation.CombinedCrossEntropySoftmax ? 
                            LocalDVals.InverseDropOut(outputs[0][j][i+1][1]) : LocalDVals;
                        LocalDVals = ValMem.Layers[i].DActivation(LocalDVals, outputs[0][j][i + 1][0]);
                        ValMem.Layers[i].DBiases(LocalDVals, ValueNetwork.Layers[i], outputs[0].Count());
                        ValMem.Layers[i].DWeights(LocalDVals, outputs[0][j][i][1], ValueNetwork.Layers[i], outputs[0].Count());
                        LocalDVals = ValMem.Layers[i].DInputs(LocalDVals, ValueNetwork.Layers[i]);
                    }
                    double[] cdv = new double[1] { ContextualDVals[j] / outputs[0].Count() };
                    for (int i = AttentionNetwork.Layers.Count() - 1; i >= 0; i--)
                    {
                        cdv =
                            AttentionNetwork.Layers[i].Function != Activation.SoftMax &&
                            AttentionNetwork.Layers[i].Function != Activation.CombinedCrossEntropySoftmax ?
                            cdv.InverseDropOut(outputs[1][j][i+1][1]) : cdv;
                        cdv = FocMem.Layers[i].DActivation(cdv, outputs[1][j][i+1][0]);
                        FocMem.Layers[i].DBiases(cdv, AttentionNetwork.Layers[i], outputs[0].Count());
                        FocMem.Layers[i].DWeights(cdv, outputs[1][j][i][1], AttentionNetwork.Layers[i], outputs[0].Count());
                        cdv = FocMem.Layers[i].DInputs(cdv, AttentionNetwork.Layers[i]);
                    }
                }
                catch (Exception e) { e.OutputError(); }
            });
        }
        /*
        public double[] Forward(string s, AlphaMem am, NeuralNetwork net = null)
        {
            net = net == null ? Predictionary.GetNetwork(CMDLibrary.WriteNull) : net;
            List<double[]> locations = new List<double[]>();
            locations = s.LocateWords(locations, Radius, 0, net);
            try
            {
                double[,] loc = new double[locations.Count, Size];
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
        }*/
    }
}
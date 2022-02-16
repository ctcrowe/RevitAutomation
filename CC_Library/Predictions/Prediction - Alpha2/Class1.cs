/*
    TODO:
    New Locate Command that uses a network to predict the beginning and end of each word. This will then advance the term by n, where n is the length of the word.
    Ultimately, breaking a phrase down into words. Search Radius will need to be substantially large, potentially 10 characters +/-. This will give us access to a set of pseudo words,
    without having to instantiate a dictionary for prediction purposes, giving more flexibility than a dictionary, but more structure than just letters to determine terms.
    
    This Network needs to be relatively small and quick, to interpret words on the fly fairly efficiently.
    Base Layer 1 size to have search radius 2 and Locate by character.
    Additional Base Layer to have coordintaed search size and locate a set of characters (potentially turns them into something like a syllable.)
    These syllables will then be interpreted into words, starting and ending being highlighted.
*/
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using CC_Library;
using CC_Library.Datatypes;
using CC_Library.Predictions;

namespace CC_Library.Predictions
{
    internal class Predictionary
    {
        internal Predictionary(WriteToCMDLine write)
        {
            Network = Datatype.Alpha.LoadNetwork(write);
            if(Network.Datatype == Datatype.None)
            {
                Network = new NeuralNetwork(Datatype.Alpha);
                Network.Layers.Add(new Layer(Size, ((2 * Radius) + 1) * CharSet.CharCount, Activation.LRelu, 1e-5, 1e-5));
                Network.Layers.Add(new Layer(Size, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 1e-5, 1e-5));
                Network.Layers.Add(new Layer(1, Network.Layers.Last().Weights.GetLength(0), Activation.Linear, 1e-5, 1e-5));
            }
        }
        
        public const int Size = 25;
        public const int Radius = 2;
        public NeuralNetwork Network { get; }
        
        public int Output(string s, int start)
        {
            double[] output = new double[s.Length];
            Parallel.For(start, s.Length, j =>
                         {
                             var result = s.Locate(j, Radius);
                             for(int i = 0; i < Network.Layers.Count(); i++)
                             {
                                 result = Network.Layers[i].Output(result);
                             }
                             output[j] = result.First();
                         });
            return output.ToList().IndexOf(output.Max());
        }
        public List<double[,]>[] Forward(string s)
        {
            List<double[,]>[] Output = new List<double[,]>[s.Length + 1];
            Output[s.Length] = new List<double[,]>();
            Output[s.Length].Add(new double[2, s.Length]);
            
            try
            {
                Parallel.For(0, s.Length, j =>
                {
                    Output[j] = new List<double[,]>();
                    Output[j].Add(new double[2, Network.Layers[0].Weights.GetLength(1)]);
                    Output[j][0].SetRank(s.Locate(j, Radius), 0);
                    Output[j][0].SetRank(s.Locate(j, Radius), 1);
                    
                    for (int i = 0; i < Network.Layers.Count(); i++)
                    {
                        Output[j].Add
                           (Network.Layers[i].Forward(Output[j].Last().GetRank(1), 0.1));
                    }
                    
                    Output[s.Length][0][0, j] = Output[j].Last()[0,0];
                });
                Output[s.Length][0].SetRank(Activations.SoftMax(Output[s.Length][0].GetRank(0)), 1);
            }
            catch (Exception e) { e.OutputError(); }
            return Output;
        }
        public void Backward(string s, NetworkMem mem, double[] DValues, List<double[,]>[] Output)
        {
            DValues = Activations.InverseSoftMax(DValues, Output.Last().First().GetRank(0));
            Parallel.For(0, s.Length, j =>
            {
                var ldv = DValues[j];
                for (int i = Network.Layers.Count() - 1; i >= 0; i--)
                {
                    ldv = mem.Layers[i].DActivation(ldv, Output[j][i].GetRank(1));     //am.LocationOutputs[j][i + 1]);
                    mem.Layers[i].DBiases(ldv, Network.Layers[i], s.Length);
                    mem.Layers[i].DWeights(ldv, Output[j][i].GetRank(1), Network.Layers[i], s.Length);
                    ldv = mem.Layers[i].DInputs(ldv, Network.Layers[i]);
                }
            });
        }
    }
}

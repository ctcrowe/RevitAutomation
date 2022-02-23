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
using System.IO;
using System.Threading.Tasks;
using CC_Library;
using CC_Library.Datatypes;
using CC_Library.Predictions;

namespace CC_Library.Predictions
{
    internal static class Predictionary
    {
        internal static NeuralNetwork GetNetwork(WriteToCMDLine write)
        {
            NeuralNetwork Network = Datatype.Alpha.LoadNetwork(write);
            if(Network.Datatype == Datatype.None)
            {
                Network = new NeuralNetwork(Datatype.Alpha);
                Network.Layers.Add(new Layer(Size, ((2 * Radius) + 1) * CharSet.CharCount, Activation.LRelu, 1e-5, 1e-5));
                Network.Layers.Add(new Layer(Size, Network.Layers.Last().Weights.GetLength(0), Activation.LRelu, 1e-5, 1e-5));
                Network.Layers.Add(new Layer(1, Network.Layers.Last().Weights.GetLength(0), Activation.Linear, 1e-5, 1e-5));
            }
            return Network;
        }
        
        public const int Size = 50;
        public const int Radius = 5;
        private const double dropout = 0.0001;
        
        public static int Output(string s, int start)
        {
            var net = GetNetwork(CMDLibrary.WriteNull);
            double[] output = new double[s.Length];
            Parallel.For(start, s.Length, j =>
                         {
                             var result = s.Locate(j, Radius);
                             for(int i = 0; i < net.Layers.Count(); i++)
                             {
                                 result = net.Layers[i].Output(result);
                             }
                             output[j] = result.First();
                         });
            return output.ToList().IndexOf(output.Max());
        }
        public static Dictionary<string, int> GetSamples(string fn, int numb = 24)
        {
            var Lines = File.ReadLines(fn);
            Dictionary<string, int> inputs = new Dictionary<string, int>();
            Random r = new Random();
            
            for(int i = 0; i < numb; i++)
            {
                var line = Lines.ElementAt(r.Next(Lines.Count()));
                var parts = line.Split(',');
                string input = parts[0];
                int number = r.Next(1, parts.Count());
                if (int.TryParse(parts[number], out int output))
                {
                    bool inclresult = true;
                    if (number > 1)
                    {
                        for (int j = 1; j < number; j++)
                        {
                            if (int.TryParse(parts[j], out int removal))
                                input = input.Remove(0, removal);
                            else
                            {
                                inclresult = false;
                                break;
                            }
                        }
                    }
                    if(inclresult && !inputs.ContainsKey(input)) inputs.Add(input, output - 1);
                }
                
            }
            return inputs;
        }
        public static double Propogate(string fn, WriteToCMDLine write)
        {
            double error = 0;
            {
                NeuralNetwork net = GetNetwork(write);
                var Samples = GetSamples(fn, 24);
                NetworkMem mem = new NetworkMem(net);

                try
                {
                    Parallel.For(0, Samples.Count(), z =>
                    {
                        var s = Samples.Keys.ElementAt(z);
                        List<double[,]>[] Output = new List<double[,]>[s.Length + 1];
                        Output[s.Length] = new List<double[,]>();
                        Output[s.Length].Add(new double[2, s.Length]);
            
                        try
                        {
                            Parallel.For(0, s.Length, j =>
                            {
                                Output[j] = new List<double[,]>();
                                Output[j].Add(new double[2, net.Layers[0].Weights.GetLength(1)]);
                                Output[j][0].SetRank(s.Locate(j, Radius), 0);
                                Output[j][0].SetRank(s.Locate(j, Radius), 1);

                                for (int i = 0; i < net.Layers.Count(); i++)
                                    Output[j].Add(
                                        i == net.Layers.Count() - 1 ? net.Layers[i].Forward(Output[j].Last().GetRank(1), 0) :
                                                                      net.Layers[i].Forward(Output[j].Last().GetRank(1), dropout));

                                Output[s.Length][0][0, j] = Output[j].Last()[0, 0];
                            });
                            Output[s.Length][0].SetRank(Activations.SoftMax(Output[s.Length][0].GetRank(0)), 1);
                            var Desired = new double[s.Length];
                            Desired[Samples[s]] = 1;

                            error += CategoricalCrossEntropy.Forward(Output[s.Length].Last().GetRank(1), Desired).Max();
                            var DValues = Activations.InverseSoftMax(Desired, Output.Last().First().GetRank(1));
                            Parallel.For(0, s.Length, j =>
                            {
                                var ldv = new double[1] { DValues[j] };
                                for (int i = net.Layers.Count() - 1; i >= 0; i--)
                                {
                                    ldv = mem.Layers[i].DActivation(ldv, Output[j][i + 1].GetRank(0));
                                    mem.Layers[i].DBiases(ldv, net.Layers[i], s.Length);
                                    mem.Layers[i].DWeights(ldv, Output[j][i].GetRank(0), net.Layers[i], s.Length);
                                    ldv = mem.Layers[i].DInputs(ldv, net.Layers[i]);
                                }
                            });
                        }
                        catch (Exception e) { e.OutputError(); }
                    });
                }
                catch (Exception e) { e.OutputError(); }
                
                mem.Update(Samples.Count(), 0.1, net);
                write("Error : " + error);
                net.Save();
            }
            return error;
        }
    }
}

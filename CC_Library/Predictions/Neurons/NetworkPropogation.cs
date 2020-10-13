using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class NetworkPropogation
    {
        public delegate void SaveNetwork(NeuralNetwork Network, List<string> Words);
        public delegate void AccuracyOutput(List<Entry> entries, NeuralNetwork network, List<string> words, bool tf);
        private const int RunSize = 16;

        internal static void Propogate
            (this string filepath,
            Datatype datatype,
            Func<string, WriteToCMDLine, List<Entry>> GetEntry,
            Func<Entry, List<string>, double[]> Input,
            Func<Entry, List<string>, double[]> Output,
            Func<double[], double[], double[]> Forward,
            Func<double[], double[], double[]> Backward,
            Func<List<Entry>, NeuralNetwork, List<string>, double[]> Accuracy,
            AccuracyOutput output,
            SaveNetwork Save,
            WriteToCMDLine write)
        {
            Random random = new Random();

            var Entries = filepath.GetEntries(GetEntry, write);

            List<string> Words = new List<string>();
            if(datatype == Datatype.Dictionary)
            {
                for (int i = 0; i < Entries.Count(); i++)
                {
                    var w = Entries[i].Keys[0];
                    if (!Words.Contains(w))
                        Words.Add(w);
                }
            }

            var Network = datatype.LoadNetwork(Words, write);
            Save(Network, Words);
            write("Network Saved");

            int count = 1;
            double[] RunAccuracy = new double[4];
            
            while (count < 50000)
            {
                var Entryset = Entries.OrderBy(x => random.Next()).ToList();

                for (int i = 0; i < Entryset.Count() - (RunSize - 1); i += RunSize)
                {

                    do
                    {
                        double Error = 0;
                        write("Run Number : " + count);

                        Parallel.For(0, RunSize, j =>
                        {
                            var input = Input(Entryset[i + j], Words);
                            var DesiredOutput = Output(Entryset[i + j], Words);
                            List<double[]> Zees = new List<double[]>();
                            List<double[]> Results = new List<double[]>();
                            Results.Add(input);

                            for (int k = 0; k < Network.Layers.Count(); k++)
                            {
                                Zees.Add(Network.Layers[k].ZScore(Results.Last()));
                                Results.Add(Network.Layers[k].Output(Zees.Last()));
                            }

                            var result = Forward(Results.Last(), DesiredOutput);
                            Error += result.MeanLoss();
                            var DValues = Backward(Results.Last(), DesiredOutput);

                            for (int k = Network.Layers.Count() - 1; k >= 0; k--)
                            {
                                DValues = Network.Layers[k].DActivation(DValues, Zees[k], Results[k + 1]);
                                Network.Layers[k].DBiases(DValues);
                                if (j == 1)
                                {
                                    write("Propogating Layer : " + k);
                                    write("DValues : " + DValues.Count());
                                    write("Inputs : " + Results[k].Count());
                                    write("Weight Size : " + Network.Layers[k].Weights.GetLength(0) + " , " + Network.Layers[k].Weights.GetLength(1));
                                }
                                Network.Layers[k].DWeights(DValues, Results[k]);
                                DValues = Network.Layers[k].DInputs(DValues);
                            }
                        });

                        for (int j = 0; j < Network.Layers.Count(); j++)
                        {
                            Network.Layers[j].DeltaB.Divide(RunSize);
                            Network.Layers[j].DeltaW.Divide(RunSize);
                        }

                        Error /= RunSize;

                        Parallel.For(0, Network.Layers.Count(), j => Network.Layers[j].Update(1e-3));

                        RunAccuracy = Accuracy(Entries, Network, Words);
                        write("\tError is : " + Error);
                        write("\tBase Accuracy is " + RunAccuracy[0] + " / " + RunAccuracy[1] + " = " + RunAccuracy[0] / RunAccuracy[1]);
                        write("\tBase Negative Distance is : " + RunAccuracy[2]);
                        write("\tBase Positive Distance is : " + RunAccuracy[3]);
                        write("");

                        count++;
                    }
                    while (count % 10 > 0);
                    Save(Network, Words);
                    output(Entries, Network, Words, false);
                }
            }
            output(Entries, Network, Words, true);
        }
    }
}
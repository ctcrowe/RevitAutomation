using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal class NetworkMem
    {
        public List<LayerMem> Layers { get; set; }
        public NetworkMem(NeuralNetwork net, int RunSize)
        {
            this.Layers = new List<LayerMem>();
            Parallel.For(0, net.Layers.Count, i => Layers[i] = new LayerMem(net.Layers[i], RunSize));
        }
    }
    internal class LayerMem
    {
        public Layer layer { get; set; }
        public double[] DeltaB { get; set; }
        public double[,] DeltaW { get; set; }
        public Activation Function { get; set; }
        public LayerMem(Layer l)
        {
            layer = l;
            DeltaW = new double[l.Weights.GetLength(0), l.Weights.GetLength(1)];
            DeltaB = new double[l.Biases.Count()];
            Function = l.Function;
        }
        public double[] DActivation(double[] dvalues, double[] output) { return Function.InvertFunction()(dvalues, output); }
        public void DBiases(double[] dvalues) { DeltaB.Add(dvalues); }
        public void DWeights(double[] dvalues, double[] inputs)
        {
            for (int i = 0; i < DeltaW.GetLength(0); i++)
            {
                for (int j = 0; j < DeltaW.GetLength(1); j++)
                {
                    DeltaW[i, j] += inputs[j] * dvalues[i];
                }
            }
        }
        /*
        public void AddDerivative(Layer l)
        {
            if (l.Weights.GetLength(0) == Weights.GetLength(0) && l.Weights.GetLength(1) == Weights.GetLength(1))
            {
                for (int i = 0; i < Weights.GetLength(0); i++)
                {
                    DeltaB[i] += l.DeltaB[i];
                    for (int j = 0; j < Weights.GetLength(1); j++)
                    {
                        DeltaW[i, j] += l.DeltaW[i, j];
                    }
                }
            }
        }
        */
    }
}

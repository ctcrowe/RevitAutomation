using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    public class NetworkMem
    {
        public LayerMem[] Layers { get; set; }
        public NetworkMem(NeuralNetwork net)
        {
            this.Layers = new LayerMem[net.Layers.Count()];
            Parallel.For(0, net.Layers.Count(), i => Layers[i] = new LayerMem(net.Layers[i]));
        }
        internal void Update(int RunSize, double ChangeSize, NeuralNetwork Net)
        {
            Parallel.For(0, this.Layers.Count(), j =>
            {
                this.Layers[j].DeltaB.Divide(RunSize);
                this.Layers[j].DeltaW.Divide(RunSize);
                this.Layers[j].Update(Net.Layers[j], ChangeSize);
            });
        }
    }
    public class LayerMem
    {
        public double[] DeltaB { get; set; }
        public double[,] DeltaW { get; set; }
        public Activation Function { get; set; }
        public LayerMem(Layer l)
        {
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
        public double[] DInputs(double[] dvalues, Layer layer)
        {
            double[] result = new double[this.DeltaW.GetLength(1)];
            for(int i = 0; i < this.DeltaW.GetLength(0); i++)
            {
                for(int j = 0; j < this.DeltaW.GetLength(1); j++)
                {
                    result[j] += dvalues[i] * layer.Weights[i, j];
                }
            }
            return result;
        }
        public void Update(Layer layer, double adjustment)
        {
            Parallel.For(0, DeltaB.Count(), i =>
            {
                    layer.Biases[i] = double.IsNan(DeltaB[i]) ? 
                        layer.Biases[i] : DeltaB[i] >= 1 ?
                        layer.Biases[i] - adjustment : DeltaB[i] <= -1?
                        layer.Biases[i] + adjustment : layer.Biases[i] - (adjustment * DeltaB[i]);
            });
            Parallel.For(0, layer.Weights.GetLength(0), i =>
            {
                Parallel.For(0, layer.Weights.GetLength(1), j =>
                {
                    layer.Weights[i, j] = double.IsNan(DeltaW[i, j]) ? 
                        layer.Weights[i, j] : DeltaW[i, j] >= 1 ?
                        layer.Weights[i, j] - adjustment : DeltaW[i, j] <= -1?
                        layer.Weights[i, j] + adjustment : layer.Weights[i, j] - (adjustment * DeltaW[i, j]);
                });
            });
            Reset();
        });
        public void Reset()
        {
            DeltaB = new double[DeltaB.Count()];
            DeltaW = new double[DeltaW.GetLength(0), DeltaW.GetLength(1)];
        }
    }
}

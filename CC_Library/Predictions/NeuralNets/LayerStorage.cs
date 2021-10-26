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
        public void DBiases(double[] dvalues, Layer l)
        {
            DeltaB.Add(dvalues);
            if (l.L1Regularization > 0)
            {
                Parallel.For(0, l.Biases.GetLength(0), i =>
                {
                    DeltaB[i] += l.Biases[i] >= 0 ? l.L1Regularization : (-1 * l.L1Regularization);
                    DeltaB[i] += 2 * l.L2Regularizattion * l.Biases[i];
                });
            }
        }
        public void DWeights(double[] dvalues, double[] inputs, Layer l)
        {
            for (int i = 0; i < DeltaW.GetLength(0); i++)
            {
                for (int j = 0; j < DeltaW.GetLength(1); j++)
                {
                    DeltaW[i, j] += inputs[j] * dvalues[i];
                }
            }
            if (l.L1Regularization > 0)
            {
                Parallel.For(0, l.Weights.GetLength(0), i =>
                {
                    Parallel.For(0, l.Weights.GetLength(1), j =>
                    {
                        DeltaW[i, j] += l.Weights[i, j] >= 0 ? l.L1Regularization : (-1 * l.L1Regularization);
                        DeltaW[i, j] += 2 * l.L2Regularizattion * l.Weights[i, j];
                    });
                });
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
                    layer.BMomentum[i] = double.IsNaN(DeltaB[i]) ? 
                        layer.BMomentum[i] : layer.BMomentum[i] - (adjustment * DeltaB[i]);
                //DeltaB[i] >= 1 ?
                        //layer.BMomentum[i] - adjustment : DeltaB[i] <= -1 ?
                        //layer.BMomentum[i] + adjustment : layer.BMomentum [i] - (adjustment * DeltaB[i]);
            });
            Parallel.For(0, layer.Weights.GetLength(0), i =>
            {
                Parallel.For(0, layer.Weights.GetLength(1), j =>
                {
                    layer.WMomentum[i, j] = double.IsNaN(DeltaW[i, j]) ? 
                        layer.WMomentum[i, j] : layer.WMomentum[i, j] - (adjustment * DeltaW[i, j]);
                    //DeltaW[i, j] >= 1 ?
                        //layer.WMomentum[i, j] - adjustment : DeltaW[i, j] <= -1?
                        //layer.WMomentum[i, j] + adjustment : layer.WMomentum[i, j] - (adjustment * DeltaW[i, j]);
                });
            });
            layer.Update();
            Reset();
        }
        public void Reset()
        {
            DeltaB = new double[DeltaB.Count()];
            DeltaW = new double[DeltaW.GetLength(0), DeltaW.GetLength(1)];
        }
    }
}

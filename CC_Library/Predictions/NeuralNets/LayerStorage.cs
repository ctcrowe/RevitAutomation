using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal class LayerStor
    {
        public double[,] Output { get; set; }
        public double[,] Input { get; set; }
        public double[] DeltaB { get; set; }
        public double[,] DeltaW { get; set; }
        public Activation Function { get; set; }
        public LayerStor(Layer l, int RunSize)
        {
            Output = new double[RunSize, l.Biases.Count()];
            Input = new double[RunSize, l.Weights.GetLength(1)];
            DeltaW = new double[l.Weights.GetLength(0), l.Weights.GetLength(1)];
            DeltaB = new double[l.Biases.Count()];
            Function = l.Function;
        }
        public double[] DActivation(double[] dvalues, int Run)
        {
            var function = Function.InvertFunction();
            double[] output = Output.GetRank(Run);
            var deriv = function(dvalues, output);
            return deriv;
        }
        public void DBiases(double[] dvalues)
        {
            DeltaB.Add(dvalues);
        }
        public void DWeights(double[] dvalues, int Run)
        {
            var inputs = Input.GetRank(Run);
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

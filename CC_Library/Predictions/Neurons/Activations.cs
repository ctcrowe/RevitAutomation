using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal enum Activation
    {
        ReLu,
        SoftMax,
        Linear,
        Sigmoid
    }
    internal static class Activations
    {
        public static Func<double[], double[]> GetFunction(this Activation a)
        {
            switch (a)
            {
                default:
                case Activation.ReLu:
                    return ReLu;
                case Activation.SoftMax:
                    return SoftMax;
                case Activation.Linear:
                    return Linear;
                    
            }
        }
        public static Func<double[], double[], double[], double[]> InvertFunction(this Activation a)
        {
            switch (a)
            {
                default:
                case Activation.ReLu:
                    return InverseReLu;
                case Activation.SoftMax:
                    return InverseSoftMax;
                case Activation.Linear:
                    return InverseLinear;
            }
        }
        public static double[] Linear(double[] x)
        {
            return x;
        }
        public static double[] InverseLinear(double[] dvalues, double[] inputs, double[] output)
        {
            double[] y = new double[dvalues.Count()];
            for(int i =0; i < y.Count(); i++)
            {
                y[i] = dvalues[i];
            }
            return y;
        }
        public static double[] ReLu(double[] x)
        {
            double[] result = new double[x.Count()];
            for (int i = 0; i < x.Count(); i++)
            {
                if (x[i] > 0)
                    result[i] = x[i];
                else
                    result[i] = 0;
            }
            return result;
        }
        public static double[] InverseReLu(double[] dvalues, double[] inputs, double[] outputs)
        {
            double[] dinput = new double[dvalues.Count()];
            for (int j = 0; j < inputs.Count(); j++)
            {
                if (inputs[j] <= 0) dinput[j] = 0;
                else dinput[j] = dvalues[j];
            }
            return dinput;
        }
        /*
        public static double[] Sigmoid(double[] x)
        {
            double[] result = new double[x.Count()];
            for(int i = 0; i < x.Count(); i++)
            {
                result[i] = 0.5 * (1 + Math.Tanh(0.5 * x[i]));
            }
            return result;
        }
        public static double[] InverseSigmoid(double[] dvalues, double[] outputs)
        {
            double[] result = new double[x.Count()];
            for(int i = 0; i < x.Count(); i++)
            {
                double a = x[i] / 2;
                result[i] = 0.25 * Math.Pow(a.SecH(), 2);
            }
            return result;
        }
        */
        public static double[] SoftMax(double[] input)
        {
            double[] ExpVals = new double[input.Count()];
            double[] output = new double[input.Count()];
            for (int i = 0; i < ExpVals.Count(); i++)
            {
                ExpVals[i] = Math.Exp(input[i] - input.Max());
            }
            for (int i = 0; i < ExpVals.Count(); i++)
            {
                output[i] = ExpVals[i] / ExpVals.Sum();
            }
            return output;
        }
        public static double[] InverseSoftMax(double[] dvalues, double[] inputs, double[] outputs)
        {

            double[] result = new double[dvalues.Count()];
            double[,] diag = outputs.DiagFlat();
            double[,] Jacobian = new double[diag.GetLength(0), diag.GetLength(1)];

            for (int j = 0; j < Jacobian.GetLength(0); j++)
            {
                for (int k = 0; k < Jacobian.GetLength(1); k++)
                {
                    Jacobian[j, k] = diag[j, k] - (outputs[j] * outputs[k]);
                    result[j] += Jacobian[j, k] * dvalues[k];
                }
            }
            return result;
        }
    }
}

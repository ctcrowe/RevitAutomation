using System;
using System.Linq;

namespace CC_Library.Predictions
{
    [Serializable]
    public enum Activation
    {
        ReLu,
        SoftMax,
        Linear,
        Sigmoid,
        LRelu,
        SeLu,
        Tangential,
        CombinedCrossEntropySoftmax
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
                case Activation.CombinedCrossEntropySoftmax:
                    return SoftMax;
                case Activation.Linear:
                    return Linear;
                case Activation.Sigmoid:
                    return Sigmoid;
                case Activation.LRelu:
                    return LRelu;
                case Activation.Tangential:
                    return Tangential;
                case Activation.SeLu:
                    return SeLu;
            }
        }
        public static Func<double[], double[], double[]> InvertFunction(this Activation a)
        {
            switch (a)
            {
                default:
                case Activation.ReLu:
                    return InverseReLu;
                case Activation.LRelu:
                    return InverseLRelu;
                case Activation.SoftMax:
                    return InverseSoftMax;
                case Activation.Linear:
                    return InverseLinear;
                case Activation.Sigmoid:
                    return InverseSigmoid;
                case Activation.Tangential:
                    return InverseTangential;
                case Activation.SeLu:
                    return InverseSeLu;
                case Activation.CombinedCrossEntropySoftmax:
                    return InverseCombinedCrossEntropySoftmax;
            }
        }
        public static double[] Linear(double[] x)
        {
            return x;
        }
        public static double[] InverseLinear(double[] dvalues, double[] output)
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
                result[i] = Math.Max(x[i], 0);
            }
            return result;
        }
        //DValues is the derivative values coming in from the previous layer.
        //IF there are more DValues than there are inputs, we should always take in the outp
        public static double[] InverseReLu(double[] dvalues, double[] outputs)
        {
            double[] dinput = new double[outputs.Count()];
            for (int j = 0; j < outputs.Count(); j++)
            {
                dinput[j] = dvalues[j] * Math.Max(outputs[j], 0) / outputs[j];
            }
            return dinput;
        }
        public static double[] LRelu(double[] x)
        {
            double[] result = new double[x.Count()];
            for (int i = 0; i < x.Count(); i++)
            {
                result[i] = Math.Max(x[i], x[i] / 100);
            }
            return result;
        }
        public static double[] InverseLRelu(double[] dvalues, double[] outputs)
        {
            double[] dinput = new double[outputs.Count()];
            for (int j = 0; j < outputs.Count(); j++)
            {
                dinput[j] = outputs[j] <= 0?
                    dvalues[j] / 100 : dvalues[j];
            }
            return dinput;
        }
        public static double[] SeLu(double[] x)
        {
            double a = 1.67326324;
            double scale = 1.05070098;
            double[] result = new double[x.Count()];
            for(int i = 0; i < x.Count(); i++)
            {
                if (x[i] <= 0)
                    result[i] = a * scale * (Math.Exp(x[i]) - 1);
                else
                    result[i] = x[i];
            }
            return result;
        }
        public static double[] InverseSeLu(double[] dvalues, double[] outputs)
        {
            double a = 1.67326324;
            double scale = 1.05070098;
            double[] dinput = new double[outputs.Count()];
            for (int j = 0; j < outputs.Count(); j++)
            {
                if (outputs[j] <= 0)
                    dinput[j] = dvalues[j] * a * scale * Math.Exp(outputs[j]);
                else
                    dinput[j] = dvalues[j];
            }
            return dinput;
        }
        public static double[] Sigmoid(double[] x)
        {
            double[] result = new double[x.Count()];
            for(int i = 0; i < x.Count(); i++)
            {
                result[i] = 1 / (1 + (Math.Exp(-1 * x[i])));
            }
            return result;
        }
        public static double[] InverseSigmoid(double[] dvalues, double[] outputs)
        {
            double[] result = new double[outputs.Count()];
            for(int i = 0; i < outputs.Count(); i++)
            {
                result[i] = dvalues[i] * (1 - outputs[i]) * outputs[i];
            }
            return result;
        }
        public static double[] SoftMax(double[] input)
        {
            double[] output = new double[input.Count()];
            for (int i = 0; i < input.Count(); i++)
            {
                input[i] = input[i] >= double.MaxValue ?
                    double.MaxValue : input[i] <= double.MinValue ?
                        double.MinValue : input[i];
            }
            double max = input.Max();
            for (int i = 0; i < output.Count(); i++)
            {
                double a = input[i] - max;
                output[i] = Math.Exp(a);
            }
            double sum = output.Sum();
            for (int i = 0; i < output.Count(); i++)
            {
                output[i] = output[i] / sum;
            }
            return output;
        }
        public static double[] InverseSoftMax(double[] dvalues, double[] outputs, WriteToCMDLine write)
        {
            double[] input = new double[dvalues.Count()];
            double[,] diag = outputs.DiagFlat();
            double[,] Jacobian = new double[diag.GetLength(0), diag.GetLength(1)];
            
            Parallel.For(0, Jacobian.GetLength(0), i =>
                         {
                             Parallel.For(0, Jacobian.GetLength(1), j =>
                                          {
                                              Jacobian[i, j] = outputs[i] * outputs[j];
                                          });
                         });
            
            double[,] Fin = new double[Jacobian.GetLength(0), Jacobian.GetLength(1)];          
            Parallel.For(0, Jacobian.GetLength(0), i =>
                         {
                             Parallel.For(0, Jacobian.GetLength(1), j =>
                                          {
                                              Fin[i, j] = diag[i, j] - Jacobian[i, j];
                                          });
                         });
            
            for(int i = 0; i < input.Count(); i++)
            {
                for(int j = 0; j < Fin.GetLength(1); j++)
                {
                    input[i] += dvalues[j] * Fin[i, j];
                }
            }
            for(int i = 0; i < diag.Length(0); i++)
            {
                diag.GetRank(i).WriteArray("diag " + i, write);
            }
            dvalues.WriteArray("DValues", write);
            dvalues.WriteArray("Inputs", write);
            return input;
            /*
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
            */
        }
        public static double[] InverseCombinedCrossEntropySoftmax(double[] dvalues, double[] outputs)
        {
            double[] dinputs = new double[dvalues.Count()];
            int max = dvalues.ToList().IndexOf(dvalues.Max());
            for (int i = 0; i < dinputs.Count(); i++)
            {
                dinputs[i] = outputs[i];
            }
            dinputs[max] -= 1;
            return dinputs;
        }
        public static double[] Tangential(double[] input)
        {
            double[] output = new double[input.Count()];
            for(int i = 0; i < output.Count(); i++)
            {
                double a = Math.Exp(input[i]);
                double b = Math.Exp(-1 * input[i]);
                output[i] = (a - b) / (a + b);
            }
            return output;
        }
        public static double[] InverseTangential(double[] dvalues, double[] outputs)
        {
            double[] result = new double[outputs.Count()];
            for (int i = 0; i < outputs.Count(); i++)
            {
                result[i] = dvalues[i] * (1 - (outputs[i] * outputs[i]));
            }
            return result;
        }
    }
}

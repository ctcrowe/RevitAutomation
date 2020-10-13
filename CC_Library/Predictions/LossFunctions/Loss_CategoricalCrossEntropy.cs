using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class CategoricalCrossEntropy
    {
        public static double[] Forward(double[] predicted, double[] actual)
        {
            double[] Clipped = new double[predicted.Count()];
            double[] Result = new double[Clipped.Count()];
            for (int j = 0; j < predicted.Count(); j++)
            {
                if (predicted[j] <= 0)
                    Clipped[j] = 1e-9;
                else
                {
                    if (predicted[j] >= 1)
                        Clipped[j] = 1 - 1e-9;
                    else
                        Clipped[j] = predicted[j];
                }
            }
            Result[actual.ToList().IndexOf(actual.Max())] = -1 * Math.Log(Clipped[actual.ToList().IndexOf(actual.Max())]);
            return Result;
        }
        public static double[] Backward(double[] DValues, double[] actual)
        {
            int Labels = DValues.Length;

            double[] dinput = new double[DValues.Length];
            dinput[actual.ToList().IndexOf(actual.Max())] = -1 * actual.Max() / DValues[actual.ToList().IndexOf(actual.Max())];

            return dinput;
        }
        public static double MeanLoss(double[] F)
        {
            double loss = 0;
            for(int i = 0; i < F.Count(); i++)
            {
                loss += F[i];
            }
            return loss;
        }
    }
}

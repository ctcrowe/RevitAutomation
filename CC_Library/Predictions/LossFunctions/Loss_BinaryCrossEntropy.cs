using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class BinaryCrossEntropy
    {
        public static double[] Forward(double[] predicted, double[] actual)
        {
            double[] result = new double[predicted.Count()];
            int amax = actual.ToList().IndexOf(actual.Max());
            double[] clipped = predicted.Clipped();
            for(int i = 0; i < clipped.Count(); i++)
            {
                double a = actual[i] * Math.Log(clipped[i]);
                double b = (1 - actual[i]) * Math.Log(1 - clipped[i]);
                result[i] = -1 * (a + b);
            }
            return result;
        }
        public static double[] Backward(double[] DValues, double[] actual)
        {
            int amax = actual.ToList().IndexOf(actual.Max());
            int count = DValues.Count();
            double[] clipped = DValues.Clipped();
            double[] dinput = new double[DValues.Length];

            for(int i = 0; i < clipped.Count(); i++)
            {
                double a = actual[i] / clipped[i];
                double b = (1 - actual[i]) / (1 - clipped[i]);
                dinput[i] = -1 * (a - b) / count;
            }
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

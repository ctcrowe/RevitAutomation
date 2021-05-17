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
            double[] Result = new double[predicted.Count()];
            int amax = actual.ToList().IndexOf(actual.Max());
            double pmax = 1;
            double min = 1e-6;
            double max = 1 - min;
            if (predicted[amax] <= min)
                pmax = min;
            else
            {
                if (predicted[amax] >= max)
                    pmax = max;
                else
                    pmax = predicted[amax];
            }
            var afin = -1 * Math.Log(pmax);
            Result[amax] = afin;
            return Result;
        }
        public static double[] Backward(double[] dvals, double[] actual)
        {
            double[] dinput = new double[dvals.Length];
            double[] dvalues = dvals.Clipped();
            dinput[actual.ToList().IndexOf(actual.Max())] = -1 / dvalues[actual.ToList().IndexOf(actual.Max())];

            return dinput;
        }
    }
}

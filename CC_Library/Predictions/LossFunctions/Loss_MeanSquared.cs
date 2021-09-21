using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class MeanSquared
    {
        public static double[] Forward(double[] predicted, double[] actual)
        {
            double[] Result = new double[predicted.Count()];
            for (int i = 0; i < predicted.Count(); i++)
            {
                var x = Math.Abs(actual[i] - predicted[i]);
                var y = Math.Pow(x, 2);
                Result[i] = y / 2;
            }
            return Result;
        }
        public static double[] Backward(double[] DValues, double[] actual)
        {
            double[] dinput = new double[DValues.Length];
            for(int i = 0; i < DValues.Length; i++)
            {
                dinput[i] = -2 * (actual[i] - DValues[i]);
                dinput[i] /= dinput.Count();
            }

            return dinput;
        }
    }
}

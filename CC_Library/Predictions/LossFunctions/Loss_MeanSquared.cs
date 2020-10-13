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
                Result[i] = (1 / 2) * Math.Pow((predicted[i] - actual[i]), 2);
            }
            return Result;
        }
        public static double[] Backward(double[] DValues, double[] actual)
        {
            double[] dinput = new double[DValues.Length];
            for(int i = 0; i < DValues.Length; i++)
            {
                dinput[i] = -2 * (DValues[i] - actual[i]);
            }

            return dinput;
        }
    }
}

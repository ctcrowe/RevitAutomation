using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal enum CostFunction
    {
        MeanSquared,
        CrossEntropy
    }
    internal static class Cost
    {
        public static double[] MeanSquared(this double[] outputs, double[] desired)
        {
            double[] result = new double[outputs.Count()];
            for(int i = 0; i < outputs.Count(); i++)
            {
                result[i] = Math.Pow(outputs[i] - desired[i], 2) / 2;
            }
            return result;
        }
        public static double[] DMeanSquared(this double[] outputs, double[] desired)
        {
            double[] result = new double[outputs.Count()];
            for(int i = 0; i < outputs.Count(); i++)
            {
                result[i] = desired[i] - outputs[i];
            }
            return result;
        }
        public static double[] CrossEntropy(this double[] outputs, double[] desired)
        {
            double[] result = new double[outputs.Count()];
            for (int i = 0; i < outputs.Count(); i++)
            {
                if (desired[i] == 0)
                {
                    if (outputs[i] < 1)
                        result[i] = -1 * Math.Log(1 - outputs[i]);
                    else
                        result[i] = 0;
                }
                else
                {
                    if (outputs[i] > 0)
                        result[i] = -1 * desired[i] * Math.Log(outputs[i]);
                    else
                        result[i] = 0;
                }
            }
            return result;
        }
        public static double[] DCrossEntropy(this double[] outputs, double[] desired)
        {
            double[] result = new double[outputs.Count()];
            for (int i = 0; i < outputs.Count(); i++)
            {
                result[i] = (outputs[i] - desired[i]) / ((outputs[i] + 1) * outputs[i]);
            }
            return result;
        }
    }
}
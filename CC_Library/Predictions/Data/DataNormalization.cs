using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class DataNormalization
    {
        public static double[] NormalizeVector(this double[] values)
        {
            double[] newvalues = new double[values.Count()];
            double length = values.CalcDistance();
            for (int i = 0; i < values.Count(); i++)
            {
                newvalues[i] = values[i] / length;
            }
            return newvalues;
        }
    }
}

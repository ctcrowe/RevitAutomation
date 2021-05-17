using System.Linq;

namespace CC_Library
{
    internal static class Array_Clipped
    {
        public static double[] Clipped(this double[] X)
        {
            double[] result = new double[X.Count()];
            for(int i = 0; i < X.Count(); i++)
            {
                if (X[i] <= 0)
                    result[i] = 1e-7;
                else
                {
                    if (X[i] >= 1)
                        result[i] = 1 - (1e-7);
                    else
                        result[i] = X[i];
                }
            }
            return result;
        }
    }
}

using System.Linq;

namespace CC_Library
{
    internal static class Array_DiagFlat
    {
        public static double[,] DiagFlat(this double[] X)
        {
            double[,] result = new double[X.Count(), X.Count()];
            for(int i = 0; i < X.Count(); i++)
            {
                result[i, i] = X[i];
            }
            return result;
        }
    }
}

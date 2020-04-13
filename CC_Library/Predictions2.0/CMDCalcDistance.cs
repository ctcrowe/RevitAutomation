using System;

namespace CC_Library.Predictions
{
    internal static class CMDCalcDistance
    {
        public static double CalcDistance(this DataPt d, DataPt r)
        {
            double val = 0;
            for(int i = 0; i < 20; i++)
            {
                double a = d.GetValue(i) - r.GetValue(i);
                double b = Math.Pow(a, 2);
                val += b;
            }
            double fin = Math.Sqrt(val);
            return fin;
        }
    }
}
using System;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class PredictionDistance
    {
        public static double Distance(this Element ele1, Element ele2)
        {
            double loc = 0;
            if (ele2.datatype == Datatype.Dictionary)
            {
                for (int i = 0; i < ele1.Location.Count(); i++)
                {
                    double a = ele2.Location[i] - ele1.Location[i];
                    double b = Math.Pow(a, 2);
                    loc += b;
                }
            }
            else
            {
                for (int i = 0; i < ele1.Location.Count(); i++)
                {
                    double a = ele2.Location[i] - ele1.Location[i];
                    double b = Math.Pow(a, 2);
                    loc += b;
                }
            }
            double fin = Math.Sqrt(loc);
            return fin;
        }
        public static double Distance(this Element ele, double[] Point)
        {
            double loc = 0;
            for (int i = 0; i < ele.Location.Count(); i++)
            {
                double a = Point[i] - ele.Location[i];
                double b = Math.Pow(a, 2);
                loc += b;
            }
            double fin = Math.Sqrt(loc);
            return fin;
        }
    }
}

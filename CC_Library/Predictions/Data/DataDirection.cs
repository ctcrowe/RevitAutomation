using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class DataDirection
    {
        public static double[] DirectionBetween(this KeyValuePair<string, Data> point1, KeyValuePair<string, Data> point2)
        {
            int sizecount = 0;
            if (point1.Value.Datatype.Count() > point2.Value.Datatype.Count())
                sizecount = point2.Value.Datatype.Count();
            else
                sizecount = point1.Value.Datatype.Count();
            double[] SpaceBetween = new double[sizecount];

            for (int i = 0; i < sizecount; i++)
            {
                SpaceBetween[i] = point2.Value.Location[i] - point1.Value.Location[i];
            }
            return SpaceBetween;
        }
        public static double[] DirectionBetween(this double[] point1, double[] point2)
        {
            int sizecount = 0;
            if (point1.Count() > point2.Count())
                sizecount = point2.Count();
            else
                sizecount = point1.Count();
            double[] SpaceBetween = new double[sizecount];

            for (int i = 0; i < sizecount; i++)
            {
                SpaceBetween[i] = point2[i] - point1[i];
            }
            return SpaceBetween;
        }
    }
}

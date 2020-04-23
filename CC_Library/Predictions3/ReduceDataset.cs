using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class ReduceDataset
    {
        public static void Reduce(this Dataset dataset)
        {
            double max = 0;
            foreach(KeyValuePair<string, double[]> Data in dataset.Data)
            {
                foreach(double d in Data.Value)
                {
                    if(Math.Abs(d) > max)
                    {
                        max = Math.Abs(d);
                    }
                }
            }
            foreach (KeyValuePair<string, double[]> Data in dataset.Data)
            {
                for (int i = 0; i < Data.Value.Count(); i++)
                {
                    Data.Value[i] /= max;
                }
            }
        }
    }
}

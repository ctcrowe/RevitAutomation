using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class ElevationOutput
    {
        public static double[] ElevInput(this Entry e, List<string> Words)
        {
            double[] input = new double[200];
            for(int i = 0; i < e.Keys.Count(); i++)
            {
                input[i] = double.Parse(e.Keys[i]);
            }
            return input;
        }
        public static double[] ElevOutput(this Entry e, List<string> Words)
        {
            double[] output = new double[50];
            for (int i = 0; i < e.Values.Count(); i++)
            {
                output[i] = double.Parse(e.Values[i]);
            }
            return output;
        }
    }
}

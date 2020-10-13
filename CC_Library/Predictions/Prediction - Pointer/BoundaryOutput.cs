using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class PointerOutput
    {
        public static double[] PInput(this Entry e, List<string> Words)
        {
            double[] input = new double[15];
            for(int i = 0; i < e.Keys.Count(); i++)
            {
                input[i] = double.Parse(e.Keys[i]);
            }
            return input;
        }
        public static double[] POutput(this Entry e, List<string> Words)
        {
            double[] vals = new double[2];
            if (e.Values[0] == "Y")
                vals[1] = 1;
            else
                vals[0] = 1;
            return vals;
        }
    }
}

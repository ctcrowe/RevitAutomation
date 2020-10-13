using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class DictionaryOutput
    {
        public static double[] DictInput(this Entry e, List<string> Words)
        {
            var input = e.Keys[0];
            double[] result = new double[Words.Count()];
            result[Words.IndexOf(input)] = 1;
            return result;
        }
        public static double[] DictOutput(this Entry input, List<string> Words)
        {
            double[] result = new double[Words.Count()];
            result[Words.IndexOf(input.Values[0])] = 1;
            return result;
        }
    }
}

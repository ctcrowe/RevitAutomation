using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class MasterformatOutput
    {
        public static double[] MFInput(this Entry e, List<string> Words)
        {
            double[] input = new double[CustomNeuralNet.DictSize];
            var words = e.Keys[0].SplitTitle();
            if (words.Any())
            {
                for (int i = 0; i < words.Count(); i++)
                {
                    for (int j = 0; j < input.Count(); j++)
                    {
                        input[j] += Datatype.Dictionary.GetElement(words[i]).Location[j];
                    }
                }
            }
            return input;
        }
        public static double[] MFOutput(this Entry e, List<string> Words)
        {
            double[] o = new double[40];
            int resultnumb = int.Parse(e.Values[0]);
            for (int i = 0; i < o.Count(); i++)
            {
                o[i] = 0;
            }
            o[resultnumb] = 1;
            return o;
        }
    }
}

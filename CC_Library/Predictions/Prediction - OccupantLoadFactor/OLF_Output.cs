using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class OLF_Output
    {
        public static double[] OLFInput(this Entry e, List<string> Words)
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
        public static double[] OLFOutput(this Entry e, List<string> Words)
        {
            double[] o = new double[16];
            int resultnumb = 10;
            switch(int.Parse(e.Values[0]))
            {
                case 5:
                    resultnumb = 0;
                    break;
                case 7:
                    resultnumb = 1;
                    break;
                case 11:
                    resultnumb = 2;
                    break;
                case 15:
                    resultnumb = 3;
                    break;
                case 20:
                    resultnumb = 4;
                    break;
                case 30:
                    resultnumb = 5;
                    break;
                case 35:
                    resultnumb = 6;
                    break;
                case 40:
                    resultnumb = 7;
                    break;
                case 50:
                    resultnumb = 8;
                    break;
                case 60:
                    resultnumb = 9;
                    break;
                default:
                case 100:
                    resultnumb = 10;
                    break;
                case 120:
                    resultnumb = 11;
                    break;
                case 200:
                    resultnumb = 12;
                    break;
                case 240:
                    resultnumb = 13;
                    break;
                case 300:
                    resultnumb = 14;
                    break;
                case 500:
                    resultnumb = 15;
                    break;
            }
            for (int i = 0; i < o.Count(); i++)
            {
                o[i] = 0;
            }
            o[resultnumb] = 1;
            return o;
        }
    }
}

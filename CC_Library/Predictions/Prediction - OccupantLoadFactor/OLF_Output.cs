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
            var words = e.Keys[0].GetWords();
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
                case 0:
                    resultnumb = 0;
                    break;
                case 5:
                    resultnumb = 1;
                    break;
                case 7:
                    resultnumb = 2;
                    break;
                case 11:
                    resultnumb = 3;
                    break;
                case 15:
                    resultnumb = 4;
                    break;
                case 20:
                    resultnumb = 5;
                    break;
                case 30:
                    resultnumb = 6;
                    break;
                case 35:
                    resultnumb = 7;
                    break;
                case 40:
                    resultnumb = 8;
                    break;
                case 50:
                    resultnumb = 9;
                    break;
                case 60:
                    resultnumb = 10;
                    break;
                default:
                case 100:
                    resultnumb = 11;
                    break;
                case 120:
                    resultnumb = 12;
                    break;
                case 200:
                    resultnumb = 13;
                    break;
                case 240:
                    resultnumb = 14;
                    break;
                case 300:
                    resultnumb = 15;
                    break;
                case 500:
                    resultnumb = 16;
                    break;
            }
            for (int i = 0; i < o.Count(); i++)
            {
                o[i] = 0;
            }
            o[resultnumb] = 1;
            return o;
        }
        public static int ReverseOLFOutput(this int input)
        {
            switch(input)
            {
                case 0:
                    return 0;
                case 1:
                    return 5;
                case 2:
                    return 7;
                case 3:
                    return 11;
                case 4:
                    return 15;
                case 5:
                    return 20;
                case 6:
                    return 30;
                case 7:
                    return 35;
                case 8:
                    return 40;
                case 9:
                    return 50;
                case 10:
                    return 60;
                case 11:
                default:
                    return 100;
                case 12:
                    return 120;
                case 13:
                    return 200;
                case 14:
                    return 240;
                case 15:
                    return 300;
                case 16:
                    return 500;
            }
        }
    }
}

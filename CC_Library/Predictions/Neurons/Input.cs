using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class Input
    {
        public static double[] GetInput(this List<string> input)
        {
            double[] result = new double[CustomNeuralNet.DictSize];
            for(int i = 0; i < input.Count(); i++)
            {
                Element e = Datatype.Dictionary.GetElement(input[i]);
                for (int j = 0; j < CustomNeuralNet.DictSize; j++)
                {
                    result[j] += e.Location[j];
                }
            }
            return result;
        }
        /*
        public static double[] GetOutput(this Entry input, Datatype datatype)
        {
            double[] result = new double[8];
            for(int i = 0; i < input.Values.Count(); i++)
            {
                var ele = datatype.GetElement(input.Values[i]);
                for(int j = 0; j < 8; j++)
                {
                    result[j] += ele.Location[j];
                }
            }
            for (int i = 0; i < 8; i++)
            {
                result[i] /= input.Values.Count();
            }
            return result;
        }*/
    }
}

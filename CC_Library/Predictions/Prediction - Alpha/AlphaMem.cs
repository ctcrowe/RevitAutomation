using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using CC_Library.Datatypes;
using System.Reflection;

/// <summary>
/// 2 Vectors
/// 1) Sparce Context Vector (input is 2 * Chars.Count() + 1)
///     This last input will be the distance between them.
/// 2) Actual location of characters
///     This will be DictSize and represents the N Dimensional encoding of the letter where N is the DictSize in the class.

namespace CC_Library.Predictions
{
    internal class AlphaMem
    {
        public List<double[,]>[] LocationOutputs { get; set; }
        public List<double[,]>[] LocalContextOutputs { get; set; }
        public double[] GlobalContextOutputs { get; set; }

        public AlphaMem(int length)
        {
            LocationOutputs = new List<double[,]>[length]; // Old = PhraseLength, DictSize
            LocalContextOutputs = new List<double[,]>[length];
            GlobalContextOutputs = new double[length];

            Parallel.For(0, LocalContextOutputs.Count(), j => LocalContextOutputs[j] = new List<double[,]>());
            Parallel.For(0, LocationOutputs.Count(), j => LocationOutputs[j] = new List<double[,]>());
        }
        public double[] DGlobalContext(double[] dvalues)
        {
            double[] result = new double[LocationOutputs.Count()];
            for (int i = 0; i < result.Count(); i++)
            {
                for (int j = 0; j < dvalues.Count(); j++)
                {
                    if (LocationOutputs[i].Any())
                        result[i] += dvalues[j] * LocationOutputs[i].Last()[1, j];
                }
            }
            return result;
        }
        //Location is multiplied by the softmax global context.
        //GlobalContext is the comparison of all of the localcontexts put together into one array.
        //The derivative of the Location is going to be relative to the global context softmax value and the derivatives of the output of the next layer
        // IE Dvalues and global context outputs.
        public List<double[]> DLocation(double[] dvalues)
        {
            List<double[]> result = new List<double[]>();
            for (int i = 0; i < GlobalContextOutputs.Count(); i++)
            {
                double[] res = new double[dvalues.Count()];
                for (int j = 0; j < res.Count(); j++)
                {
                    res[j] = dvalues[j] * GlobalContextOutputs[i];
                }
                result.Add(res);
            }
            return result;
        }
    }
}
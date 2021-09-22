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
    internal class StonkMem
    {
        public List<double[]>[] LocationOutputs { get; set; }
        public List<double[]>[] LocalContextOutputs { get; set; }
        public double[] GlobalContextOutputs { get; set; }

        public StonkMem(char[] Phrase)
        {
            LocationOutputs = new List<double[]>[Phrase.Count()]; // Old = PhraseLength, DictSize
            LocalContextOutputs = new List<double[]>[Phrase.Count()];
            GlobalContextOutputs = new double[Phrase.Count()];

            Parallel.For(0, LocalContextOutputs.Count(), j => LocalContextOutputs[j] = new List<double[]>());
            Parallel.For(0, LocationOutputs.Count(), j => LocationOutputs[j] = new List<double[]>());
        }
        public double[] DGlobalContext(double[] dvalues)
        {
            double[] result = new double[LocationOutputs.Count()];
            for (int i = 0; i < result.Count(); i++)
            {
                for (int j = 0; j < Alpha.DictSize; j++)
                {
                    if (LocationOutputs[i].Any())
                        result[i] += dvalues[j] * LocationOutputs[i].Last()[j];
                }
            }
            return result;
        }
        public List<double[]> DLocation(double[] dvalues)
        {
            List<double[]> result = new List<double[]>();
            for (int i = 0; i < GlobalContextOutputs.Count(); i++)
            {
                double[] res = new double[Alpha.DictSize];
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
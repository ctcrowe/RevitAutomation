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
    public class StonkMem
    {
        public List<List<double[]>> LocationOutputs { get; set; }
        public List<List<double[]>> LocalContextOutputs { get; set; }
        public double[] GlobalOutputs { get; set; }

        public StonkMem()
        {
            LocationOutputs = new List<List<double[]>>();
            LocalContextOutputs = new List<List<double[]>>();
            GlobalOutputs = new double[0];

            Parallel.For(0, LocalContextOutputs.Count(), j => LocalContextOutputs[j] = new List<double[]>());
            Parallel.For(0, LocationOutputs.Count(), j => LocationOutputs[j] = new List<double[]>());
        }
        public double[] Multiply()
        {
            if(LocationOutputs.Count() == LocalContextOutputs.Count())
            {
                double[] output = new double[LocationOutputs.Last().Last().Count()];
                double[] ctxt = new double[LocalContextOutputs.Count()];
                Parallel.For(0, ctxt.Count(), j => ctxt[j] = LocalContextOutputs[j].Last().First());
                this.GlobalOutputs = Activations.SoftMax(ctxt);
                Parallel.For(0, LocationOutputs.Count(), j =>
                {
                    for (int i = 0; i < output.Count(); i++)
                    {
                        output[i] = LocationOutputs[j].Last()[i] * GlobalOutputs[j];
                    }
                });
            }
            return null;
        }
        public double[] DGlobalContext(double[] dvalues)
        {
            double[] result = new double[LocationOutputs.Count()];
            for (int i = 0; i < result.Count(); i++)
            {
                for (int j = 0; j < Stonk.MktSize; j++)
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
            for (int i = 0; i < GlobalOutputs.Count(); i++)
            {
                double[] res = new double[Stonk.MktSize];
                for (int j = 0; j < res.Count(); j++)
                {
                    res[j] = dvalues[j] * GlobalOutputs[i];
                }
                result.Add(res);
            }
            return result;
        }
    }
}

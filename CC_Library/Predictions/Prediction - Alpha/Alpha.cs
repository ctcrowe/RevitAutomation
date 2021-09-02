using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class CharSet
    {
        public const int CharCount = 38;
        private static char[] Chars = new char[38] {
            'A', 'B', 'C', 'D', 'E', 'F', 'G',
            'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z', '0', '1',
            '2', '3', '4', '5', '6', '7', '8',
            '9', ' ', '_'};
        
        public static double[] Locate(this string s, int numb, int range)
        {
            double[] result = new double[CharCount() * ((2 * range) + 1)];
            string a = s.ToUpper();
            char[] chars = a.ToCharArray();
            
            int imin = numb < range? range - (range - numb) : range;
            int imax = numb + range < chars.Count()? range : range - ((numb + range) - chars.Count());
            
            result[LocationOf(phrase[numb])] = 1;
            Parallel.For(1, imin + 1, i => result[(i * CharCount) + LocationOf(chars[numb - i])] = 1);
            Parallel.For(1, imax + 1, i => result[((range + i) * CharCount()) + LocationOf(chars[numb + i])] = 1);

            return result;
        }
        private static int LocationOf(char c) { return Chars.Contains(c)? Chars.IndexOf(c) : Chars.Count() - 1; }
    }
    internal class Alpha
    {
        internal Alpha()
        {
            Network = Datatype.Alpha.LoadNetwork();
            this.Results = new List<double[]>();
        }
        
        public List<double[]> Results { get; set; }
        public const int DictSize = 100;
        public const int SearchRange = 3;
        public NeuralNetwork Network { get; }
        
        public double[] Forward(string s, AlphaContext context)
        {
            double[] ctxt = new double[chars.Count()];
            double[,] loc = new double[chars.Count(), DictSize];
            
            Parallel.For(0, chars.Count(), j =>
            {
                double[] a = s.Locate(j, SearchRange);
                for (int i = 0; i < Network.Layers.Count(); i++) { a = Network.Layers[i].Output(a); }
                loc.SetRank(a, j);
                ctxt[j] = context.Contextualize(chars, j);
            });
            
            return loc.Multiply(Activations.SoftMax(ctxt));
        }
        public double[] Forward(string s, AlphaContext context, AlphaMem am)
        {
            double[] ctxt = new double[chars.Count()];
            double[,] loc = new double[chars.Count(), DictSize];
            
            Parallel.For(0, chars.Count(), j =>
            {
                double[] a = s.Locate(j, SearchRange);
                am.LocationOutputs[numb].Add(a);
                for (int i = 0; i < Network.Layers.Count(); i++)
                {
                    a = Network.Layers[i].Output(a);
                    am.LocationOutputs[numb].Add(a);
                }
                loc.SetRank(a, j);
                ctxt[j] = context.Contextualize(chars, j, am);
            });
            return loc.Multiply(Activations.SoftMax(ctxt));
        }
        public void Backward(string s, double[] DValues, AlphaContext context, AlphaMem am, NetworkMem mem, NetworkMem CtxtMem)
        {
            char[] chars = GetChars(s);
            var LocDValues = am.DLocation(DValues);
            DValues = am.DGlobalContext(DValues);
            DValues = Activations.InverseSoftMax(DValues, am.GlobalContextOutputs);
            context.Backward(DValues, chars.Count(), am, CtxtMem);
            Parallel.For(0, chars.Count(), j =>
            {
                var ldv = LocDValues[j];
                for (int i = Network.Layers.Count() - 1; i >= 0; i--)
                {
                    ldv = mem.Layers[i].DActivation(ldv, am.LocationOutputs[j][i + 1]);
                    mem.Layers[i].DBiases(ldv);
                    mem.Layers[i].DWeights(ldv, am.LocationOutputs[j][i]);
                    ldv = mem.Layers[i].DInputs(ldv, Network.Layers[i]);
                }
            });
        }
    }
}

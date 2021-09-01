using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;

/// <summary>
/// 2 Vectors
/// 1) Sparce Context Vector (input is 2 * Chars.Count() + 1)
///     This last input will be the distance between them.
/// 2) Actual location of characters
///     This will be DictSize and represents the N Dimensional encoding of the letter where N is the DictSize in the class.

//New complexity: Element Location as Input
//This will require an attention mechanism and name input
//Attention input can be same as element input - 1 input fed into 2 hidden layers
//The input will be the name in 100D space, followed by the location (start XY, end XY)
//if the location is a single point, location is (start XY, start XY)
//One outputs a position in 100 dimensional space
//The second outputs a focus value - an interpretation of the significance of that location...

namespace CC_Library.Predictions
{
    internal class Alpha
    {
        internal Alpha()
        {
            Network = Datatype.Alpha.LoadNetwork();
            this.Results = new List<double[]>();
        }
        public List<double[]> Results { get; set; }
        public const int DictSize = 100;
        public const int SearchSize = 4;
        public static int CharCount() { return Chars.Count(); }
        private static List<char> Chars = new List<char>() {
            'A', 'B', 'C', 'D', 'E', 'F', 'G',
            'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z', '0', '1',
            '2', '3', '4', '5', '6', '7', '8',
            '9', ' ', '_'};
        public double[] Forward(string s, AlphaContext context)
        {
            char[] chars = GetChars(s);
            double[] ctxt = new double[chars.Count()];
            
            double[,] loc = new double[chars.Count(), DictSize];
            Parallel.For(0, chars.Count(), j =>
            {
                var location = Locate(chars, j);
                loc.SetRank(location, j);
                //ctxt1[j] = AlphaSender.Contextualize(chars, j);
                //ctxt2[j] = AlphaObject.Contextualize(chars, j);
                //ctxt3[j] = AlphaReceiver.Contextualize(chars, j);
                //ctxt4[j] = AlphaAction.Contextualize(chars, j);
                ctxt[j] = context.Contextualize(chars, j);
            });
            var output = Multiply(loc, Activations.SoftMax(ctxt));
            return output;
        }
        public double[] Forward(string s, AlphaContext context, AlphaMem am)
        {
            char[] chars = GetChars(s);
            double[] ctxt = new double[chars.Count()];
            double[,] loc = new double[chars.Count(), DictSize];
            Parallel.For(0, chars.Count(), j =>
            {
                var location = Locate(chars, am, j);
                loc.SetRank(location, j);
                ctxt[j] = context.Contextualize(chars, j, am);
            });
            am.GlobalContextOutputs = Activations.SoftMax(ctxt);
            var output = Multiply(loc, am.GlobalContextOutputs);
            return output;
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
        private double[] Locate(char[] c, AlphaMem am, int numb)
        {
            double[] a = GetLocation(c, numb);
            am.LocationOutputs[numb].Add(a);

            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                a = Network.Layers[i].Output(a);
                am.LocationOutputs[numb].Add(a);
            }

            return a;
        }
        private double[] Locate(char[] c, int numb)
        {
            double[] a = GetLocation(c, numb);
            for (int i = 0; i < Network.Layers.Count(); i++)
            {
                a = Network.Layers[i].Output(a);
            }
            return a;
        }
        public NeuralNetwork Network { get; }
        private static char[] GetChars(string s)
        {
            string a = s.ToUpper();
            return a.ToCharArray();
        }
        public static int LocationOf(char c) { return Chars.Contains(c)? Chars.IndexOf(c) : Chars.Count() - 1; }
        private static double[] GetLocation(char[] c, int numb)
        {
            double[] a = new double[3 * Chars.Count()];
            if (numb > 0)
            {
                a[Chars.Count() + LocationOf(c[numb - 1])] = 1;
            }
            a[Chars.Count() + LocationOf(c[numb])] = 1;
            if (numb < c.Count() - 1)
            {
                a[(2 * Chars.Count()) + LocationOf(c[numb + 1])] = 1;
            }
            return a;
        }
        private static double[] Multiply(double[,] a, double[] b)
        {
            double[] c = new double[a.GetLength(1)];
            if (a.GetLength(0) == b.GetLength(0))
            {
                for (int i = 0; i < a.GetLength(0); i++)
                {
                    for (int j = 0; j < a.GetLength(1); j++)
                    {
                        c[j] += a[i, j] * b[i];
                    }
                }
            }
            return c;
        }
    }
}

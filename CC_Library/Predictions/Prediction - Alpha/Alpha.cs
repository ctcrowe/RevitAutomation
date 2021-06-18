﻿using System;
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
namespace CC_Library.Predictions
{
    internal class Alpha
    {
        internal Alpha(WriteToCMDLine write)
        {
            Location = Datatype.Alpha.LoadNetwork(write);
        }
        public const int DictSize = 100;
        public const int SearchSize = 20;
        public static int CharCount() { return Chars.Count(); }
        private static List<char> Chars = new List<char>() {
            'A', 'B', 'C', 'D', 'E', 'F', 'G',
            'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z', '0', '1',
            '2', '3', '4', '5', '6', '7', '8',
            '9', ' ', '_'};

        public static double[] Predict(Datatype dt, string s)
        {
            Alpha a = new Alpha(Delegates.WriteNull);
            LocalContext context = new LocalContext(dt, Delegates.WriteNull);
            AlphaMem am = new AlphaMem(s.ToCharArray());
            return a.Forward(s, context, am, Delegates.WriteNull);
        }
        public double[] Forward(string s, LocalContext context, AlphaMem am, WriteToCMDLine write)
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
        public void Backward(string s, double[] DValues, LocalContext context, AlphaMem am, WriteToCMDLine write)
        {
            char[] chars = GetChars(s);
            var LocDValues = am.DLocation(DValues);
            DValues = am.DGlobalContext(DValues);
            DValues = Activations.InverseSoftMax(DValues, am.GlobalContextOutputs);
            context.Backward(DValues, chars.Count(), am, write);
            Parallel.For(0, chars.Count(), j =>
            {
                var ldv = LocDValues[j];
                for (int i = Location.Layers.Count() - 1; i >= 0; i--)
                {
                    ldv = Location.Layers[i].DActivation(ldv, am.LocationOutputs[j][i + 1]);
                    Location.Layers[i].DBiases(ldv);
                    Location.Layers[i].DWeights(ldv, am.LocationOutputs[j][i]);
                    ldv = Location.Layers[i].DInputs(ldv);
                }
            });
        }
        private double[] Locate(char[] c, AlphaMem am, int numb)
        {
            double[] a = GetLocation(c, numb);
            am.LocationOutputs[numb].Add(a);

            for (int i = 0; i < Location.Layers.Count(); i++)
            {
                a = Location.Layers[i].Output(a);
                am.LocationOutputs[numb].Add(a);
            }

            return a;
        }

        public NeuralNetwork Location { get; }

        public static string GenerateTypo(string input, Random r, int ErrorCount = 2)
        {
            var array = input.ToCharArray().ToList();
            for (int i = 0; i < ErrorCount; i++)
            {
                int c = r.Next(0, Alpha.CharCount() + 10);
                if (c >= Alpha.CharCount())
                    array.RemoveAt(r.Next(0, array.Count()));
                else
                    array[r.Next(0, array.Count())] = Chars[c];
            }
            return new string(array.ToArray());
        }

        private static char[] GetChars(string s)
        {
            string a = s.ToUpper();
            return a.ToCharArray();
        }
        public static int LocationOf(char c)
        {
            if (Chars.Contains(c))
                return Chars.IndexOf(c);
            else
                return Chars.Count() - 1;
        }
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
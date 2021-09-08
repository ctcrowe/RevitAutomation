using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System.Collections.Generic;
using System;

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
            double[] result = new double[CharCount * ((2 * range) + 1)];
            string a = s.ToUpper();
            char[] chars = a.ToCharArray();

            int imin = numb < range ? numb : range;
            int imax = (numb + range) < chars.Count() ? range : chars.Count() - numb;

            Parallel.For(0, imax, i => result[(i * CharCount) + LocationOf(chars[numb + i])] = 1);
            Parallel.For(0, imin, i => result[((range + i) * CharCount) + LocationOf(chars[numb - (i + 1)])] = 1);

            return result;
        }
        private static int LocationOf(char c) { return Chars.Contains(c) ? Chars.ToList().IndexOf(c) : Chars.Count() - 1; }
    }
    public static class TestCharSet
    {
        public static void TestChars(this string s, WriteToCMDLine write)
        {
            try
            {
                var Loc = s.Locate(3, 4);
                string l = "";
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < CharSet.CharCount; j++)
                    {
                        l += Loc[(i * CharSet.CharCount) + j] + ", ";
                    }
                    l += "\r\n";
                }
                write(l);
            }
            catch(Exception e) { e.OutputError(); }
        }
    }
}
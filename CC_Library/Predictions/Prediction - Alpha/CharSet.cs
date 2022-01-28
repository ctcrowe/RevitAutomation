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
        private static Dictionary<char, int> Chars = new Dictionary<char, int> {
            {'A', 1}, {'B', 3}, {'C', 3}, {'D', 2}, {'E', 1}, {'F', 4}, {'G', 2},
            {'H', 4}, {'I', 1}, {'J', 8}, {'K', 5}, {'L', 1}, {'M', 3}, {'N', 1},
            {'O', 1}, {'P', 3}, {'Q', 10}, {'R', 1}, {'S', 1}, {'T', 1}, {'U', 1},
            {'V', 4}, {'W', 4}, {'X', 8}, {'Y', 4}, {'Z', 10}, {'0', 0}, {'1', 0},
            {'2', 0}, {'3', 0}, {'4', 0}, {'5', 0}, {'6', 0}, {'7', 0}, {'8', 0},
            {'9', 0}, {' ', 0}, {'_', 0}};

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
        public static double[] LocateScrabble(this string s, int numb, int range)
        {
            double[] result = new double[CharCount * ((2 * range) + 1)];
            string a = s.ToUpper();
            char[] chars = a.ToCharArray();

            int imin = numb < range ? numb : range;
            int imax = (numb + range) < chars.Count() ? range : chars.Count() - numb;

            Parallel.For(0, imax, i => result[(i * CharCount) + LocationOfS(chars[numb + i])] = 1);
            Parallel.For(0, imin, i => result[((range + i) * CharCount) + LocationOfS(chars[numb - (i + 1)])] = 1);

            return result;
        }
        public static double[] LocateNumbs(this string s, int numb, int range)
        {
            double[] result = new double[CharCount * ((2 * range) + 1)];
            string a = s.ToUpper();
            char[] chars = a.ToCharArray();

            int imin = numb < range ? numb : range;
            int imax = (numb + range) < chars.Count() ? range : chars.Count() - numb;

            Parallel.For(0, imax, i => result[(i * CharCount) + LocationOfN(chars[numb + i])] = 1);
            Parallel.For(0, imin, i => result[((range + i) * CharCount) + LocationOfN(chars[numb - (i + 1)])] = 1);

            return result;
        }
        public static double[] LocateLetters(this string s, int numb, int range)
        {
            double[] result = new double[CharCount * ((2 * range) + 1)];
            string a = s.ToUpper();
            char[] chars = a.ToCharArray();

            int imin = numb < range ? numb : range;
            int imax = (numb + range) < chars.Count() ? range : chars.Count() - numb;

            Parallel.For(0, imax, i => result[(i * CharCount) + LocationOfC(chars[numb + i])] = 1);
            Parallel.For(0, imin, i => result[((range + i) * CharCount) + LocationOfC(chars[numb - (i + 1)])] = 1);

            return result;
        }
        private static int LocationOf(char c) { return Chars.Keys.Contains(c) ? Chars.Keys.ToList().IndexOf(c) : Chars.Keys.Count() - 1; }
        private static int LocationOfS(char c) { return Chars.Keys.Contains(c) ? Chars[c] : 0; }
        private static int LocationOfN(char c) { return int.TryParse(c, out int x) ? x : 0; }
        private static int LocationOfC(char c) { return !Chars.Keys.Contains(c) ? Chars.Count() - 1 :
                                                Chars[c] > 0? Chars.Keys.ToList().IndexOf(c) :
                                                Chars.Keys.Count() - 1; }
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

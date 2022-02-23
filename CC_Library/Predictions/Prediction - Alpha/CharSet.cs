using System.Linq;
using System.Threading.Tasks;
using CC_Library.Datatypes;
using System.Collections.Generic;
using System;
/*
    TODO:
    New Locate Command that uses a network to predict the beginning and end of each word. This will then advance the term by n, where n is the length of the word.
    Ultimately, breaking a phrase down into words. Search Radius will need to be substantially large, potentially 10 characters +/-. This will give us access to a set of pseudo words,
    without having to instantiate a dictionary for prediction purposes, giving more flexibility than a dictionary, but more structure than just letters to determine terms.
    
    This Network needs to be relatively small and quick, to interpret words on the fly fairly efficiently.
    Base Layer 1 size to have search radius 2 and Locate by character.
    Additional Base Layer to have coordintaed search size and locate a set of characters (potentially turns them into something like a syllable.)
    These syllables will then be interpreted into words, starting and ending being highlighted.
*/
namespace CC_Library.Predictions
{
    internal static class CharSet
    {
        public const int CharCount = 38;
        public const int LetterCount = 27;
        public const int NumbCount = 11;
        private static Dictionary<char, int> Chars = new Dictionary<char, int> {
            {'A', 1}, {'B', 3}, {'C', 3}, {'D', 2}, {'E', 1}, {'F', 4}, {'G', 2},
            {'H', 4}, {'I', 1}, {'J', 8}, {'K', 5}, {'L', 1}, {'M', 3}, {'N', 1},
            {'O', 1}, {'P', 3}, {'Q', 10}, {'R', 1}, {'S', 1}, {'T', 1}, {'U', 1},
            {'V', 4}, {'W', 4}, {'X', 8}, {'Y', 4}, {'Z', 10}, {'0', 0}, {'1', 0},
            {'2', 0}, {'3', 0}, {'4', 0}, {'5', 0}, {'6', 0}, {'7', 0}, {'8', 0},
            {'9', 0}, {' ', 0}, {'_', 0}};

        public static double[] Locate(this string s, int numb, int range, bool incl = false)
        {
            double[] result = new double[
                incl ? (CharCount * ((2 * range) + 1)) + 1 :
                CharCount * (2 * range)];
            string a = s.ToUpper();
            char[] chars = a.ToCharArray();

            int imin = numb < range ? numb : range;
            int imax = (numb + range) < chars.Count() ? range : chars.Count() - numb;

            Parallel.For(0, imax, i => result[(i * CharCount) + LocationOf(chars[numb + i])] = 1);
            Parallel.For(0, imin, i => result[((range + i) * CharCount) + LocationOf(chars[numb - (i + 1)])] = 1);
            result[result.Count() - 1] = incl ? numb : result[result.Count() - 1];

            return result;
        }
        public static double[] LocateScrabble(this string s, int numb, int range)
        {
            double[] result = new double[((2 * range) + 1)];
            string a = s.ToUpper();
            char[] chars = a.ToCharArray();

            int imin = numb < range ? numb : range;
            int imax = (numb + range) < chars.Count() ? range : chars.Count() - numb;

            Parallel.For(0, imax, i => { result[i] = LocationOfS(chars[numb + i]); });
            Parallel.For(0, imin, i => { result[range + i] = LocationOfS(chars[numb - (i + 1)]); });

            return result;
        }
        public static double[] LocateNumbs(this string s, int numb, int range)
        {
            double[] result = new double[NumbCount * ((2 * range) + 1)];
            string a = s.ToUpper();
            char[] chars = a.ToCharArray();

            int imin = numb < range ? numb : range;
            int imax = (numb + range) < chars.Count() ? range : chars.Count() - numb;

            Parallel.For(0, imax, i => result[(i * NumbCount) + LocationOfN(chars[numb + i])] = 1);
            Parallel.For(0, imin, i => result[((range + i) * NumbCount) + LocationOfN(chars[numb - (i + 1)])] = 1);

            return result;
        }
        public static double[] LocateLetters(this string s, int numb, int range)
        {
            double[] result = new double[LetterCount * ((2 * range) + 1)];
            string a = s.ToUpper();
            char[] chars = a.ToCharArray();

            int imin = numb < range ? numb : range;
            int imax = (numb + range) < chars.Count() ? range : chars.Count() - numb;

            Parallel.For(0, imax, i =>
            {
                if(LocationOfC(chars[numb + i]) >= 0)
                    result[(i * LetterCount) + LocationOfC(chars[numb + i])] = 1;
            });
            Parallel.For(0, imin, i =>
            {
                if (LocationOfC(chars[numb - (i + 1)]) >= 0)
                    result[((range + i) * LetterCount) + LocationOfC(chars[numb - (i + 1)])] = 1;
            });

            return result;
        }
        public static double[] LocateOrder(this string s, int numb, int range)
        {
            double[] result = new double[((2 * range) + 1)];
            string a = s.ToUpper();
            char[] chars = a.ToCharArray();

            int imin = numb < range ? numb : range;
            int imax = (numb + range) < chars.Count() ? range : chars.Count() - numb;

            Parallel.For(0, imax, i => { result[i] = numb + i + 1; });
            Parallel.For(0, imin, i => { result[range + i] = numb - i; });

            return result;
        }
        public static double[] LocatePercent(this string s, int numb, int range)
        {
            double[] result = new double[((2 * range) + 1)];
            string a = s.ToUpper();
            char[] chars = a.ToCharArray();

            int imin = numb < range ? numb : range;
            int imax = (numb + range) < chars.Count() ? range : chars.Count() - numb;

            Parallel.For(0, imax, i => { result[i] = 1.0 * (numb + i + 1.0) / chars.Length; });
            Parallel.For(0, imin, i => { result[range + i] = 1.0 * (numb - i) / chars.Length; });

            return result;
        }
        public static double[] LocatePhrase(this string s, int numb, int range)
        {
            double[] result = new double[LetterCount * ((2 * range) + 1)];
            string a = s.ToUpper();
            char[] chars = a.ToCharArray();

            int imin = numb < range ? numb : range;
            int imax = (numb + range) < chars.Count() ? range : chars.Count() - numb;

            for(int i = 0; i < imax; i++)
            {
                var index = LocationOfC(chars[numb + i]);
                if (index >= 0)
                    result[(i * LetterCount) + index] = 1;
                else
                    break;
            }
            for (int i = 0; i < imin; i++)
            {
                var index = LocationOfC(chars[numb - (i + 1)]);
                if (index >= 0)
                    result[((range + i) * LetterCount) + index] = 1;
                else
                    break;
            }
            return result;
        }
        private static int LocationOf(char c) { return Chars.Keys.Contains(c) ? Chars.Keys.ToList().IndexOf(c) : CharCount - 1; }
        private static int LocationOfS(char c) { return Chars.Keys.Contains(c) ? Chars[c] : 0; }
        private static int LocationOfN(char c) { return int.TryParse(c.ToString(), out int x) ? x : 10; }
        private static int LocationOfC(char c) { return !Chars.Keys.Contains(c) ? -1 :
                                                Chars[c] > 0? Chars.Keys.ToList().IndexOf(c) :
                                               -1; }
    }
    public static class TestCharSet
    {
        public static void TestChars(this string s, WriteToCMDLine write)
        {
            try
            {
                var Loc1 = s.Locate(3, 4);
                var Loc2 = s.LocateScrabble(3, 4);
                var Loc3 = s.LocateNumbs(3, 4);
                var Loc4 = s.LocateLetters(3, 4);
                var Loc5 = s.LocateOrder(3, 4);
                var Loc6 = s.LocatePercent(3, 4);
                var Loc7 = s.LocatePhrase(3, 2);
                string l = "";
                l += "Base Location : ";
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < CharSet.CharCount; j++)
                    {
                        l += Loc1[(i * CharSet.CharCount) + j] + ", ";
                    }
                    l += "\r\n";
                }
                l += "\r\n\r\n\r\n";
                l += "Scrabble Location : ";
                for (int i = 0; i < 8; i++)
                {
                    l += Loc2[i] + ", ";
                    l += "\r\n";
                }
                l += "\r\n\r\n\r\n";
                l += "Number Location : ";
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < CharSet.NumbCount; j++)
                    {
                        l += Loc3[(i * CharSet.NumbCount) + j] + ", ";
                    }
                    l += "\r\n";
                }
                l += "\r\n\r\n\r\n";
                l += "Letter Location : ";
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < CharSet.LetterCount; j++)
                    {
                        l += Loc4[(i * CharSet.LetterCount) + j] + ", ";
                    }
                    l += "\r\n";
                }
                l += "\r\n\r\n\r\n";
                l += "Order Location : ";
                for (int i = 0; i < 8; i++)
                {
                    l += Loc5[i] + ", ";
                    l += "\r\n";
                }
                l += "\r\n\r\n\r\n";
                l += "Percent Location : ";
                for (int i = 0; i < 8; i++)
                {
                    l += Loc6[i] + ", ";
                    l += "\r\n";
                }
                l += "\r\n\r\n\r\n";
                l += "Phrase Location : ";
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < CharSet.LetterCount; j++)
                    {
                        l += Loc7[(i * CharSet.LetterCount) + j] + ", ";
                    }
                    l += "\r\n";
                }
                l += "\r\n\r\n\r\n";
                write(l);
            }
            catch(Exception e) { e.OutputError(); }
        }
    }
}

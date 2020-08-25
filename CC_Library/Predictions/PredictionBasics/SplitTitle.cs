using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class SplitTitle_Command
    {
        public static List<string> SplitTitle(this string s)
        {
            List<string> Array = new List<string>();
            List<string> data = new List<string>();
            char[] Characters = {'A', 'a', 'B', 'b', 'C', 'c', 'D', 'd', 'E', 'e', 'F', 'f', 'G', 'g',
            'H', 'h', 'I', 'i', 'J', 'j', 'K', 'k', 'L', 'l', 'M', 'm', 'N', 'n', 'O', 'o', 'P', 'p',
            'Q', 'q', 'R', 'r', 'S', 's', 'T', 't', 'U', 'u', 'V', 'v', 'W', 'w', 'X', 'x', 'Y', 'y', 'Z', 'z'};
            char[] text = s.ToCharArray();
            string next = "";
            for (int i = 0; i < text.Count(); i++)
            {
                if (Characters.Contains(text[i]))
                    next += text[i];
                else
                {
                    if (next.Length >= 3)
                        Array.Add(next);
                    next = "";
                }
            }
            if (next.Length >= 3)
                Array.Add(next);
            foreach (string a in Array)
            {
                data.AddRange(a.SplitOnCaps());
            }
            return data;
        }
        public static List<double[]> Engrams(this string s, int ncount)
        {
            List<double[]> Result = new List<double[]>();
            List<string> Words = s.SplitTitle();
            for(int i = 0; i < Words.Count() - ncount; i++)
            {
                List<Element> NElements = new List<Element>();
                for(int j = 0; j < ncount; j++)
                {
                    NElements.Add(Datatype.Dictionary.GetElement(Words[i + j]));
                }
                Result.Add(NElements.Combine());
            }
            return Result;
        }
    }
}

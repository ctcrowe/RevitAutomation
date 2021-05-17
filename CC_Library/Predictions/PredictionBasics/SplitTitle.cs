using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class SplitTitle_Command
    {
        public static List<string> GetWords(this string s)
        {
            List<string> output = new List<string>();
            var word = s.ToUpper();
            int length = word.Count();
            int start = 0;
            while (start < length - 1)
            {
                bool modified = false;
                for (int j = length; j > start; j--)
                {
                    string sub = word.Substring(start, j - start);
                    if (Enum.GetNames(typeof(Dict)).Contains(sub) || double.TryParse(sub, out double x))
                    {
                        if (double.TryParse(sub, out double y))
                            output.Add("NUMBER");
                        else
                            output.Add(sub);
                        start = j;
                        modified = true;
                        break;
                    }
                }
                if (!modified)
                    start++;
            }
            return output;
        }
    }
}
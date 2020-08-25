using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class Stud_Entry
    {
        public static Entry StudEntry(string s)
        {
            Entry e = new Entry();
            e.Keys = new string[s.Split(',').Count() - 1];
            e.Values = new string[1];
            e.Values[0] = s.Split(',').Last();
            for (int i = 0; i < s.Split(',').Count() - 1; i++)
            {
                e.Keys[i] = s.Split(',')[i];
            }
            e.correct = false;
            return e;
        }
    }
}

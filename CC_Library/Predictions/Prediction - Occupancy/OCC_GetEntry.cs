using System;
using System.Linq;

namespace CC_Library.Predictions
{
    internal class OCCGetEntry
    {
        public static Entry OCCEntry(string s)
        {
            Entry e = new Entry();
            e.Keys = new string[1];
            e.Keys[0] = s.Split(',').First();
            e.Values = new string[s.Split(',').Count() - 1];
            for (int i = 1; i < s.Split(',').Count(); i++)
            {
                e.Values[i - 1] = s.Split(',')[i];
            }
            e.correct = false;
            return e;
        }
    }
}

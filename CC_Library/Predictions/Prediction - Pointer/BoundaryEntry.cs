using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CC_Library.Predictions
{
    internal static class PointerEntry
    {
        public static List<Entry> PEntry(string s, WriteToCMDLine write)
        {
            List<Entry> Entries = new List<Entry>();
            Entry e = new Entry();
            e.Keys = new string[15];
            e.Values = new string[1] { s.Split(',').Last() };
            double KeyX = double.Parse(s.Split(',')[6]);
            double KeyY = double.Parse(s.Split(',')[7]);
            double KeyZ = double.Parse(s.Split(',')[8]);
            for (int i = 0; i < s.Split(',').Count() - 1; i+=3)
            {
                e.Keys[i] = (double.Parse(s.Split(',')[i]) - KeyX).ToString();
                e.Keys[i + 1] = (double.Parse(s.Split(',')[i + 1]) - KeyY).ToString();
                e.Keys[i + 2] = (double.Parse(s.Split(',')[i + 2]) - KeyZ).ToString();
            }
            e.correct = false;
            Entries.Add(e);
            return Entries;
        }
    }
}
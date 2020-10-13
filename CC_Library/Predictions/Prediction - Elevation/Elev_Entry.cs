using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CC_Library.Predictions
{
    internal static class ElevationEntry
    {
        public static List<Entry> ElevEntry(string s, WriteToCMDLine write)
        {
            List<Entry> Entries = new List<Entry>();
            Entry e = new Entry();
            e.Keys = new string[200];
            e.Values = new string[50];
            for (int i = 0; i < 200; i++)
            {
                e.Keys[i] = s.Split(',')[i];
            }
            for(int i = 0; i < 50; i++)
            {
                e.Values[i] = s.Split(',')[i + 200];
            }
            e.correct = false;
            Entries.Add(e);
            return Entries;
        }
    }
}
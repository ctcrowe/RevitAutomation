using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CC_Library.Predictions
{
    internal static class OLF_Entry
    {
        public static List<Entry> OLFEntry(string s, WriteToCMDLine write)
        {
            List<Entry> Entries = new List<Entry>();
            Entry e = new Entry();
            e.Keys = new string[1] { s.Split(',').First() };
            e.Values = new string[1] { s.Split(',').Last() };
            e.correct = false;
            Entries.Add(e);
            return Entries;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CC_Library.Predictions
{
    internal static class DictionaryEntry
    {
        public static List<Entry> DictEntry(string s, WriteToCMDLine write)
        {
            List<Entry> Entries = new List<Entry>();
            var l1 = s.Split('.');
            for(int i = 0; i < l1.Count(); i++)
            {
                var s1 = l1[i].SplitTitle();
                for (int k = 0; k < s1.Count(); k++)
                {
                    if (k >= 2)
                    {
                        Entry e = new Entry();
                        e.Keys = new string[1] { s1[i] };
                        e.Values = new string[1] { s1[i - 2] };
                        e.correct = false;
                        Entries.Add(e);
                    }
                    if (k >= 1)
                    {
                        Entry e = new Entry();
                        e.Keys = new string[1] { s1[i] };
                        e.Values = new string[1] { s1[i - 1] };
                        e.correct = false;
                        Entries.Add(e);
                    }
                    if (k <= s1.Count() - 2)
                    {
                        Entry e = new Entry();
                        e.Keys = new string[1] { s1[i] };
                        e.Values = new string[1] { s1[i + 1] };
                        e.correct = false;
                        Entries.Add(e);
                    }
                    if (k <= s1.Count() - 3)
                    {
                        Entry e = new Entry();
                        e.Keys = new string[1] { s1[i] };
                        e.Values = new string[1] { s1[i + 2] };
                        e.correct = false;
                        Entries.Add(e);
                    }
                }
            }
            return Entries;
        }
    }
}
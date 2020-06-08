using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class ReduceEntries
    {
        public static Dictionary<string, string[]> ReducedEntries(
            this KeyValuePair<string, Data> Referenced,
            Dictionary<string, string[]> EntrySet,
            WriteToCMDLine write)
        {
            Dictionary<string, string[]> ReducedEntries = new Dictionary<string, string[]>();
            foreach (KeyValuePair<string, string[]> Entry in EntrySet)
            {
                if (Referenced.Value.Datatype == Datatype.Dictionary)
                {
                    List<string> SplitEntry = Entry.Key.SplitTitle();
                    foreach (string s in SplitEntry)
                    {
                        if (s == Referenced.Key)
                        {
                            if (!ReducedEntries.ContainsKey(Entry.Key))
                                ReducedEntries.Add(Entry.Key, Entry.Value);
                            break;
                        }
                    }
                }
                else
                {
                    if (Entry.Value.Any(x => x == Referenced.Key))
                        if (!ReducedEntries.ContainsKey(Entry.Key))
                            ReducedEntries.Add(Entry.Key, Entry.Value);
                }
            }
            return ReducedEntries;
        }
        public static Dictionary<string, string[]> ReducedEntries(
            this KeyValuePair<string, Data> Referenced,
            Dictionary<string, string[]> EntrySet)
        {
            Dictionary<string, string[]> ReducedEntries = new Dictionary<string, string[]>();
            foreach (KeyValuePair<string, string[]> Entry in EntrySet)
            {
                if (Referenced.Value.Datatype == Datatype.Dictionary)
                {
                    List<string> SplitEntry = Entry.Key.SplitTitle();
                    foreach (string s in SplitEntry)
                    {
                        if (s == Referenced.Key)
                        {
                            if (!ReducedEntries.ContainsKey(Entry.Key))
                                ReducedEntries.Add(Entry.Key, Entry.Value);
                            break;
                        }
                    }
                }
                else
                {
                    if (Entry.Value.Any(x => x == Referenced.Key))
                        if (!ReducedEntries.ContainsKey(Entry.Key))
                            ReducedEntries.Add(Entry.Key, Entry.Value);
                }
            }
            return ReducedEntries;
        }
    }
}
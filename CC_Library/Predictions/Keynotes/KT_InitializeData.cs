using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions.Keynotes
{
    internal static class KT_InitializeData
    {
        public static void InitializeDict
            (this Dictionary<string, Data> ds,
            Dictionary<string, string[]> EntrySet,
            Random random, WriteToCMDLine write)
        {
            int Added = 0;
            foreach (string key in EntrySet.Keys)
            {
                foreach (string section in key.SplitTitle())
                {
                    if (!ds.ContainsKey(section))
                    {
                        Added++;
                        ds.Add(section, new Data(Datatype.Dictionary, section, random));
                    }
                }
            }
            write("Added : " + Added + "to the Dictionary. Total entries at : " + ds.Count());
        }
        public static void InitializeKeynotes
            (this Dictionary<string, Data> Keynotes,
            Dictionary<string, Data> Dict,
            Dictionary<string, string[]> EntrySet,
            Random random, WriteToCMDLine write)
        {
            int DictCount = 0;
            int KeyCount = 0;
            int AdjustedKeys = 0;
            foreach(KeyValuePair<string, string[]> Entry in EntrySet)
            {
                string[] Values = Entry.Value;
                foreach(string v in Values)
                {
                    var words = v.SplitTitle();
                    foreach (string s in words)
                    {
                        if (!Dict.ContainsKey(s))
                        {
                            DictCount++;
                            Dict.Add(s, new Data(Datatype.Dictionary, s, random));
                        }
                    }

                    var DictionaryPoints = Dict.Where(x => words.Contains(x.Key));
                    var WordPoint = DictionaryPoints.ResultantDatapoint();

                    if (!Keynotes.ContainsKey(v))
                    {
                        KeyCount++;
                        Keynotes.Add(v, new Data(Datatype.Keynote, v, WordPoint.Value));
                    }
                    else
                    {
                        AdjustedKeys++;
                        Keynotes[v].Location = WordPoint.Value;
                    }
                }
            }
            write("Added : " + DictCount + " to the Dictionary. Total entries at : " + Dict.Count());
            write("Added : " + KeyCount + " to the Keynote File. Total entries at : " + Keynotes.Count());
            write("Adjusted : " + AdjustedKeys + " Existing Keynotes in the File.");
        }
    }
}

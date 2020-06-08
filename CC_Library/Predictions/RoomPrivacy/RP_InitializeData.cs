using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions.RoomPrivacy
{
    internal static class RP_InitializeData
    {
        public static void InitializeDict(this Dictionary<string, Data> ds, Dictionary<string, string[]> EntrySet, Random random, WriteToCMDLine write)
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
        public static void InitializeRP(this Dictionary<string, Data> ds, Dictionary<string, string[]> EntrySet, Random random, WriteToCMDLine write)
        {
            int Added = 0;
            foreach (string[] values in EntrySet.Values)
            {
                foreach (string value in values)
                {
                    if (!ds.ContainsKey(value))
                    {
                        Added++;
                        ds.Add(value, new Data(Datatype.RoomPrivacy, value, random));
                    }
                }
            }
            write("Added : " + Added + "to the Room Privacy Data. Total entries at : " + ds.Count());
        }
    }
}

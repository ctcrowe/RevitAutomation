using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CC_Library.Predictions
{
    internal static class Entryset
    {
        public static Dictionary<string, string[]> GetEntryValues(this string filename, WriteToCMDLine write)
        {
            Dictionary<string, string[]> Values = new Dictionary<string, string[]>();
            if(File.Exists(filename))
            {
                string[] lines = File.ReadAllLines(filename);
                foreach(string l in lines)
                {
                    if (!Values.ContainsKey(l.Split(',').First()))
                    {
                        string[] results = new string[l.Split(',').Count() - 1];
                        for(int i = 1; i < l.Split(',').Count(); i++)
                        {
                            results[i-1] = l.Split(',')[i];
                        }
                        Values.Add(l.Split(',').First(), results);
                    }
                }
                write(Values.Count().ToString() + " entries loaded from " + filename);
            }
            return Values;
        }
    }
}

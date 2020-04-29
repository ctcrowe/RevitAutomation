using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CC_Library.Predictions
{
    internal static class Entryset
    {
        public static Dictionary<string, string> GetEntryValues(this string filename, WriteToCMDLine write)
        {
            Dictionary<string, string> Values = new Dictionary<string, string>();
            if(File.Exists(filename))
            {
                string[] lines = File.ReadAllLines(filename);
                foreach(string l in lines)
                {
                    if (!Values.ContainsKey(l.Split(',').First()))
                        Values.Add(l.Split(',').First(), l.Split(',').Last());
                }
                write(Values.Count().ToString() + " entries loaded from " + filename);
            }
            return Values;
        }
    }
}

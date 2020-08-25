using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CC_Library.Predictions
{
    internal static class PredictionGetEntries
    {
        public static List<Entry> GetEntries(this string filename, GetEntry getentry, WriteToCMDLine write)
        {
            List<Entry> Entries = new List<Entry>();
            if(File.Exists(filename))
            {
                string[] lines = File.ReadAllLines(filename);
                foreach(string l in lines)
                {
                    Entries.Add(getentry(l));
                }
                write(Entries.Count().ToString() + " entries loaded from " + filename);
            }
            return Entries;
        }
    }
}

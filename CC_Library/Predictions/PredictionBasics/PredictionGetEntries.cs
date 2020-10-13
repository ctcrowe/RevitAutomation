using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace CC_Library.Predictions
{
    internal static class PredictionGetEntries
    {
        public static List<Entry> GetEntries
            (this string filename,
            Func<string, WriteToCMDLine, List<Entry>> getentry,
            WriteToCMDLine write)
        {
            List<Entry> Entries = new List<Entry>();
            if(File.Exists(filename))
            {
                string[] lines = File.ReadAllLines(filename);
                foreach(string l in lines)
                {
                    Entries.AddRange(getentry(l, write));
                }
                write(Entries.Count().ToString() + " entries loaded from " + filename);
            }
            return Entries;
        }
    }
}

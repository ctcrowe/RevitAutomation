using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library;
using CC_Library.Datatypes;


namespace CC_Library.Predictions
{
    internal delegate double Initialize
        (Dictionary<string, string[]> Entries,
        WriteToCMDLine write,
        Random random);
    public delegate void WriteToOutput(List<string> s, string output);
    public delegate void Hold();
    public static class Delegates
    {
        public static string WriteNull(string s) { return null; }
        public static void show(this string label, double[] values, WriteToCMDLine write)
        {
            string s = label + " : ";
            for (int i = 0; i < values.Count(); i++)
            {
                s += values[i] + ", ";
            }
            write(s);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal delegate double CalcAccuracy
        (Dictionary<string, Data> Dataset,
        Dictionary<string, Data> Dict,
        Dictionary<string, string[]> EntrySet);
    internal delegate double Initialize
        (Dictionary<string, string[]> Entries,
        WriteToCMDLine write,
        Random random);
    public delegate void WriteToCMDLine(string s);
    public delegate void WriteToOutput(List<string> s, string output);
    public delegate void Hold();
}

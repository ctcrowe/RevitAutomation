using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal delegate List<string>[] ChangedElements
        (Entry[] entry);
    internal delegate double[] Accuracy
        (List<Entry> Entries,
        NeuralNetwork network);
    internal delegate Dictionary<string, Element>[] InitializeData
        (List<Entry> Entries);
    internal delegate double Initialize
        (Dictionary<string, string[]> Entries,
        WriteToCMDLine write,
        Random random);
    public delegate void WriteToCMDLine(string s);
    public delegate void WriteToOutput(List<string> s, string output);
    public delegate void Hold();
}

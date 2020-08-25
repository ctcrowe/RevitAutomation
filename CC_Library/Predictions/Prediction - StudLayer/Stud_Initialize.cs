using System.Collections.Generic;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class Stud_Initialize
    {
        public static Dictionary<string, Element>[] Initialize(this List<Entry> Entries)
        {
            Dictionary<string, Element>[] Datasets = new Dictionary<string, Element>[2];
            Datasets[0] = new Dictionary<string, Element>();
            Datasets[1] = new Dictionary<string, Element>();
            Datasets[0].Add("Stud", Datatype.StudLayer.GetElement("Stud"));
            foreach (Entry entry in Entries)
            {
                foreach (string result in entry.Values)
                {
                    if (!Datasets[1].ContainsKey(result))
                        Datasets[1].Add(result, Datatype.Dictionary.GetElement(result));
                }
                foreach (string e in entry.Keys)
                {
                    var Words = e.SplitTitle();
                    foreach (string word in Words)
                    {
                        if (!Datasets[1].ContainsKey(word))
                            Datasets[1].Add(word, Datatype.Dictionary.GetElement(word));
                    }
                }
            }
            return Datasets;
        }
        public static List<string>[] CollectChangedElements
            (this List<Entry> Adjusted)
        {
            List<string>[] ChangedElements = new List<string>[2];
            ChangedElements[0] = new List<string>();
            ChangedElements[1] = new List<string>();
            ChangedElements[0].Add("Stud");
            foreach (var entry in Adjusted)
            {
                foreach (var e in entry.Keys)
                {
                    var strings = e.SplitTitle();
                    foreach (var s in strings)
                    {
                        if(!ChangedElements[1].Contains(s))
                        ChangedElements[1].Add(s);
                    }
                }
            }
            return ChangedElements;
        }
    }
}

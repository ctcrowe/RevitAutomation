using System;
using System.Collections.Generic;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class OCCInitialize
    {
        public static Dictionary<string, Element>[] Initialize(this List<Entry> Entries)
        {
            Dictionary<string, Element>[] Datasets = new Dictionary<string, Element>[2];
            Datasets[0] = new Dictionary<string, Element>();
            Datasets[1] = new Dictionary<string, Element>();
            foreach (Entry entry in Entries)
            {
                foreach (string result in entry.Values)
                {
                    if (!Datasets[0].ContainsKey(result))
                        Datasets[0].Add(result, Datatype.OccupancyGroup.GetElement(result));
                }
                var Words = entry.Keys[0].GetWords();
                foreach (string word in Words)
                {
                    if (!Datasets[1].ContainsKey(word))
                        Datasets[1].Add(word, Datatype.Dictionary.GetElement(word));
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
            foreach (var entry in Adjusted)
            {
                foreach (string result in entry.Values)
                {
                    if (!ChangedElements[0].Contains(result))
                        ChangedElements[0].Add(result);
                }
                var words = entry.Keys[0].GetWords();
                foreach (string word in words)
                {
                    if (!ChangedElements[1].Contains(word))
                        ChangedElements[1].Add(word);
                }
            }
            return ChangedElements;
        }
    }
}

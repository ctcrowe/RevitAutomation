using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class PredictionChangedElements
    {
        public static List<Element> CollectChangedElements
            (this Dictionary<string, bool> changes)
        {
            List<Element> eles = new List<Element>();
            foreach(var kvp in changes)
            {
                if (kvp.Value)
                    eles.Add(Datatype.Masterformat.GetElement(kvp.Key));
                else
                    eles.Add(Datatype.Dictionary.GetElement(kvp.Key));
            }
            return eles;
        }
        public static Dictionary<string, Element> ChangedElements
            (this IEnumerable<Entry> entries)
        {
            Dictionary<string, Element> eles = new Dictionary<string, Element>();
            foreach(Entry entry in entries)
            {
                var WordList = entry.Keys[0].GetWords();
                foreach (var word in WordList)
                {
                    if (!eles.ContainsKey(word))
                        eles.Add(word, Datatype.Dictionary.GetElement(word));
                }
                foreach (var value in entry.Values)
                    if (!eles.ContainsKey(value))
                        eles.Add(value, Datatype.Masterformat.GetElement(value));
            }
            return eles;
        }
        public static Dictionary<string, Element> ChangedElements
            (this Entry entry)
        {
            Dictionary<string, Element> eles = new Dictionary<string, Element>();
            var WordList = entry.Keys[0].GetWords();
            foreach (var word in WordList)
            {
                if (!eles.ContainsKey(word))
                    eles.Add(word, Datatype.Dictionary.GetElement(word));
            }
            foreach (var value in entry.Values)
                if (!eles.ContainsKey(value))
                    eles.Add(value, Datatype.Masterformat.GetElement(value));

            return eles;
        }
    }
}

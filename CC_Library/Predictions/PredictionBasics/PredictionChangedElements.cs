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

        public static string[] CollectChangedElements
            (this Dictionary<string, Element> Dataset, int Count)
        {
            int FCount = 0;
            if (Dataset.Count() < Count)
                FCount = Dataset.Count();
            else
                FCount = Count;
            string[] Results = new string[FCount];
            var Data = Dataset.OrderBy(x => x.Value.accuracy).Take(FCount);
            for(int i = 0; i < Results.Count(); i++)
            {
                Results[i] = Data.ElementAt(i).Key;
            }
            return Results;
        }
        public static KeyValuePair<string[], string[]> CollectChangedElements
            (this Dictionary<string, Element> Dataset,
            Dictionary<string, Element> Compset, int Count)
        {
            var Combo = Dataset.Concat(Compset).OrderBy(x => x.Value.accuracy).Take(Count);
            var Data = Combo.Where(x => x.Value.datatype == Dataset.First().Value.datatype);
            var Comp = Combo.Where(x => x.Value.datatype != Dataset.First().Value.datatype);
            string[] Set = Data.ToDictionary(x => x.Key, y => y.Value).Keys.ToArray();
            string[] Compare = Comp.ToDictionary(x => x.Key, y => y.Value).Keys.ToArray();
            return new KeyValuePair<string[], string[]>(Set, Compare);
        }
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
                var WordList = entry.Keys[0].SplitTitle();
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
    }
}

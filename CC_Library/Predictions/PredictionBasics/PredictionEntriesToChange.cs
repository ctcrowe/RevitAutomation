using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class EntriesToChange
    {
        public static double FurthestElements
            (this List<Entry> EntrySet,
            Dictionary<string, Element> Dataset,
            Dictionary<string, Element> Dictionary,
            WriteToCMDLine write)
        {
            double correct = 0;
            double total = 0;

            foreach (var Entry in EntrySet)
            {
                double distance = 0;
                var WordList = Entry.Keys[0].GetWords();
                Dictionary<string, Element> DictPoints = new Dictionary<string, Element>();
                foreach (var word in WordList)
                {
                    if (!DictPoints.ContainsKey(word))
                    {
                        DictPoints.Add(word, Dictionary[word]);
                    }
                }

                if (DictPoints.Any())
                {
                    var WordPoint = DictPoints.Combine();
                    foreach (string val in Entry.Values)
                    {
                        var ele = Dataset[val];
                        double dist = ele.Distance(WordPoint);

                        if (dist > distance)
                        {
                            distance = dist;
                        }
                    }

                    var ResultantPoint = Dataset.FindClosest(WordPoint);
                    foreach (string val in Entry.Values)
                    {
                        total++;
                        if (ResultantPoint == val)
                        {
                            correct++;
                        }
                    }
                }
            }

            double acc = correct / total;
            write("Current Total Accuracy : " + acc);

            return acc;
        }
    }
}
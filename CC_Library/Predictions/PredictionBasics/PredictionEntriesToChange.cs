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
            foreach(var e in Dataset)
            {
                e.Value.total = 1;
                e.Value.correct = 1;
            }
            foreach(var e in Dictionary)
            {
                e.Value.total = 1;
                e.Value.correct = 1;
            }

            foreach (var Entry in EntrySet)
            {
                double distance = 0;
                var WordList = Entry.Keys[0].SplitTitle();
                Dictionary<string, Element> DictPoints = new Dictionary<string, Element>();
                foreach (var word in WordList)
                {
                    if (!DictPoints.ContainsKey(word))
                    {
                        Dictionary[word].total++;
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
                        Dataset[val].total++;
                        if (ResultantPoint == val)
                        {
                            foreach (var word in WordList)
                                Dictionary[word].correct++;
                            Dataset[val].correct++;
                            correct++;
                        }
                    }
                }
            }

            foreach (var e in Dataset)
                e.Value.accuracy = (e.Value.correct * 1.0) / (e.Value.total * 1.0);
            foreach (var e in Dictionary)
                e.Value.accuracy = (e.Value.correct * 1.0) / (e.Value.total * 1.0);

            double acc = correct / total;
            write("Current Total Accuracy : " + acc);

            return acc;
        }
    }
}
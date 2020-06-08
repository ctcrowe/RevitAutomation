using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.IO;

namespace CC_Library.Predictions
{
    internal static class Accuracy
    {
        /*
        private static double WithinRangeAccuracy(this Dataset ReferenceSet, Dataset DictionarySet, Dictionary<string, string[]> EntrySet, double Distance)
        {
            double correct = 0;
            double total = 0;
            foreach (var Entry in EntrySet)
            {
                var WordList = Entry.Key.SplitTitle();
                var DictionaryPoints = DictionarySet.Data.Where(x => WordList.Contains(x.Key));
                var WordPoint = DictionaryPoints.ResultantDatapoint();

                if (ReferenceSet.Data.Any())
                {
                    var ResultantPoint = ReferenceSet.FindWithinRange(WordPoint, Distance);
                    foreach(var rp in ResultantPoint)
                    {
                        if (Entry.Value.Any(x => x == rp.Key))
                            correct++;
                        total++;
                    }
                }
            }
            double d = correct / total;
            return d;
        }*/
    }
}
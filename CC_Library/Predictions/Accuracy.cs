using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class Accuracy
    {
        public static double CalcAccuracy(this Dataset ActionSet, Dataset ReferencedSet, Dictionary<string, string> EntrySet, WriteToCMDLine write)
        {
            if (ActionSet.datatype == Datatype.TextData)
                return ReferencedSet.BasicAccuracy(ActionSet, EntrySet, write);
            else
                return ActionSet.BasicAccuracy(ReferencedSet, EntrySet, write);
        }
        private static double BasicAccuracy(this Dataset ReferenceSet, Dataset DictionarySet, Dictionary<string, string> EntrySet, WriteToCMDLine write)
        {
            double correct = 0;
            double total = 0;
            foreach (var Entry in EntrySet)
            {
                total++;
                var WordList = Entry.Key.SplitTitle();
                var DictionaryPoints = DictionarySet.Data.Where(x => WordList.Contains(x.Key));
                var WordPoint = DictionaryPoints.ResultantDatapoint();

                if (ReferenceSet.Data.Any())
                {
                    var ResultantPoint = ReferenceSet.FindClosest(WordPoint, write);

                    if (ResultantPoint.Key == Entry.Value)
                    {
                        correct++;
                    }
                }
            }
            double d = correct / total;
            return d;
        }
    }
}
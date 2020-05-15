using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.IO;

namespace CC_Library.Predictions
{
    internal static class Accuracy
    {
        public static double CalcAccuracy(this Dataset ActionSet, Dataset ReferencedSet, Dictionary<string, string[]> EntrySet, bool closest, double range)
        {
            if (closest)
            {
                if (ActionSet.datatype == Datatype.TextData)
                    return ReferencedSet.ClosestAccuracy(ActionSet, EntrySet);
                else
                    return ActionSet.ClosestAccuracy(ReferencedSet, EntrySet);
            }
            else
            {
                if (ActionSet.datatype == Datatype.TextData)
                    return ReferencedSet.WithinRangeAccuracy(ActionSet, EntrySet, range);
                else
                    return ActionSet.WithinRangeAccuracy(ReferencedSet, EntrySet, range);
            }
        }
        public static void ShowClosestAccuracy(this Dataset ReferenceSet, Dataset DictionarySet, Dictionary<string, string[]> EntrySet, WriteToCMDLine write, int i)
        {
            double total = 0;
            double correct = 0;
            var Lines = new List<string>();
            string directory = ReferenceSet.datatype + "AccuracyTest";
            string dir = directory.GetMyDocs();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string file = dir + "\\" + ReferenceSet.datatype + "AccuracyTest" + i + ".csv";

            foreach (var Entry in EntrySet)
            {
                var WordList = Entry.Key.SplitTitle();
                var DictionaryPoints = DictionarySet.Data.Where(x => WordList.Contains(x.Key));
                var WordPoint = DictionaryPoints.ResultantDatapoint();

                if (ReferenceSet.Data.Any())
                {
                    var ResultantPoint = ReferenceSet.FindNClosest(WordPoint, Entry.Value.Count());
                    string results = ResultantPoint.First().Key;
                    string entry = Entry.Value.FirstOrDefault();
                    for(int rp = 1; rp < ResultantPoint.Count(); rp++)
                    {
                        if(results.Count() > rp)
                            results += (", " + ResultantPoint[i].Key);
                    }
                    foreach (string val in Entry.Value)
                    {
                        total++;
                        if (val != Entry.Value.FirstOrDefault())
                            entry += ", " + val;
                        if (ResultantPoint.Any(x => x.Key == val))
                            correct++;
                    }
                    Lines.Add("Total : " + total + ", Correct : " + correct + ", Key : " + Entry.Key + " , Entry Value : " + entry + " , Predicted Values : " + results);
                }
            }
            double accuracy = correct / total * 100;
            Lines.Add("Accuracy : " + accuracy);
            File.WriteAllLines(file, Lines);
            write(file);
        }
        private static double ClosestAccuracy(this Dataset ReferenceSet, Dataset DictionarySet, Dictionary<string, string[]> EntrySet)
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
                    var ResultantPoint = ReferenceSet.FindNClosest(WordPoint, Entry.Value.Count());
                    foreach(string val in Entry.Value)
                    {
                        total++;
                        if(ResultantPoint.Any(x => x.Key == val))
                            correct++;
                    }
                }
            }
            double d = correct / total;
            return d;
        }
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
        }
    }
}
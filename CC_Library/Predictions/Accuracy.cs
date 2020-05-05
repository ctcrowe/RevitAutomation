using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.IO;

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
        public static void ShowAccuracy(this Dataset ReferenceSet, Dataset DictionarySet, Dictionary<string, string> EntrySet, WriteToCMDLine write, int i)
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
                total++;
                var WordList = Entry.Key.SplitTitle();
                var DictionaryPoints = DictionarySet.Data.Where(x => WordList.Contains(x.Key));
                var WordPoint = DictionaryPoints.ResultantDatapoint();

                if (ReferenceSet.Data.Any())
                {
                    var ResultantPoint = ReferenceSet.FindClosest(WordPoint);
                    if (ResultantPoint.Key == Entry.Value)
                        correct++;
                    Lines.Add("Total : " + total + ", Correct : " + correct + ", Key : " + Entry.Key + " , Entry Value : " + Entry.Value + " , Predicted Value : " + ResultantPoint.Key);
                }
            }
            double accuracy = correct / total * 100;
            Lines.Add("Accuracy : " + accuracy);
            File.WriteAllLines(file, Lines);
            write(file);
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
                    var ResultantPoint = ReferenceSet.FindClosest(WordPoint);

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
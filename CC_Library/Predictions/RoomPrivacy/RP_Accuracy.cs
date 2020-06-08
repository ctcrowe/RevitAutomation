using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CC_Library.Predictions.RoomPrivacy
{
    internal static class RP_Accuracy
    {
        public static void ShowAccuracy
            (this Dictionary<string, Data> RPDataset,
            Dictionary<string, Data> DictionarySet,
            Dictionary<string, string[]> EntrySet, WriteToCMDLine write,
            int i)
        {
            double total = 0;
            double correct = 0;
            var Lines = new List<string>();
            string directory = RPDataset.FirstOrDefault().Value.Datatype + "AccuracyTest";
            string dir = directory.GetMyDocs();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string file = dir + "\\" + RPDataset.FirstOrDefault().Value.Datatype + "AccuracyTest" + i + ".csv";

            foreach (var Entry in EntrySet)
            {
                var WordList = Entry.Key.SplitTitle();
                var DictionaryPoints = DictionarySet.Where(x => WordList.Contains(x.Key));
                var WordPoint = DictionaryPoints.ResultantDatapoint();

                if (RPDataset.Any())
                {
                    var ResultantPoint = RPDataset.FindNClosest(WordPoint, Entry.Value.Count());
                    string results = ResultantPoint.First().Key;
                    string entry = Entry.Value.FirstOrDefault();
                    foreach (string key in ResultantPoint.Keys)
                    {
                        if (key != ResultantPoint.First().Key)
                            results += ", " + key;
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
        public static double Accuracy
            (this Dictionary<string, Data> RPDataset,
            Dictionary<string, Data> DictionarySet,
            Dictionary<string, string[]> EntrySet)
        {
            double correct = 0;
            double total = 0;
            foreach (var Entry in EntrySet)
            {
                var WordList = Entry.Key.SplitTitle();
                var DictionaryPoints = DictionarySet.Where(x => WordList.Contains(x.Key));
                var WordPoint = DictionaryPoints.ResultantDatapoint();

                if (RPDataset.Any())
                {
                    var ResultantPoint = RPDataset.FindNClosest(WordPoint, Entry.Value.Count());
                    foreach (string val in Entry.Value)
                    {
                        total++;
                        if (ResultantPoint.Any(x => x.Key == val))
                            correct++;
                    }
                }
            }
            double d = correct / total;
            return d;
        }
    }
}

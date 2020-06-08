using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CC_Library.Predictions.Keynotes
{
    internal static class KT_Accuracy
    {
        public static void ShowAccuracy
            (this Dictionary<string, Data> KeynoteSet,
            Dictionary<string, Data> DictionarySet,
            Dictionary<string, string[]> EntrySet, WriteToCMDLine write,
            int i)
        {
            double total = 0;
            double correct = 0;
            var Lines = new List<string>();
            string directory = "KeynoteTextAccuracyTest";
            string dir = directory.GetMyDocs();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string file = dir + "\\KeynoteTextAccuracyTest" + i + ".csv";

            if (DictionarySet.Any())
            {
                foreach (var Entry in EntrySet)
                {
                    var WordList = Entry.Key.SplitTitle();
                    var WordToDict = DictionarySet.Where(x => WordList.Contains(x.Key));
                    var WordLocation = WordToDict.ResultantDatapoint();

                    var ResultantPoint = DictionarySet.FindNClosest(WordLocation, Entry.Value.Count());
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
                    Lines.Add("Total : " + total
                        + ", Correct : " + correct
                        + ", Key : " + Entry.Key
                        + ", Entry Value : " + entry
                        + ", Predicted Values : " + results);
                }
            }
            double accuracy = correct / total * 100;

            Lines.Add("Accuracy : " + accuracy);
            File.WriteAllLines(file, Lines);
            write(file);
        }
        public static double Accuracy
            (this Dictionary<string, Data> KeynoteSet,
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

                if (KeynoteSet.Any())
                {
                    var ResultantPoint = KeynoteSet.FindNClosest(WordPoint, Entry.Value.Count());
                    foreach (string val in Entry.Value)
                    {
                        total++;
                        if (ResultantPoint.ContainsKey(val))
                            correct++;
                    }
                }
            }
            double d = correct / total;
            return d;
        }
    }
}
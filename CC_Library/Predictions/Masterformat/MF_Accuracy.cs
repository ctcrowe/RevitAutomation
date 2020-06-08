using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CC_Library.Predictions.Masterformat
{
    internal static class MF_Accuracy
    {
        public static void ShowAccuracy
            (this Dictionary<string, Data> MFDataset, 
            Dictionary<string, Data> DictionarySet,
            Dictionary<string, string[]> EntrySet, WriteToCMDLine write,
            int i)
        {
            double total = 0;
            double correct = 0;
            var Lines = new List<string>();
            string directory = MFDataset.FirstOrDefault().Value.Datatype + "AccuracyTest";
            string dir = directory.GetMyDocs();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string file = dir + "\\" + MFDataset.FirstOrDefault().Value.Datatype + "AccuracyTest" + i + ".csv";

            foreach (var Entry in EntrySet)
            {
                var WordList = Entry.Key.SplitTitle();
                var DictionaryPoints = DictionarySet.Where(x => WordList.Contains(x.Key));
                var WordPoint = DictionaryPoints.ResultantDatapoint();

                if (MFDataset.Any())
                {
                    var ResultantPoint = MFDataset.FindNClosest(WordPoint, Entry.Value.Count());
                    var result = ResultantPoint.First();
                    double closest = result.CalcDistance(WordPoint.Value);
                    double distance = MFDataset[Entry.Value.FirstOrDefault()].CalcDistance(WordPoint.Value);
                    string results = ResultantPoint.First().Key;
                    string entry = Entry.Value.FirstOrDefault();
                    foreach(string key in ResultantPoint.Keys)
                    {
                        if(key != ResultantPoint.First().Key)
                            results += ", " + key;
                    }
                    foreach (string val in Entry.Value)
                    {
                        total++;
                        if (val != Entry.Value.FirstOrDefault())
                            entry += ", " + val;
                        if (ResultantPoint.ContainsKey(val))
                            correct++;
                    }
                    Lines.Add("Total : " + total + ", Correct : " + correct + ", Distance : " + distance + ", Closest Distance : " + closest + ", Key : " + Entry.Key);
                }
            }
            double accuracy = correct / total * 100;
            Lines.Add("Accuracy : " + accuracy);
            File.WriteAllLines(file, Lines);
            write(file);
        }
        public static double Accuracy
            (this Dictionary<string, Data> MFDataset,
            Dictionary<string, Data> DictionarySet,
            Dictionary<string, string[]> EntrySet)
        {
            double correct = 0;
            double total = 0;
            foreach (var Entry in EntrySet)
            {
                var WordList = Entry.Key.SplitTitle();
                var DictionaryPoints = DictionarySet.Where(x => WordList.Contains(x.Key));
                if (DictionaryPoints.Any())
                {
                    var WordPoint = DictionaryPoints.ResultantDatapoint();

                    if (MFDataset.Any())
                    {
                        var ResultantPoint = MFDataset.FindNClosest(WordPoint, Entry.Value.Count());
                        foreach (string val in Entry.Value)
                        {
                            total++;
                            if (ResultantPoint.ContainsKey(val))
                                correct++;
                        }
                    }
                }
            }
            double d = correct / total;
            return d;
        }
        public static double CreateAccuracy
            (Dictionary<string, Data> MFDataset,
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

                if (MFDataset.Any())
                {
                    var ResultantPoint = MFDataset.FindNClosest(WordPoint, Entry.Value.Count());
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

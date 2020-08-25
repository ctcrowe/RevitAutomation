using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class MF_XmlAccuracy
    {
        public static double Accuracy2
            (List<XDocument> Dataset,
            List<XDocument> DictSet,
            Dictionary<string, string[]> EntrySet)
        {
            double correct = 0;
            double total = 0;

            foreach (var Entry in EntrySet)
            {
                var WordList = Entry.Key.SplitTitle();
                var DictPoints = DictSet.Where(x => WordList.Any(y => y == x.Root.Attribute("Label").Value));

                if (DictPoints.Any())
                {
                    var WordPoint = DictPoints.ResultantDatapoint(Datatype.Masterformat.First());

                    var ResultantPoint = Dataset.FindClosest(WordPoint);
                    foreach (string val in Entry.Value)
                    {
                        total++;
                        if (ResultantPoint == val)
                            correct++;
                    }
                }
            }

            double acc = correct / total;
            return acc;
        }
        public static double Accuracy
            (List<XDocument> Dataset,
            List<XDocument> DictSet,
            Dictionary<string, string[]> EntrySet)
        {
            foreach (var d in DictSet)
            {
                if (d.Root.Attribute("Total") != null)
                    d.Root.Attribute("Total").Value = "1";
                else
                    d.Root.Add(new XAttribute("Total", "1"));
                if (d.Root.Attribute("Correct") != null)
                    d.Root.Attribute("Correct").Value = "1";
                else
                    d.Root.Add(new XAttribute("Correct", "1"));
            }
            foreach (var d in Dataset)
            {
                if (d.Root.Attribute("Total") != null)
                    d.Root.Attribute("Total").Value = "1";
                else
                    d.Root.Add(new XAttribute("Total", "1"));
                if (d.Root.Attribute("Correct") != null)
                    d.Root.Attribute("Correct").Value = "1";
                else
                    d.Root.Add(new XAttribute("Correct", "1"));
            }

            double correct = 0;
            double total = 0;

            foreach (var Entry in EntrySet)
            {
                var WordList = Entry.Key.SplitTitle();
                var DictPoints = DictSet.Where(x => WordList.Contains(x.Root.Attribute("Label").Value));
                foreach (var dp in DictPoints)
                    dp.Root.Attribute("Total").Value = (int.Parse(dp.Root.Attribute("Total").Value) + 1).ToString();

                if (DictSet.Any())
                {
                    var WordPoint = DictSet.ResultantDatapoint(Datatype.Masterformat.First());

                    var ResultantPoint = Dataset.FindClosest(WordPoint);
                    foreach (string val in Entry.Value)
                    {
                        total++;

                        if (Dataset.Any(x => x.Root.Name == Datatype.Masterformat.ToString() + "_" + val))
                        {
                            Dataset.Where(x => x.Root.Name == Datatype.Masterformat.ToString() + "_" + val).First().Root.Attribute("Total").Value =
                                (int.Parse(Dataset.Where(x => x.Root.Name == Datatype.Masterformat.ToString() + "_" + val)
                                .First().Root.Attribute("Total").Value) + 1).ToString();
                        }

                        if (ResultantPoint == val)
                        {
                            foreach (var dp in DictPoints)
                                dp.Root.Attribute("Correct").Value =
                                    (int.Parse(dp.Root.Attribute("Correct").Value) + 1).ToString();

                            Dataset.Where(x => x.Root.Name == Datatype.Masterformat.ToString() + "_" + val).First().Root.Attribute("Total").Value =
                                (int.Parse(Dataset.Where(x => x.Root.Name == Datatype.Masterformat.ToString() + "_" + val)
                                .First().Root.Attribute("Correct").Value) + 1).ToString();
                            correct++;
                        }
                    }
                }
            }
            foreach (var d in DictSet)
            {
                if (d.Root.Attribute("Accuracy") != null)
                    d.Root.Attribute("Accuracy").Value =
                        (double.Parse(d.Root.Attribute("Correct").Value) /
                        double.Parse(d.Root.Attribute("Total").Value)).ToString();
                else
                    d.Root.Add(new XAttribute("Accuracy", (double.Parse(d.Root.Attribute("Correct").Value) /
                        double.Parse(d.Root.Attribute("Total").Value)).ToString()));
            }
            foreach (var d in Dataset)
            {
                if (d.Root.Attribute("Accuracy") != null)
                    d.Root.Attribute("Accuracy").Value =
                        (double.Parse(d.Root.Attribute("Correct").Value) /
                        double.Parse(d.Root.Attribute("Total").Value)).ToString();
                else
                    d.Root.Add(new XAttribute("Accuracy", (double.Parse(d.Root.Attribute("Correct").Value) /
                        double.Parse(d.Root.Attribute("Total").Value)).ToString()));
            }

            double acc = correct / total;
            return acc;
        }
    }
}

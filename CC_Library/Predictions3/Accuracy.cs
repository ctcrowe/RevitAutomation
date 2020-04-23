using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class Accuracy
    {
        public static double CalcAccuracy(this Dataset ActionSet, Dataset ReferencedSet, Dictionary<string, string> EntrySet, Random random, WriteToCMDLine write)
        {
            if (ActionSet.datatype == Datatype.TextData)
                return ReferencedSet.BasicAccuracy(ActionSet, EntrySet, random, write);
            else
                return ActionSet.CalcAccuracy(ReferencedSet, EntrySet, random, write);
        }
        private static double BasicAccuracy(this Dataset ReferenceSet, Dataset DictionarySet, Dictionary<string, string> EntrySet, Random random, WriteToCMDLine write)
        {
            double correct = 0;
            double total = 0;
            foreach(var Entry in EntrySet)
            {
                total++;
                var WordList = Entry.Key.SplitTitle();
                foreach(string s in WordList)
                {
                    if (!DictionarySet.Data.ContainsKey(s))
                        DictionarySet.AddEntry(s, random);
                }
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
                if (!ReferenceSet.Data.ContainsKey(Entry.Value))
                    ReferenceSet.AddEntry(Entry.Value, random);
            }
            double d = correct / total;
            return d;
        }

        public static double FindMaximum(this double[] values)
        {
            double max = 0;
            for(int i = 0; i < values.Count(); i++)
            {
                if (Math.Abs(values[i]) > max)
                    max = Math.Abs(values[i]);
            }
            return max;
        }
        public static double Sigmoid(this double x)
        {
            double Top = Math.Pow(Math.E, x);
            double Bottom = Top + 1;
            return Top / Bottom;
        }
        public static Dictionary<string, string> ReducedEntries(this KeyValuePair<string, double[]> Replaced, Dataset ActionSet, Dictionary<string, string> EntrySet)
        {
            Dictionary<string, string> ReducedEntries = new Dictionary<string, string>();
            if (ActionSet.datatype == Datatype.TextData)
            {
                foreach (KeyValuePair<string, string> kvp in EntrySet)
                {
                    if (kvp.Key.SplitTitle().Contains(Replaced.Key))
                        ReducedEntries.Add(kvp.Key, kvp.Value);
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> kvp in EntrySet)
                {
                    if (kvp.Value == Replaced.Key)
                        ReducedEntries.Add(kvp.Key, kvp.Value);
                }
            }
            return ReducedEntries;
        }
        public static void ChartEntry(this KeyValuePair<string, double[]> Datum, Dataset ActionSet, Dataset ReferencedSet, Dictionary<string, string> EntrySet, Random random, WriteToCMDLine write)
        {
            /*
             * Rather than randomly adjusting a value when its wrong or trying to chart out what direction it should move like this,
             * Simplify the system to find the location of the "Correct" Value and move everything closer together.
             * The "Direction" That something should move should be a ratio relative to the Accuracy of the independent and the direction that it should move in.
             * Direction is going to be the vector between the action datum and the relevant reference datum (coordinated through the entry set)
             * ResultantVector = Sum of all phrases
             * Return Vector = Direction from resultant vector towards correct reference vector
            */

            Dictionary<string, string> ReducedEntries = Datum.ReducedEntries(ActionSet, EntrySet);
            double Accuracy = ActionSet.CalcAccuracy(ReferencedSet, ReducedEntries, random, write);

            var PositiveRelation = Datum.PositiveCorrelation2(ActionSet, ReferencedSet, ReducedEntries, random, write);
            double PositiveDistance = PositiveRelation.CalcDistance();

            var NegativeRelation = Datum.NegativeCorrelation(ActionSet, ReferencedSet, ReducedEntries, random, write);
            double NegativeDistance = NegativeRelation.CalcDistance();

            double DirectionPreAdjustment = PositiveDistance / NegativeDistance;
            double DirectionAdjustment = Sigmoid(DirectionPreAdjustment);

            double[] TotalAdjustment = new double[5];
            for(int i = 0; i < 5; i++)
            {
                double AdjustmentValue = (NegativeRelation[i] + PositiveRelation[i]) * (1 - Accuracy) * DirectionAdjustment;
                ActionSet.Data.Where(x => x.Key == Datum.Key).First().Value[i] += AdjustmentValue;
            }
        }
    }
}
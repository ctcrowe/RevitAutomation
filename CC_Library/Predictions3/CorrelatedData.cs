using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal class CorrelationSet
    {
        public Dataset Positive { get; set; }
        public Dataset Negative { get; set; }

        public CorrelationSet(KeyValuePair<string, double[]> datapoint, Dataset ReferenceSet, Dictionary<string, string> Entries, WriteToCMDLine write)
        {
            Positive = new Dataset(ReferenceSet.datatype);
            Negative = new Dataset(ReferenceSet.datatype);

            foreach (KeyValuePair<string, string> pairs in Entries)
            {
                if (ReferenceSet.datatype == Datatype.TextData)
                {
                    List<string> s = pairs.Key.SplitTitle();
                    foreach (var ReferencedValue in ReferenceSet.Data)
                    {
                        if (s.Contains(ReferencedValue.Key))
                        {
                            if (!Positive.Data.ContainsKey(ReferencedValue.Key))
                            {
                                Positive.Data.Add(ReferencedValue.Key, ReferencedValue.Value);
                            }
                        }
                    }
                }
                else
                {
                    if (ReferenceSet.Data.Any(x => x.Key == pairs.Value))
                    {
                        if (!Positive.Data.ContainsKey(pairs.Value))
                        {
                            KeyValuePair<string, double[]> point = ReferenceSet.Data.Where(x => x.Key == pairs.Value).First();
                            Positive.Data.Add(point.Key, point.Value);
                        }
                    }
                }
            }
            foreach (var Reference in ReferenceSet.Data)
            {
                if (!Positive.Data.ContainsKey(Reference.Key))
                {
                    if (!Negative.Data.ContainsKey(Reference.Key))
                    {
                        Negative.Data.Add(Reference.Key, Reference.Value);
                    }
                }
            }
        }
    }
    internal static class FindCorrelations
    {
        public static double[] Correlation(this KeyValuePair<string, double[]> Datum,
            Dictionary<string, double[]> ReducedSet,
            Dictionary<string, string> EntrySet,
            WriteToCMDLine write, bool CorrelationIsPositive)
        {
            double[] results = new double[Dataset.DataSize];
            double[] Location = new double[Dataset.DataSize];
            
            foreach (var Reduced in ReducedSet)
            {
                double[] Direction = Datum.DirectionBetween(Reduced);
                for (int i = 0; i < Location.Count(); i++)
                {
                    if (CorrelationIsPositive)
                        Location[i] += Direction[i];
                    else
                        Location[i] = 1 / Direction[i];
                }
            }
            Location.Divide(ReducedSet.Count());

            for (int i = 0; i < Dataset.DataSize; i++)
            {
                results[i] = Datum.Value[i] - Location[i];
            }
            return results;
        }
        public static Dictionary<string, double[]> SimilarSet(this KeyValuePair<string, double[]> Datum,
            Dictionary<string, double[]> Dataset,
            WriteToCMDLine write)
        {
            Dictionary<string, double[]> Results = new Dictionary<string, double[]>();
            foreach(var data in Dataset)
            {
                if(data.Key != Datum.Key)
                {
                    Results.Add(data.Key, data.Value);
                }
            }
            return Results;
        }
    }
    internal static class ReducedDataset
    {
        public static Dictionary<string, string> ReducedEntries(
            this KeyValuePair<string, double[]> Referenced,
            Dataset ActionSet,
            Dictionary<string, string> EntrySet,
            WriteToCMDLine write)
        {
            Dictionary<string, string> ReducedEntries = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> Entry in EntrySet)
            {
                if (ActionSet.datatype == Datatype.TextData)
                {
                    List<string> SplitEntry = Entry.Key.SplitTitle();
                    foreach (string s in SplitEntry)
                    {
                        if (s == Referenced.Key)
                        {
                            if (!ReducedEntries.ContainsKey(Entry.Key))
                                ReducedEntries.Add(Entry.Key, Entry.Value);
                            break;
                        }
                    }
                }
                else
                {
                    if (Entry.Value == Referenced.Key)
                        if (!ReducedEntries.ContainsKey(Entry.Key))
                            ReducedEntries.Add(Entry.Key, Entry.Value);
                }
            }
            return ReducedEntries;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    /// <summary>
    /// TODO:
    /// Not all negative correlations are created equal. Find a way to give them more significance than just 1 each!
    /// </summary>
    internal static class CMD_NegativeCorrelation
    {
        public static double[] SingularNegativeCorrelation(this KeyValuePair<string, double[]> ActionDatum,
            KeyValuePair<string, double[]> ReferencedDatum,
            Dataset ActionSet,
            Dataset ReferencedSet,
            Dictionary<string, string> EntrySet,
            Random random,
            WriteToCMDLine write)
        {
            double Accuracy = 0;
            int FinDirection = 1;
            double[] Direction = ReferencedDatum.DirectionBetween(ActionDatum);
            for (int i = 0; i <= 20; i++)
            {
                Dataset ActionSet2 = ActionSet;
                for (int j = 0; j < 5; j++)
                {
                    ActionSet2.Data[ActionDatum.Key][j] += Direction[j] * i / 10;
                }

                double Accuracy2 = ActionSet2.CalcAccuracy(ReferencedSet, EntrySet, random, write);
                if (Accuracy2 > Accuracy)
                {
                    Accuracy = Accuracy2;
                    FinDirection = i;
                }
            }
            for (int i = 0; i < Direction.Count(); i++)
            {
                Direction[i] = Direction[i] * (FinDirection / 10);
            }
            return Direction;
        }
        public static Dictionary<string, double[]> NegativeReferences(this KeyValuePair<string, double[]> Datum, Dataset ActionSet, Dataset ReferencedSet, Dictionary<string, string> EntrySet)
        {
            /*
             * Negative Comparison is used to develop the difference between the entry point and incorrect results.
             * It creates a negative point in the data that the resultant should attempt to move away from.
             * This should help prevent clumping of datapoints as they all seem to want to congregate to the same place.
            */
            List<string> CorrectReferences = new List<string>();
            foreach (KeyValuePair<string, string> pairs in EntrySet)
            {
                if (ActionSet.datatype == Datatype.TextData)
                {
                    if (ReferencedSet.Data.Any(x => x.Key == pairs.Value))
                    {
                        CorrectReferences.Add(pairs.Value);
                    }
                }
                else
                {
                    List<string> s = pairs.Key.SplitTitle();
                    foreach (var ReferencedValue in ReferencedSet.Data)
                    {
                        if (s.Contains(ReferencedValue.Key))
                        {
                            CorrectReferences.Add(ReferencedValue.Key);
                        }
                    }
                }
            }
            Dictionary<string, double[]> NegativeReferences = new Dictionary<string, double[]>();
            foreach (var Reference in ReferencedSet.Data)
            {
                if (!CorrectReferences.Any(x => x == Reference.Key))
                    if(!NegativeReferences.Any(x => x.Key == Reference.Key))
                        NegativeReferences.Add(Reference.Key, Reference.Value);
            }
            return NegativeReferences;
        }
        public static double[] NegativeCorrelation(this KeyValuePair<string, double[]> Datum,
            Dataset ActionSet, Dataset ReferencedSet,
            Dictionary<string, string> EntrySet,
            Random random, WriteToCMDLine write)
        {
            double[] results = new double[5];
            double[] Location = new double[5];

            int count = 0;
            var NegativeReferences = Datum.NegativeReferences(ActionSet, ReferencedSet, EntrySet);

            foreach(var NR in NegativeReferences)
            {
                count++;
                double[] Direction = Datum.SingularPositiveCorrelation(NR,
                    ActionSet,
                    ReferencedSet,
                    EntrySet,
                    random,
                    write);
                for (int i = 0; i < Location.Count(); i++)
                    Location[i] += Direction[i];
            }
            Location.Divide(NegativeReferences.Count());
            
            for(int i = 0; i < 5; i++)
            {
                results[i] = Datum.Value[i] - Location[i];
            }

            return results;
        }
    }
}

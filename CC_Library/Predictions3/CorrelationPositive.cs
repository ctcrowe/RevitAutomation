using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    /// <summary>
    /// TODO:
    /// Not all positive correlations are created equal. Find a way to give them more significance than just 1 each!
    /// </summary>
    internal static class CMD_PositiveCorrelation
    {
        public static double[] SingularPositiveCorrelation(this KeyValuePair<string, double[]> ActionDatum,
            KeyValuePair<string, double[]> ReferencedDatum,
            Dataset ActionSet,
            Dataset ReferencedSet,
            Dictionary<string, string> EntrySet,
            Random random,
            WriteToCMDLine write)
        {
            double Accuracy = 0;
            int FinDirection = 1;
            double[] Direction = ActionDatum.DirectionBetween(ReferencedDatum);
            for(int i = 0; i <= 20; i++)
            {
                Dataset ActionSet2 = ActionSet;
                for (int j = 0; j < 5; j++)
                {
                    ActionSet2.Data[ActionDatum.Key][j] += Direction[j] * i / 10;
                }
                double Accuracy2 = ActionSet2.CalcAccuracy(ReferencedSet, EntrySet, random, write);

                if(Accuracy2 > Accuracy)
                {
                    Accuracy = Accuracy2;
                    FinDirection = i;
                }
            }
            for(int i = 0; i < Direction.Count(); i++)
            {
                Direction[i] = Direction[i] * (FinDirection / 10);
            }
            return Direction;
        }
        public static double[] PositiveCorrelation2(this KeyValuePair<string, double[]> Datum,
            Dataset ActionSet,
            Dataset ReferencedSet,
            Dictionary<string, string> EntrySet,
            Random random,
            WriteToCMDLine write)
        {
            double[] results = new double[5];
            double[] Location = new double[5];

            int count = 0;
            foreach (KeyValuePair<string, string> pairs in EntrySet)
            {
                if (ActionSet.datatype == Datatype.TextData)
                {
                    if (ReferencedSet.Data.Any(x => x.Key == pairs.Value))
                    {
                        count++;
                        double[] Direction = Datum.SingularPositiveCorrelation(ReferencedSet.Data.Where(x => x.Key == pairs.Value).First(),
                            ActionSet,
                            ReferencedSet,
                            EntrySet,
                            random,
                            write);
                        for (int i = 0; i < Location.Count(); i++)
                            Location[i] += Direction[i];
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, double[]> ReferencedValue in ReferencedSet.Data)
                    {
                        if (pairs.Key.SplitTitle().Contains(ReferencedValue.Key))
                        {
                            count++;
                            double[] Direction = Datum.SingularPositiveCorrelation(ReferencedValue,
                                ActionSet,
                                ReferencedSet,
                                EntrySet,
                                random,
                                write);
                            for (int i = 0; i < Location.Count(); i++)
                                Location[i] += Direction[i];
                        }
                    }
                }
            }
            Location.Divide(count);

            for (int i = 0; i < 5; i++)
            {
                results[i] = Datum.Value[i] - Location[i];
            }
            return results;
        }
        public static double[] PositiveCorrelation(this KeyValuePair<string, double[]> Datum, Dataset ActionSet, Dataset ReferencedSet, Dictionary<string, string> EntrySet)
        {
            double[] results = new double[5];
            double[] Location = new double[5];
            
            int count = 0;
            foreach (KeyValuePair<string, string> pairs in EntrySet)
            {
                if (ActionSet.datatype == Datatype.TextData)
                {
                    if (ReferencedSet.Data.Any(x => x.Key == pairs.Value))
                    {
                        double[] Direction = Datum.DirectionBetween(ReferencedSet.Data.Where(x => x.Key == pairs.Value).First());
                        count++;
                        for (int i = 0; i < Location.Count(); i++)
                            Location[i] += Direction[i];
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, double[]> ReferencedValue in ReferencedSet.Data)
                    {
                        if (pairs.Key.SplitTitle().Contains(ReferencedValue.Key))
                        {
                            count++;
                            double[] Direction = Datum.DirectionBetween(ReferencedValue);
                            for (int i = 0; i < Location.Count(); i++)
                                Location[i] += Direction[i];
                        }
                    }
                }
            }
            Location.Divide(count);

            for (int i = 0; i < 5; i++)
            {
                results[i] = Datum.Value[i] - Location[i];
            }
            return results;
        }
    }
}

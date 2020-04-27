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
            WriteToCMDLine write)
        {
            double Accuracy = 0;
            double FinDirection = 10;
            double[] Direction = ActionDatum.DirectionBetween(ReferencedDatum);
            for(double i = 1; i <= 20; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    ActionSet.Data[ActionDatum.Key][j] += Direction[j] * i / 10;
                }

                double Accuracy2 = ActionSet.CalcAccuracy(ReferencedSet, EntrySet, write);

                for (int j = 0; j < 5; j++)
                {
                    ActionSet.Data[ActionDatum.Key][j] -= Direction[j] * i / 10;
                }

                if (Accuracy2 > Accuracy)
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
        public static double[] PositiveCorrelation(this KeyValuePair<string, double[]> Datum,
            Dataset ActionSet,
            Dataset PositiveSet,
            Dictionary<string, string> EntrySet,
            WriteToCMDLine write)
        {
            double[] results = new double[5];
            double[] Location = new double[5];

            int count = 0;
            foreach(var Positive in PositiveSet.Data)
            {
                count++;
                double[] Direction = Datum.SingularPositiveCorrelation(Positive,
                    ActionSet,
                    PositiveSet,
                    EntrySet,
                    write);
                for (int i = 0; i < Location.Count(); i++)
                    Location[i] += Direction[i];
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

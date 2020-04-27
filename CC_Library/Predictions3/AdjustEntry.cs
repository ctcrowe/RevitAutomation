using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class AdjustEntry
    {
        public static void ChartEntry(this KeyValuePair<string, double[]> Datum,
            Dataset ActionSet,
            Dataset ReferencedSet,
            Dictionary<string, string> EntrySet,
            WriteToCMDLine write)
        {
            /*
             * Rather than randomly adjusting a value when its wrong or trying to chart out what direction it should move like this,
             * Simplify the system to find the location of the "Correct" Value and move everything closer together.
             * The "Direction" That something should move should be a ratio relative to the Accuracy of the independent and the direction that it should move in.
             * Direction is going to be the vector between the action datum and the relevant reference datum (coordinated through the entry set)
             * ResultantVector = Sum of all phrases
             * Return Vector = Direction from resultant vector towards correct reference vector
            */

            /*
             * 3 Vectors to calculate for adjustment:
             *  1 - Positive Vector - the resultant of all correct answers.
             *  2 - Negative Vector - The resultant of all incorrect answers.
             *  3 - Similar Vector - The resultant of all other options in the given dataset.
            */

            Dictionary<string, string> ReducedEntries = Datum.ReducedEntries(ActionSet, EntrySet, write);
            double Accuracy = ActionSet.CalcAccuracy(ReferencedSet, ReducedEntries, write);

            CorrelationSet Relations = new CorrelationSet(Datum, ReferencedSet, ReducedEntries, write);

            //The positive vector
            var PositiveRelation = Datum.Correlation(Relations.Positive.Data, ReducedEntries, write, true);
            //Normalized Positive Vector
            var PositiveNormal = PositiveRelation.NormalizeVector();
            //double PositiveDistance = PositiveRelation.CalcDistance(write);

            //The negative vector
            var NegativeRelation = Datum.Correlation(Relations.Negative.Data, ReducedEntries, write, false);
            //Normalized Negative Vector
            var NegativeNormal = NegativeRelation.NormalizeVector();
            //double NegativeDistance = NegativeRelation.CalcDistance(write);

            //The Similar Vector
            var SimilarRelation = Datum.Correlation(Datum.SimilarSet(ActionSet.Data, write), EntrySet, write, false);
            //Normalized Similar Vector
            var SimilarNormal = SimilarRelation.NormalizeVector();
            //double SimilarDistance = SimilarNormal.CalcDistance(write);

            double NegativeChange = 0.01;
            double PositiveChange = 0.02;
            double SimilarChange = 0.01;
            
            double OldAccuracy = ActionSet.CalcAccuracy(ReferencedSet, ReducedEntries, write);

            var CopySet = ActionSet.Clone();
            var CopyPoint = CopySet.Data.Where(x => x.Key == Datum.Key).First();

            for (double pos = 0; pos <= 0.1; pos += 0.01)
            {
                CopyPoint.AdustValue(PositiveNormal, pos);
                for(double neg = 0; neg <= 0.1; neg += 0.01)
                {
                    CopyPoint.AdustValue(NegativeNormal, -1 * neg);
                    for(double sim = 0; sim <= 0.1; sim += 0.01)
                    {
                        CopyPoint.AdustValue(SimilarNormal, -1 * sim);
                        double NewAccuracy = CopySet.CalcAccuracy(ReferencedSet, ReducedEntries, write);
                        if(NewAccuracy > OldAccuracy)
                        {
                            NegativeChange = neg;
                            PositiveChange = pos;
                            SimilarChange = sim;
                            OldAccuracy = NewAccuracy;
                        }
                    }
                }
            }

            Datum.AdustValue(PositiveNormal, PositiveChange);
            Datum.AdustValue(NegativeNormal, -1 * NegativeChange);
            Datum.AdustValue(SimilarNormal, -1 * SimilarChange);

            Datum.Value.WriteArray(Datum.Key, write);
        }
        public static void AdustValue(this KeyValuePair<string, double[]> datapoint, double[] values, double adjustment)
        {
            for(int i = 0; i < datapoint.Value.Count(); i++)
            {
                double MaxChange = 1 - datapoint.Value[i];
                double MinChange = -1 - datapoint.Value[i];
                double change = values[i] * adjustment;

                if(change < MaxChange && change > MinChange)
                    datapoint.Value[i] += (values[i] * adjustment);
                else
                {
                    if (change > MaxChange)
                        datapoint.Value[i] += MaxChange;
                    if (change < MinChange)
                        datapoint.Value[i] -= MinChange;
                }
            }
        }
    }
}

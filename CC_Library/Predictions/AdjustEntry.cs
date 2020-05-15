using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class AdjustEntry
    {
        public static void ChartEntry(this KeyValuePair<string, double[]> Datum,
            Dataset ActionSet,
            Dataset ReferencedSet,
            Dictionary<string, string[]> EntrySet,
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

            Dictionary<string, string[]> ReducedEntries = Datum.ReducedEntries(ActionSet, EntrySet, write);
            CorrelationSet Relations = new CorrelationSet(Datum, ReferencedSet, ReducedEntries, write);

            //The positive vector
            var PositiveRelation = Datum.Correlation(Relations.Positive.Data, write, true);
            //The negative vector
            var NegativeRelation = Datum.Correlation(Relations.Negative.Data, write, false);
            //The Similar Vector
            var SimilarRelation = Datum.Correlation(Datum.SimilarSet(ActionSet.Data, write), write, false);

            double NegativeChange = -0.1;
            double PositiveChange = 0.1;
            double SimilarChange = -0.1;
            double OldAccuracy = ActionSet.CalcAccuracy(ReferencedSet, ReducedEntries, true, 0);

            for (double pos = 0.1; pos >= 0; pos -= 0.01)
            {
                for (double neg = -0.1; neg <= 0; neg += 0.01)
                {
                    for (double sim = -0.1; sim <= 0; sim += 0.01)
                    {
                        var CopySet = ActionSet.Clone();
                        var CopyPoint = CopySet.Data.Where(x => x.Key == Datum.Key).First();
                        CopyPoint.AdustValue(PositiveRelation, pos, null, write, false);
                        CopyPoint.AdustValue(NegativeRelation, neg, null, write, false);
                        if (ActionSet.datatype != Datatype.TextData)
                            CopyPoint.AdustValue(SimilarRelation, sim, null, write, false);
                        double NewAccuracy = CopySet.CalcAccuracy(ReferencedSet, ReducedEntries, true, 0);
                        if (NewAccuracy > OldAccuracy)
                        {
                            NegativeChange = neg;
                            PositiveChange = pos;
                            if (ActionSet.datatype != Datatype.TextData)
                                SimilarChange = sim;
                            OldAccuracy = NewAccuracy;
                        }
                    }
                }
            }

            Datum.AdustValue(NegativeRelation, NegativeChange, "Negative", write, true);
            if (ActionSet.datatype != Datatype.TextData)
                Datum.AdustValue(SimilarRelation, SimilarChange, "Similar", write, true);

            if (Relations.Positive.Data.Any())
            {
                Datum.AdustValue(PositiveRelation, PositiveChange, "Positive", write, true);
                double Accuracy = ActionSet.CalcAccuracy(ReferencedSet, ReducedEntries, true, 0);
                double LengthVector = 1;

                for (double i = 0.95; i <= 1.05; i++)
                {
                    var CopySet = ActionSet.Clone();
                    var CopyPoint = CopySet.Data.Where(x => x.Key == Datum.Key).First();

                    for (int j = 0; j < Dataset.DataSize; j++)
                    {
                        CopyPoint.Value[j] *= i;
                    }
                    double NewAccuracy = CopySet.CalcAccuracy(ReferencedSet, ReducedEntries, true, 0);
                    if (NewAccuracy > Accuracy)
                    {
                        LengthVector = i;
                    }
                }

                for (int i = 0; i < Dataset.DataSize; i++)
                {
                    if (Datum.Value[i] * LengthVector <= 1 && Datum.Value[i] * LengthVector >= -1)
                        Datum.Value[i] *= LengthVector;
                    else
                    {
                        if (Datum.Value[i] * LengthVector >= 1)
                            Datum.Value[i] = 1;
                        if (Datum.Value[i] * LengthVector <= -1)
                            Datum.Value[i] = -1;
                    }
                }
                write(Datum.Key + " ; Distance : " + Datum.Value.CalcDistance());
            }
        }
        public static void AdustValue(this KeyValuePair<string, double[]> datapoint, double[] values, double adjustment, string type, WriteToCMDLine write, bool t)
        {
            double DataLength = datapoint.Value.VectorLength();
            double[] Normal = datapoint.Value.Normalize();
            double[] ValueNorm = values.Normalize();
            double[] ChangeDirection = ValueNorm.DirectionBetween(Normal);
            if(t) write(type + " : " + ChangeDirection.VectorLength());

            for(int i = 0; i < datapoint.Value.Count(); i++)
            {
                if (ChangeDirection[i] > 0)
                {
                    double change = ChangeDirection[i] * adjustment;

                    Normal[i] += change;
                    datapoint.Value[i] = Normal[i] * DataLength;
                }
            }
        }
    }
}

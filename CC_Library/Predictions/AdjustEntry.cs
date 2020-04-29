using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

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

            //The negative vector
            var NegativeRelation = Datum.Correlation(Relations.Negative.Data, ReducedEntries, write, false);
            //Normalized Negative Vector
            var NegativeNormal = NegativeRelation.NormalizeVector();

            //The Similar Vector
            var SimilarRelation = Datum.Correlation(Datum.SimilarSet(ActionSet.Data, write), EntrySet, write, false);
            //Normalized Similar Vector
            var SimilarNormal = SimilarRelation.NormalizeVector();

            double NegativeChange = 0.03;
            double PositiveChange = 0.05;
            double SimilarChange = 0.03;
            
            double OldAccuracy = ActionSet.CalcAccuracy(ReferencedSet, ReducedEntries, write);

            var TestSet = ActionSet.Clone();
            var TestPoint = TestSet.Data.Where(x => x.Key == Datum.Key).First();
            TestPoint.AdustValue(PositiveNormal, PositiveChange);
            double TestAccuracy = TestSet.CalcAccuracy(ReferencedSet, ReducedEntries, write);
            if (TestAccuracy < OldAccuracy)
                PositiveChange = 0;

            TestSet = ActionSet.Clone();
            TestPoint = TestSet.Data.Where(x => x.Key == Datum.Key).First();
            TestPoint.AdustValue(NegativeNormal, -1 * NegativeChange);
            TestAccuracy = TestSet.CalcAccuracy(ReferencedSet, ReducedEntries, write);
            if (TestAccuracy < OldAccuracy)
                NegativeChange = 0;

            TestSet = ActionSet.Clone();
            TestPoint = TestSet.Data.Where(x => x.Key == Datum.Key).First();
            TestPoint.AdustValue(SimilarNormal, -1 * SimilarChange);
            TestAccuracy = TestSet.CalcAccuracy(ReferencedSet, ReducedEntries, write);
            if (TestAccuracy < OldAccuracy)
                SimilarChange = 0;

            for (double pos = 0; pos <= 0.1; pos += 0.01)
            {
                for(double neg = 0; neg <= 0.1; neg += 0.01)
                {
                    for (double sim = 0; sim <= 0.1; sim += 0.01)
                    {
                        var CopySet = ActionSet.Clone();
                        var CopyPoint = CopySet.Data.Where(x => x.Key == Datum.Key).First();
                        CopyPoint.AdustValue(PositiveNormal, pos);
                        CopyPoint.AdustValue(NegativeNormal, -1 * neg);
                        if(ActionSet.datatype != Datatype.TextData)
                            CopyPoint.AdustValue(SimilarNormal, -1 * sim);
                        double NewAccuracy = CopySet.CalcAccuracy(ReferencedSet, ReducedEntries, write);
                        if (NewAccuracy > OldAccuracy)
                        {
                            NegativeChange = neg;
                            PositiveChange = pos;
                            if(ActionSet.datatype != Datatype.TextData)
                                SimilarChange = sim;
                            OldAccuracy = NewAccuracy;
                        }
                    }
                }
            }

            Datum.AdustValue(PositiveNormal, PositiveChange);
            Datum.AdustValue(NegativeNormal, -1 * NegativeChange);
            if (ActionSet.datatype != Datatype.TextData)
                Datum.AdustValue(SimilarNormal, -1 * SimilarChange);

            double AccuracyRange = OldAccuracy;
            //The current direction of the normal. This prevents the model from congregating to 0.
            double[] CurrentVector = new double[Dataset.DataSize];
            double CurrentChange = 0.005;
            
            for (int i = 0; i < CurrentVector.Count(); i++)
            {
                CurrentVector[i] = Datum.Value[i];
            }
            var CurrentDistance = CurrentVector.CalcDistance();
            var CurrentNormal = CurrentVector.NormalizeVector();
            if(CurrentDistance > 1)
            {
                for(int i = 0; i < Dataset.DataSize; i++)
                    CurrentNormal[i] *= -1;
            }


            TestSet = ActionSet.Clone();
            TestPoint = TestSet.Data.Where(x => x.Key == Datum.Key).First();
            TestPoint.AdustValue(CurrentNormal, CurrentChange);
            TestAccuracy = TestSet.CalcAccuracy(ReferencedSet, ReducedEntries, write);
            if (TestAccuracy < AccuracyRange)
                CurrentChange = 0;

            for (double cur = 0; cur <= 0.1; cur += 0.01)
            {
                var CopySet = ActionSet.Clone();
                var CopyPoint = CopySet.Data.Where(x => x.Key == Datum.Key).First();
                
                CopyPoint.AdustValue(CurrentNormal, cur);

                double NewAccuracy = CopySet.CalcAccuracy(ReferencedSet, ReducedEntries, write);
                if (NewAccuracy > AccuracyRange)
                {
                    CurrentChange = cur;
                    AccuracyRange = NewAccuracy;
                }
            }

            string s = Datum.Key + " ; Current : " + CurrentChange + ", Positive : " + PositiveChange + ", Negative : " + NegativeChange;
            if (ActionSet.datatype != Datatype.TextData)
                s += ", Similar : " + SimilarChange;
            write(s);
            write(Datum.Key + " ; Distance : " + Datum.Value.CalcDistance());
            Datum.AdustValue(CurrentNormal, CurrentChange);
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

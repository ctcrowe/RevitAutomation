using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CC_Library.Predictions.Masterformat
{
    internal static class Predictions_ChartDataset
    {
        public static void ChartDataset(this KeyValuePair<string, Data> Datum,
            Dictionary<string, Data> Dataset,
            Dictionary<string, Data> Dictionary,
            Dictionary<string, string[]> Entries,
            CalcAccuracy accuracy,
            WriteToCMDLine write)
        {
            Dictionary<string, string[]> ReducedEntries = Datum.ReducedEntries(Entries, write);
            var Positive = Dictionary.Where(x => ReducedEntries.Any(y => y.Key.SplitTitle().Any(z => z == x.Key)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var Negative = Dictionary.Where(x => !Positive.ContainsKey(x.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var Similar = Dictionary.Where(x => x.Key != Datum.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var PositiveRelation = Datum.Correlation(Positive, write, true);
            var NegativeRelation = Datum.Correlation(Negative, write, false);
            var SimilarRelation = Datum.Correlation(Similar, write, false);

            double NegativeChange = -0.1;
            double PositiveChange = 0.1;
            double SimilarChange = -0.1;
            double OldAccuracy = accuracy(Dataset, Dictionary, ReducedEntries);

            Parallel.For(0, 100, i =>
            {
                double pos = i / 10;
                double neg = -i % 10;

                var CopySet = Dataset.Clone();
                var CopyPoint = CopySet.Where(x => x.Key == Datum.Key).First();
                CopyPoint.AdjustLocation(PositiveRelation, pos);
                CopyPoint.AdjustLocation(NegativeRelation, neg);
                double NewAccuracy = accuracy(CopySet, Dictionary, ReducedEntries);
                if (NewAccuracy > OldAccuracy)
                {
                    NegativeChange = neg;
                    PositiveChange = pos;
                    OldAccuracy = NewAccuracy;
                }
            });

            Datum.AdjustLocation(NegativeRelation, NegativeChange);
            Datum.AdjustLocation(SimilarRelation, SimilarChange);
            Datum.AdjustLocation(PositiveRelation, PositiveChange);

            OldAccuracy = accuracy(Dataset, Dictionary, ReducedEntries);

            double LengthVector = 1;

            double[] FinLength = new double[Datum.Value.Location.Count()];
            for (int i = 0; i < Datum.Value.Location.Count(); i++)
            {
                FinLength[i] = Datum.Value.Location[i];
            }
            FinLength = FinLength.NormalizeVector();

            Parallel.For(1, 200, i =>
            {
                double adjustment = i / 100.00;
                var CopySet = Dataset.Clone();
                var DatumCopy = CopySet.Where(x => x.Key == Datum.Key).First();
                double[] Location = DatumCopy.Value.Location.NormalizeVector();

                for (int j = 0; j < DatumCopy.Value.Location.Count(); j++)
                {
                    DatumCopy.Value.Location[j] = Location[j] * adjustment;
                }
                double NewAccuracy = accuracy(CopySet, Dictionary, Entries);
                if (NewAccuracy > OldAccuracy)
                {
                    LengthVector = adjustment;
                    FinLength = DatumCopy.Value.Location;
                    OldAccuracy = NewAccuracy;
                }
            });
            Datum.AdjustDistance(FinLength);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class ChartDictionary
    {
        public static void ChartDict(this KeyValuePair<string, Data> Datum,
            Dictionary<string, Data> ConnectedData,
            Dictionary<string, Data> Dictionary,
            Dictionary<string, string[]> EntrySet,
            CalcAccuracy accuracy,
            WriteToCMDLine write)
        {
            Dictionary<string, string[]> ReducedEntries = Datum.ReducedEntries(EntrySet, write);
            var Positive = ConnectedData.Where(x => ReducedEntries.Any(y => y.Value.Any(z => z == x.Key)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var Negative = ConnectedData.Where(x => !Positive.Any(y => y.Key == x.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var PositiveRelation = Datum.Correlation(Positive, write, true);
            var NegativeRelation = Datum.Correlation(Negative, write, false);

            double NegativeChange = 0;
            double PositiveChange = 0.1;
            double OldAccuracy = accuracy(ConnectedData, Dictionary, ReducedEntries);

            Parallel.For(0, 10, i =>
            {
                double pos = 0.1 - (i / 10.0);
                for (double neg = -0.1; neg <= 0; neg += 0.01)
                {
                    for (double sim = -0.1; sim <= 0; sim += 0.01)
                    {
                        var CopySet = Dictionary.Clone();
                        var CopyPoint = CopySet.Where(x => x.Key == Datum.Key).First();
                        CopyPoint.AdjustLocation(PositiveRelation, pos);
                        CopyPoint.AdjustLocation(NegativeRelation, neg);
                        double NewAccuracy = accuracy(ConnectedData, CopySet, ReducedEntries);
                        if (NewAccuracy > OldAccuracy)
                        {
                            NegativeChange = neg;
                            PositiveChange = pos;
                            OldAccuracy = NewAccuracy;
                        }
                    }
                }
            });

            Datum.AdjustLocation(NegativeRelation, NegativeChange);
            Datum.AdjustLocation(PositiveRelation, PositiveChange);

            OldAccuracy = accuracy(ConnectedData, Dictionary, ReducedEntries);

            double LengthVector = 1;

            double[] FinLength = new double[Datum.Value.Location.Count()];
            for(int i = 0; i < Datum.Value.Location.Count(); i++)
            {
                FinLength[i] = Datum.Value.Location[i];
            }
            FinLength = FinLength.NormalizeVector();

            Parallel.For(1, 200, i =>
            {
                double adjustment = i / 100.00;
                var DictCopy = Dictionary.Clone();
                var DatumCopy = DictCopy.Where(x => x.Key == Datum.Key).First();
                double[] Location = DatumCopy.Value.Location.NormalizeVector();

                for (int j = 0; j < DatumCopy.Value.Location.Count(); j++)
                {
                    DatumCopy.Value.Location[j] = Location[j] * adjustment;
                }
                double NewAccuracy = accuracy(ConnectedData, DictCopy, ReducedEntries);
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

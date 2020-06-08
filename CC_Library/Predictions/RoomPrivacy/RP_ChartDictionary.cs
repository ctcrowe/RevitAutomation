using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC_Library.Predictions.RoomPrivacy
{
    internal static class CC_RP_ChartDictionary
    {
        public static void ChartDict(this KeyValuePair<string, Data> Datum,
            Dictionary<string, Data> RPData,
            Dictionary<string, Data> DictData,
            Dictionary<string, string[]> EntrySet,
            WriteToCMDLine write)
        {
            Dictionary<string, string[]> ReducedEntries = Datum.ReducedEntries(EntrySet, write);
            var Positive = RPData.Where(x => ReducedEntries.Any(y => y.Value.Any(z => z == x.Key)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var Negative = RPData.Where(x => !Positive.Any(y => y.Key == x.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var PositiveRelation = Datum.Correlation(Positive, write, true);
            var NegativeRelation = Datum.Correlation(Negative, write, false);

            double NegativeChange = -0.1;
            double PositiveChange = 0.1;
            double OldAccuracy = RPData.Accuracy(DictData, ReducedEntries);

            Parallel.For(0, 10, i =>
            {
                double pos = 0.1 - (i / 10.0);
                for (double neg = -0.1; neg <= 0; neg += 0.01)
                {
                    var CopySet = DictData.Clone();
                    var CopyPoint = CopySet.Where(x => x.Key == Datum.Key).First();
                    CopyPoint.AdjustLocation(PositiveRelation, pos);
                    CopyPoint.AdjustLocation(NegativeRelation, neg);
                    double NewAccuracy = RPData.Accuracy(CopySet, ReducedEntries);
                    if (NewAccuracy > OldAccuracy)
                    {
                        NegativeChange = neg;
                        PositiveChange = pos;
                        OldAccuracy = NewAccuracy;
                    }
                }
            });

            Datum.AdjustLocation(NegativeRelation, NegativeChange);
            Datum.AdjustLocation(PositiveRelation, PositiveChange);
            double Accuracy = RPData.Accuracy(DictData, ReducedEntries);
            double LengthVector = 1;

            Parallel.For(60, 105, i =>
            {
                double adjustment = i / 100.00;
                var CopySet = DictData.Clone();
                var CopyPoint = CopySet.Where(x => x.Key == Datum.Key).First();

                for (int j = 0; j < Datum.Value.Datatype.Count(); j++)
                {
                    CopyPoint.Value.Location[j] *= adjustment;
                }
                double NewAccuracy = DictData.Accuracy(CopySet, ReducedEntries);
                if (NewAccuracy > Accuracy)
                {
                    LengthVector = adjustment;
                    Accuracy = NewAccuracy;
                }
            });

            for (int i = 0; i < Datum.Value.Datatype.Count(); i++)
            {
                if (Datum.Value.Location[i] * LengthVector <= 1 && Datum.Value.Location[i] * LengthVector >= -1)
                    Datum.Value.Location[i] *= LengthVector;
                else
                {
                    if (Datum.Value.Location[i] * LengthVector >= 1)
                        Datum.Value.Location[i] = 1;
                    if (Datum.Value.Location[i] * LengthVector <= -1)
                        Datum.Value.Location[i] = -1;
                }
            }
        }
    }
}

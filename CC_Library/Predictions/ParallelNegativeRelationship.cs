using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class ParallelNegativeRelationship
    {
        public static double[] NegativeChange(this Dictionary<string, Data> Dataset,
            string Label,
            Dictionary<string, Data> Negative,
            Dictionary<string, string[]> EntrySet,
            WriteToCMDLine write,
            CalcAccuracy accuracy)
        {
            /*
            Dictionary<string, string[]> ReducedEntries = Datum.ReducedEntries(EntrySet, write);
            var Positive = DictData.Where(x => ReducedEntries.Any(y => y.Key.SplitTitle().Any(z => z == x.Key)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            */
            double BaseAccuracy = accuracy(Dataset, Negative, EntrySet);
            double[] Values = new double[Dataset.FirstOrDefault().Value.Location.Count()];
            Parallel.For(0, Negative.Count(), i =>
            {
                double NegativeChange = 0.1;
                double[] direction = Dataset.Where(x => x.Key == Label).First().DirectionBetween(Negative.ElementAt(i));

                for (double neg = 0; neg < 0.15; neg += 0.01)
                {
                    var CopySet = Dataset.Clone();
                    var CopyPoint = CopySet.Where(x => x.Key == Label).First();
                    CopyPoint.AdjustLocation(direction, neg, false);
                    double AccuracyTest = accuracy(CopySet, Negative, EntrySet);
                    if (AccuracyTest > BaseAccuracy)
                    {
                        NegativeChange = neg;
                    }
                }
                for (int j = 0; j < direction.Count(); j++)
                {
                    Values[j] -= direction[j] * NegativeChange;
                }
            });
            return Values;
        }
    }
}

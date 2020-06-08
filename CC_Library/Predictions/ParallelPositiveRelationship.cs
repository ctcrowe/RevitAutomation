using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class ParallelPositiveRelationship
    {
        public static double[] PositiveChange(this Dictionary<string, Data> Dataset,
            string Label,
            Dictionary<string, Data> Positive,
            Dictionary<string, string[]> EntrySet,
            WriteToCMDLine write,
            CalcAccuracy accuracy )
        {
            /*
            Dictionary<string, string[]> ReducedEntries = Datum.ReducedEntries(EntrySet, write);
            var Positive = DictData.Where(x => ReducedEntries.Any(y => y.Key.SplitTitle().Any(z => z == x.Key)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            */
            double BaseAccuracy = accuracy(Dataset, Positive, EntrySet);
            double[] Values = new double[Dataset.FirstOrDefault().Value.Location.Count()];
            Parallel.For(0, Positive.Count(), i =>
            {
                double PositiveChange = 0.1;
                double[] direction = Dataset.Where(x => x.Key == Label).First().DirectionBetween(Positive.ElementAt(i));

                for(double pos = 0; pos < 0.15; pos += 0.01)
                {
                    var CopySet = Dataset.Clone();
                    var CopyPoint = CopySet.Where(x => x.Key == Label).First();
                    CopyPoint.AdjustLocation(direction, pos, true);
                    double AccuracyTest = accuracy(CopySet, Positive, EntrySet);
                    if(AccuracyTest > BaseAccuracy)
                    {
                        PositiveChange = pos;
                    }
                }
                for (int j = 0; j < direction.Count(); j++)
                {
                    Values[j] += direction[j] * PositiveChange;
                }
            });
            return Values;
        }
    }
}

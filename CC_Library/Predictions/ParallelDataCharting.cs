using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace CC_Library.Predictions.Masterformat
{
    internal static class ParallelChartDataset
    {
        public static bool ParallelCharting
            (this Dictionary<string, Data> Dataset,
           Dictionary<string, Data> ComparisonSet,
           Dictionary<string, string[]> Entries,
           List<double[]> ArrayOptions,
            CalcAccuracy accuracy,
            WriteToCMDLine write)
        {
            double BaseAccuracy;
            if (Dataset.FirstOrDefault().Value.Datatype == Datatypes.Datatype.Dictionary)
                BaseAccuracy = accuracy(ComparisonSet, Dataset, Entries);
            else
                BaseAccuracy = accuracy(Dataset, ComparisonSet, Entries);

            double OldAccuracy = BaseAccuracy;
            double[] ResultantChange = new double[Dataset.Count() * 2];

            Parallel.For(0, ArrayOptions.Count() / 2, i =>
            {
                var CopySet = Dataset.Clone();
                for (int j = 0; j < ArrayOptions[i].Count() / 2; j++)
                {
                    var CopyPoint = CopySet.ElementAt(j);
                    CopyPoint.AdjustLocation(CopyPoint.Value.PositiveDirection, ArrayOptions[i][j] * BaseAccuracy);
                    CopyPoint.AdjustLocation(CopyPoint.Value.NegativeDirection, ArrayOptions[i][j + Dataset.Count()] * BaseAccuracy);
                }
                double NewAccuracy;
                if (Dataset.FirstOrDefault().Value.Datatype == Datatypes.Datatype.Dictionary)
                    NewAccuracy = accuracy(ComparisonSet, Dataset, Entries);
                else
                    NewAccuracy = accuracy(Dataset, ComparisonSet, Entries);

                if (NewAccuracy > OldAccuracy)
                {
                    ResultantChange = ArrayOptions[i];
                    OldAccuracy = NewAccuracy;
                }
            });
            write("Options Changed : " + ResultantChange.Where(x => x != 0).Count() + " out of " + ResultantChange.Count() + " Possible Changes.");
            write("Total Options : " + ArrayOptions.Count());

            for (int j = 0; j < ResultantChange.Count() / 2; j++)
            {
                var DataPoint = Dataset.ElementAt(j);
                DataPoint.AdjustLocation(DataPoint.Value.PositiveDirection, ResultantChange[j]);
                DataPoint.AdjustLocation(DataPoint.Value.NegativeDirection, ResultantChange[j + Dataset.Count()]);
            }
            write("Accuracy is " + OldAccuracy);
            return true;
        }
        public static bool ParallelDataCharting
            (this Dictionary<string, Data> Dataset,
            Dictionary<string, Data> ComparisonSet,
            Dictionary<string, string[]> Entries,
            CalcAccuracy accuracy,
            WriteToCMDLine write)
        {
            double BaseAccuracy;
            if (Dataset.FirstOrDefault().Value.Datatype == Datatypes.Datatype.Dictionary)
                BaseAccuracy = accuracy(ComparisonSet, Dataset, Entries);
            else
                BaseAccuracy = accuracy(Dataset, ComparisonSet, Entries);

            double OldAccuracy = BaseAccuracy;
            double[] DataChange = new double[Dataset.Count() * 2];
            double[] ResultantChange = new double[Dataset.Count() * 2];
            int count = 0;
            while(true)
            {
                var CopySet = Dataset.Clone();
                for (int j = 0; j < DataChange.Count() / 2; j++)
                {
                    var CopyPoint = CopySet.ElementAt(j);
                    CopyPoint.AdjustLocation(CopyPoint.Value.PositiveDirection, DataChange[j] * BaseAccuracy);
                    CopyPoint.AdjustLocation(CopyPoint.Value.NegativeDirection, DataChange[j + Dataset.Count()] * BaseAccuracy);
                }
                double NewAccuracy;
                if (Dataset.FirstOrDefault().Value.Datatype == Datatypes.Datatype.Dictionary)
                    NewAccuracy = accuracy(ComparisonSet, Dataset, Entries);
                else
                    NewAccuracy = accuracy(Dataset, ComparisonSet, Entries);

                if (NewAccuracy > OldAccuracy)
                {
                    ResultantChange = DataChange;
                    OldAccuracy = NewAccuracy;
                }
                count++;

                if (DataChange.Any(x => x != 0.5))
                    DataChange = DataChange.FindNextArray();
                else
                {
                    write("Options Changed : " + ResultantChange.Where(x => x == 0.1).Count() + " Out of " + ResultantChange.Count() + " Possible Changes.");
                    write("Total Options : " + count);
                    break;
                }
            }

            for (int j = 0; j < ResultantChange.Count() / 2; j++)
            {
                var DataPoint = Dataset.ElementAt(j);
                DataPoint.AdjustLocation(DataPoint.Value.PositiveDirection, ResultantChange[j]);
                DataPoint.AdjustLocation(DataPoint.Value.NegativeDirection, ResultantChange[j + Dataset.Count()]);
            }
            write("Accuracy is " + OldAccuracy);
            return true;
        }
        private static double[] FindNextArray(this double[] PrevArray)
        {
            double[] newarray = new double[PrevArray.Count()];

            for (int i = 0; i < PrevArray.Count(); i++)
            {

                newarray[i] = PrevArray[i];
            }

            for (int i = 0; i < PrevArray.Count() - 1; i++)
            {
                if (PrevArray[i + 1] == 0.5)
                {
                    if (newarray[i] == 0.5)
                        newarray[i] = 0;
                    else
                        newarray[i] += 0.25;
                }
            }

            if (PrevArray[PrevArray.Count() - 1] != 0.5)
                newarray[PrevArray.Count() - 1] += 0.25;
            else
                newarray[PrevArray.Count() - 1] = 0;

            return newarray;
        }
    }
}

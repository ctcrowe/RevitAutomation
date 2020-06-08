using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CC_Library.Predictions
{
    internal static class ParallelDirectionalChangeTest
    {
        public static void ChartElement(this KeyValuePair<string, Data> Datum,
            Dictionary<string, Data> Dataset,
            Dictionary<string, Data> Dict,
            Dictionary<string, string[]> Entries,
            WriteToCMDLine write,
            CalcAccuracy acc)
        {
            double BaseAcc = acc(Dataset, Dict, Entries);
            double[] LocationChange = new double[Datum.Value.Location.Count()];

            List<double[]> PosArrays = PossibleArrays(Datum.Value.Location.Count());
            Parallel.For(0, PosArrays.Count(), i =>
            {
                double[] ChangeValues = PosArrays[i];
                double[] NewLoc = new double[LocationChange.Count()];
                var CopySet = Dataset.Clone();
                var CopyPoint = CopySet.Where(x => x.Key == Datum.Key).First();

                for(int j = 0; j < CopyPoint.Value.Location.Count(); j++)
                {
                    double change = ChangeValues[j] - 1;

                    if(change > 0)
                        NewLoc[j] = (1 - Datum.Value.Location[j]) * BaseAcc * change;
                    else
                        NewLoc[j] = ((-1) + Datum.Value.Location[j]) * BaseAcc * -1 * change;

                    CopyPoint.Value.Location[j] += NewLoc[j];
                }
                double NewAcc = acc(CopySet, Dict, Entries);
                if (NewAcc > BaseAcc)
                {
                    LocationChange = NewLoc;
                }
            });

            Datum.Move(LocationChange);
        }
        public static void ChartWord(this KeyValuePair<string, Data> Datum,
            Dictionary<string, Data> Dataset,
            Dictionary<string, Data> Dict,
            Dictionary<string, string[]> Entries,
            WriteToCMDLine write,
            CalcAccuracy acc)
        {
            double BaseAcc = acc(Dataset, Dict, Entries);
            double[] LocationChange = new double[Datum.Value.Location.Count()];

            List<double[]> PosArrays = PossibleArrays(Datum.Value.Location.Count());
            Parallel.For(0, PosArrays.Count(), i =>
            {
                double[] ChangeValues = PosArrays[i];
                double[] NewLoc = new double[LocationChange.Count()];
                var CopySet = Dict.Clone();
                var CopyPoint = CopySet.Where(x => x.Key == Datum.Key).First();

                for (int j = 0; j < CopyPoint.Value.Location.Count(); j++)
                {
                    double change = ChangeValues[j] - 1;

                    if (change > 0)
                        NewLoc[j] = (1 - Datum.Value.Location[j]) * BaseAcc * change;
                    else
                        NewLoc[j] = ((-1) + Datum.Value.Location[j]) * BaseAcc * -1 * change;

                    CopyPoint.Value.Location[j] += NewLoc[j];
                }
                double NewAcc = acc(Dataset, CopySet, Entries);
                if (NewAcc > BaseAcc)
                {
                    LocationChange = NewLoc;
                }
            });

            Datum.Move(LocationChange);
        }
        private static List<double[]> PossibleArrays(int ArrayLength)
        {
            List<double[]> posarrays = new List<double[]>();
            double[] nextarray = new double[ArrayLength];

            Random r = new Random();
            for (int i = 0; i < ArrayLength; i++)
            {
                double next = r.NextDouble();
                if (next > 0.5)
                    nextarray[i] = 0.01;
                else
                    nextarray[i] = -0.01;
            }

            posarrays.Add(nextarray);

            for (int i = 0; i < ArrayLength; i++)
            {
                    nextarray[i] = 0.01;
            }

            while (true)
            {
                double[] newarray = new double[ArrayLength];

                for (int i = 0; i < ArrayLength; i++)
                {

                        newarray[i] = nextarray[i];
                }

                for (int i = 0; i < ArrayLength - 1; i++)
                {
                    if (nextarray[i + 1] == 2.01)
                    {
                        if (nextarray[i] == 2.01)
                            newarray[i] = 0.01;
                        else
                            newarray[i] += 0.2;
                    }
                }

                if (nextarray[ArrayLength - 1] < 2.01)
                    newarray[ArrayLength - 1] += 0.2;
                else
                    newarray[ArrayLength - 1] = 0.01;

                posarrays.Add(newarray);

                if (newarray.Any(x => x != 2.01))
                    for (int i = 0; i < nextarray.Count(); i++)
                    {
                        nextarray[i] = newarray[i];
                    }
                else
                    break;
            }
            return posarrays;
        }
    }
}

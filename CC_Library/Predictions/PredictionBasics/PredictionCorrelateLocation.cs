using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CC_Library.Datatypes;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class PredictionCorrelateLocation
    {
        public static void Correlate
            (this Dictionary<string, Element>[] Datasets,
            List<Entry> Entries,
            Accuracy CalcAcc,
            WriteToCMDLine write)
        {
            int DataSize = Datasets.First().First().Value.datatype.Count();

            int[] CorrelatedLocation = new int[DataSize];
            for (int i = 0; i < DataSize; i++)
            {
                CorrelatedLocation[i] = i;
            }

            List<int[]> PosArrays = PossibleArrays(DataSize);
            KeyValuePair<int, double[]>[] Locations = new KeyValuePair<int, double[]>[PosArrays.Count()];
            var blankchanges = new string[] { " " };
            Parallel.For(0, PosArrays.Count(), i =>
            {
                var Clone = Datasets.CloneData();
                Clone[0].SetCorrelation(PosArrays[i]);

                double[] acc = CalcAcc(Entries, Clone);
                Locations[i] = new KeyValuePair<int, double[]>(i, acc);
            });

            var Result = Locations.FindResult();
            CorrelatedLocation = PosArrays.ElementAt(Result.Key);

            for (int i = 0; i < DataSize; i++)
            {
                write("Correlation at " + i + " : " + CorrelatedLocation[i]);
            }
            Datasets.First().UpdateCorrelation(CorrelatedLocation);
        }

        private static List<int[]> PossibleArrays(int ArrayLength)
        {
            int ComparativeArrayCount = Datatype.Dictionary.Count();

            List<int[]> posarrays = new List<int[]>();
            int[] firstarray = new int[ArrayLength];
            int[] nextarray = new int[ArrayLength];

            int ComparativeLength = ComparativeArrayCount - ArrayLength;

            if (ComparativeLength > 0)
            {
                for (int i = 0; i < firstarray.Count(); i++)
                {
                    firstarray[i] = i;
                    nextarray[i] = i;
                }
                posarrays.Add(firstarray);

                while (true)
                {
                    int[] newarray = new int[ArrayLength];
                    for (int i = 0; i < newarray.Count(); i++)
                    {
                        newarray[i] = nextarray[i];
                    }

                    if (nextarray[1] == (ComparativeLength + 1))
                        newarray[0] = nextarray[0] + 1;

                    for (int i = 1; i < ArrayLength - 1; i++)
                    {
                        if (nextarray[i + 1] == (ComparativeLength + i + 1))
                        {
                            if (nextarray[i] == (ComparativeLength + i))
                                newarray[i] = newarray[i - 1] + 1;
                            else
                                newarray[i] = nextarray[i] + 1;
                        }
                    }

                    if (nextarray[ArrayLength - 1] < (ComparativeArrayCount - 1))
                        newarray[ArrayLength - 1] = nextarray[ArrayLength - 1] + 1;
                    else
                        newarray[ArrayLength - 1] = newarray[ArrayLength - 2] + 1;

                    posarrays.Add(newarray);

                    if (newarray[0] == ComparativeLength)
                        break;
                    else
                        for (int i = 0; i < nextarray.Count(); i++)
                        {
                            nextarray[i] = newarray[i];
                        }
                }
            }
            return posarrays;
        }
    }
}

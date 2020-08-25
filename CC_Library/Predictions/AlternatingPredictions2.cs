using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CC_Library.Datatypes;

namespace CC_Library.Predictions.Masterformat
{
    public static class CC_AlternatingPredictions2
    {
        internal static void AlternatingPredictions2
            (this string filepath,
            Datatype datatype,
            InitializeData init,
            Accuracy accuracy,
            GetEntry getentry,
            WriteToCMDLine write,
            ChangedElements Changes,
            Hold hold)
        {
            Random random = new Random();
            var Entries = filepath.GetEntries(getentry, write);
            var Datasets = init(Entries);
            write("Dataset size is : " + Datasets[0].Count());
            write("Comparison set size is : " + Datasets[1].Count());

            Datasets.Correlate(Entries, accuracy, write);

            Datasets.AdjustDistance(Entries, accuracy, write);

            int count = 1;
            int Multiplier = 10;
            bool AlternatingCheck = true;

            while (true)
            {
                var Adjusted = Entries.Where(x => x.correct == false).
                    OrderBy(x => random.Next()).Take(4).ToList();

                var Changed = Changes(Adjusted);
                int AdjustedCount = 0;
                for (int i = 0; i < Changed.Count(); i++)
                    AdjustedCount += Changed[i].Count();

                write("");
                write("Run Number : " + count + ", Multiplier is : " + Multiplier);

                //Changes needs to be related to the entries fitted towards.
                var PosArrays = datatype.PossibleArrays(AdjustedCount);

                string s = Changed.First().First();
                for(int j = 0; j <Changed.Count(); j++)
                    for (int i = 1; i < Changed[j].Count(); i++)
                        s += ", " + Changed[j][i];

                var TestAccuracy = accuracy(Entries, Datasets);
                write("Modified Elements : " + s);
                write("\tBase Accuracy is " + TestAccuracy[0] + " / " + TestAccuracy[1] + " = " + TestAccuracy[0] / TestAccuracy[1]);
                write("\tBase Negative Distance is : " + TestAccuracy[2]);
                write("\tBase Positive Distance is : " + TestAccuracy[3]);
                write("");


                double Adjustment = TestAccuracy[0] / TestAccuracy[1];
                KeyValuePair<int, double[]>[] Locations = new KeyValuePair<int, double[]>[PosArrays.Count()];

                Parallel.For(0, PosArrays.Count(), i =>
                {
                    Dictionary<string, Element>[] Copy = Datasets.CloneData();

                    //Update the appropriate dictionary and data points in parallel options
                    string moved = Copy.BulkMove(PosArrays[i], Changed, (1 - Adjustment) / Multiplier);

                    double[] acc = accuracy(Entries, Copy);
                    Locations[i] = new KeyValuePair<int, double[]>(i, acc);
                });

                var Result = Locations.FindResult();
                var FinLocation = PosArrays.ElementAt(Result.Key);

                write("\tFinal Accuracy is " + Result.Value[0] + " / " + Result.Value[1] + " = " + Result.Value[0] / Result.Value[1]);
                write("\tFinal Negative Distance is : " + Result.Value[2]);
                write("\tFinal Positive Distance is : " + Result.Value[3]);

                string Mod = Datasets.BulkMove(FinLocation, Changed, (1 - Adjustment) / Multiplier);
                Datasets.BulkWrite(Changed);

                if (Mod == "Modified:")
                {
                    write("No Change");

                    if (!AlternatingCheck)
                    {
                        Multiplier += 5;
                        if (Multiplier > 50)
                        {
                            var array = PosArrays.ElementAt(random.Next(PosArrays.Count()));
                            Mod = Datasets.BulkMove(array, Changed, (1 - Adjustment) / Multiplier);
                            Datasets.BulkWrite(Changed);
                            Datasets.AdjustDistance(Entries, accuracy, write);
                            Multiplier = 20;
                        }
                    }
                    else
                        AlternatingCheck = false;
                }
                else
                {
                    write(Mod);
                    if (AlternatingCheck)
                    {
                        if (Multiplier > 2)
                            Multiplier -= 1;
                    }
                    else
                        AlternatingCheck = true;
                }
                count++;
            }
        }
    }
}
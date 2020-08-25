using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CC_Library.Datatypes;
/*
namespace CC_Library.Predictions.Masterformat
{
    public static class CC_AlternatingPredictions
    {
        internal static void AlternatingPredictions
            (this string filepath,
            Datatype datatype,
            InitializeData init,
            Accuracy accuracy,
            GetEntry getentry,
            WriteToCMDLine write,
            Hold hold)
        {
            var Entries = filepath.GetEntries(getentry, write);
            var Datasets = init(Entries);
            write("Dataset size is : " + Datasets[0].Count());
            write("Comparison set size is : " + Datasets[1].Count());

            int adjustedcount = 12;
            if (adjustedcount > Datasets[0].Count())
                adjustedcount = Datasets[0].Count();

            Datasets.Correlate(Entries, accuracy, write);
            var PosArrays = datatype.PossibleArrays(adjustedcount);
            write("Possible Arrays " + PosArrays.Count() +
                " : " + PosArrays.First().Count() + " : " + PosArrays.First().First().Count());
            
            int count = 1;
            int Multiplier = 10;
            int ChangeSet = 0;
            bool AlternatingCheck = true;

            while (true)
            {
                //Show total accuracy and sort entries by total distance.
                string[] Changes = new string[adjustedcount];

                write("");
                write("Run Number : " + count + ", Multiplier is : " + Multiplier);

                Changes = Datasets[ChangeSet].CollectChangedElements(adjustedcount);

                string s = Changes.First();
                for (int i = 1; i < Changes.Count(); i++)
                    s += ", " + Changes[i];

                var TestAccuracy = accuracy(Entries, Datasets, Changes);
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
                    string moved = Copy.BulkMove(ChangeSet, PosArrays[i], Changes, (1 - Adjustment) / Multiplier);

                    double[] acc = accuracy(Entries, Copy, Changes);
                    Locations[i] = new KeyValuePair<int, double[]>(i, acc);
                });

                var Result = Locations.FindResult();
                var FinLocation = PosArrays.ElementAt(Result.Key);

                write("\tFinal Accuracy is " + Result.Value[0] + " / " + Result.Value[1] + " = " + Result.Value[0] / Result.Value[1]);
                write("\tFinal Negative Distance is : " + Result.Value[2]);
                write("\tFinal Positive Distance is : " + Result.Value[3]);

                string Mod = Datasets.BulkMove(ChangeSet, FinLocation, Changes, (1 - Adjustment) / Multiplier);
                Datasets.BulkWrite(Changes, ChangeSet);

                if (Mod == "Modified:")
                {
                    write("No Change");
                    if (!AlternatingCheck)
                    {
                        Multiplier += 5;
                        if(Multiplier > 50)
                        {
                            Random random = new Random();
                            var array = PosArrays.ElementAt(random.Next(PosArrays.Count()));
                            Mod = Datasets.BulkMove(ChangeSet, array, Changes, (1 - Adjustment) / Multiplier);
                            Datasets.BulkWrite(Changes, ChangeSet);
                            Multiplier = 20;
                            write(Mod);
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

                ChangeSet++;
                if (ChangeSet >= Datasets.Count())
                    ChangeSet = 0;
                count++;
            }
        }
    }
}
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CC_Library.Datatypes;

namespace CC_Library.Predictions.RoomPrivacy
{
    public static class RP_PredictionData
    {
        public static void RP_RunPredictions(this string filepath, WriteToCMDLine write)
        {
            Random random = new Random();

            //Check if the dataset exists within the Solution, Load it if it does.
            Dictionary<string, Data> RPData = Datatype.Masterformat.Read(write);
            Dictionary<string, Data> DictData = Datatype.Dictionary.Read(write);

            //Note, Datapoints are calculated on the fly. 
            //It is added at this stage with the entries to the dictionary set.
            //Added with random data.
            Dictionary<string, string[]> entries = filepath.GetEntryValues(write);
            RPData.InitializeRP(entries, random, write);
            DictData.InitializeDict(entries, random, write);

            //After each accuracy test, and once when target accuracy is reached, save all datasets to xml files.
            DictData.Write();
            RPData.Write();

            int totalcount = RPData.Count() + DictData.Count();

            for (int runcount = 0; runcount < 500; runcount++)
            {
                int objectcount = 0;
                RPData.ShowAccuracy(DictData, entries, write, runcount);
                for(int i = 0; i < RPData.Count(); i++)
                {
                    Interlocked.Increment(ref objectcount);

                    RPData.ElementAt(i).ChartPrivacy(RPData, DictData, entries, write);

                    //Test for accuracy of the dataset.
                    double Accuracy = RPData.Accuracy(DictData, entries);
                    write("Scanned " + objectcount + " of " + totalcount + " objects.");
                    write("The Total Accuracy is " + Accuracy);
                    write("The number of loops completed is : " + runcount);
                }
                for(int i = 0; i < DictData.Count(); i++)
                {
                    Interlocked.Increment(ref objectcount);
                    DictData.ElementAt(i).ChartDict(DictData, RPData, entries, write);

                    //Test for accuracy of the dataset.
                    double Accuracy = RPData.Accuracy(DictData, entries);
                    write("Scanned " + objectcount + " of " + totalcount + " objects.");
                    write("The Total Accuracy is " + Accuracy);
                    write("The number of loops completed is : " + runcount);
                }

                //Save the dataset to its own xml file
                DictData.Write();
                RPData.Write();
            }
        }
    }
}
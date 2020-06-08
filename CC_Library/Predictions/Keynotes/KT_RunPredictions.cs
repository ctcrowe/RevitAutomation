using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CC_Library.Datatypes;

namespace CC_Library.Predictions.Keynotes
{
    public static class RunPredictions
    {
        public static void KT_RunPredictions(this string filepath, WriteToCMDLine write)
        {
            Random random = new Random();

            //Check if the dataset exists within the Solution, Load it if it does.
            Dictionary<string, Data> KTData = Datatype.Keynote.Read(write);
            Dictionary<string, Data> DictData = Datatype.Dictionary.Read(write);

            //Note, Datapoints are calculated on the fly. 
            //It is added at this stage with the entries to the dictionary set.
            //Added with random data.
            Dictionary<string, string[]> entries = filepath.GetEntryValues(write);
            DictData.InitializeDict(entries, random, write);
            KTData.InitializeKeynotes(DictData, entries, random, write);

            //After each accuracy test, and once when target accuracy is reached, save all datasets to xml files.
            DictData.Write();
            KTData.Write();

            int totalcount = KTData.Count() + DictData.Count();

            for (int runcount = 0; runcount < 500; runcount++)
            {
                int objectcount = 0;
                KTData.ShowAccuracy(DictData, entries, write, runcount);
                for (int i = 0; i < DictData.Count(); i++)
                {
                    objectcount++;
                    DictData.ElementAt(i).ChartData(KTData, DictData, entries, write);

                    //Test for accuracy of the dataset.
                    double Accuracy = KTData.Accuracy(DictData, entries);
                    KTData.InitializeKeynotes(DictData, entries, random, write);
                    write("Scanned " + objectcount + " of " + totalcount + " objects.");
                    write("The Total Accuracy is " + Accuracy);
                    write("The number of loops completed is : " + runcount);
                }

                //Save the dataset to its own xml file
                DictData.Write();
                KTData.Write();
            }
        }
    }
}

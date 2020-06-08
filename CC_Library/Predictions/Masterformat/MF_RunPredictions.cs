using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CC_Library.Datatypes;

namespace CC_Library.Predictions.Masterformat
{
    public static class MF_PredictionData
    {
        public static void MF_RunPredictions(this string filepath, int Loops, WriteToCMDLine write, Hold hold)
        {
            Random random = new Random();
            CalcAccuracy accuracy = new CalcAccuracy(MF_Accuracy.CreateAccuracy);

            //Check if the dataset exists within the Solution, Load it if it does.
            Dictionary<string, Data> MFData = Datatype.Masterformat.Read(write);
            Dictionary<string, Data> BaseDict = Datatype.Dictionary.Read(write);

            write("MASTERFORMAT SIZE : " + Datatype.Masterformat.Count());
            write("DICTIONARY SIZE : " + Datatype.Dictionary.Count());

            hold();

            //Note, Datapoints are calculated on the fly. 
            //It is added at this stage with the entries to the dictionary set.
            //Added with random data.
            Dictionary<string, string[]> entries = filepath.GetEntryValues(write);
            MFData.InitializeMF(entries, random, write);
            BaseDict.InitializeDict(entries, random, write);

            MFData.CorrelateArrays
                (BaseDict,
                entries,
                accuracy,
                write);

            Dictionary<string, Data> DictData = BaseDict.CloneDictionary(MFData.FirstOrDefault().Value.RefPoints, write);

            //After each accuracy test, and once when target accuracy is reached, save all datasets to xml files.
            BaseDict.Write();
            MFData.Write();

            int totalcount = MFData.Count() + DictData.Count();
            
            for (int runcount = 0; runcount < Loops; runcount++)
            {
                int objectcount = 0;
                MFData.ShowAccuracy(DictData, entries, write, runcount);
                for(int i = 0; i < MFData.Count(); i++)
                {
                    Interlocked.Increment(ref objectcount);

                    MFData.ElementAt(i).ChartElement(MFData, DictData, entries, write, accuracy);

                    //MFData.ElementAt(i).ChartDataset(MFData, DictData, entries, accuracy, write);

                    //Test for accuracy of the dataset.
                    double Accuracy = MFData.Accuracy(DictData, entries);
                    write("Scanned " + objectcount + " of " + totalcount + " objects.");
                    write("The Total Accuracy is " + Accuracy);
                    write("The number of loops completed is : " + runcount);
                }
                for(int i = 0; i < DictData.Count(); i++)
                {
                    Interlocked.Increment(ref objectcount);

                    DictData.ElementAt(i).ChartWord(MFData, DictData, entries, write, accuracy);

                    //DictData.ElementAt(i).ChartDict(MFData, DictData, entries, accuracy, write);
                    BaseDict.Update(DictData.ElementAt(i));

                    //Test for accuracy of the dataset.
                    double Accuracy = MFData.Accuracy(DictData, entries);
                    write("Scanned " + objectcount + " of " + totalcount + " objects.");
                    write("The Total Accuracy is " + Accuracy);
                    write("The number of loops completed is : " + runcount);
                }

                //Save the dataset to its own xml file
                BaseDict.Write();
                MFData.Write();
            }
        }
    }
}
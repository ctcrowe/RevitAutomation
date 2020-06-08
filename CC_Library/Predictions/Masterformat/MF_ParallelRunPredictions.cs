using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CC_Library.Datatypes;

namespace CC_Library.Predictions.Masterformat
{
    public static class MF_ParallelPredictions
    {
        public static void MF_RunPredictionsParallel(this string filepath, int Loops, WriteToCMDLine write, Hold hold)
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
            var DictArrays = DictData.PossibleArrays();
            var DataArrays = MFData.PossibleArrays();

            write("Possible Dictionary Arrays : " + DictArrays.Count());
            write("Possible Data Arrays : " + DataArrays.Count());

            for (int runcount = 0; runcount < Loops; runcount++)
            {
                MFData.ShowAccuracy(DictData, entries, write, runcount);
                for (int i = 0; i < MFData.Count(); i++)
                {
                    MFData.ElementAt(i).SetPositiveDirection(DictData, entries);
                    MFData.ElementAt(i).SetNegativeDirection(DictData, entries);
                }
                MFData.ParallelCharting(DictData, entries, DataArrays, accuracy, write);
                for (int i = 0; i < DictData.Count(); i++)
                {
                    DictData.ElementAt(i).SetPositiveDirection(MFData, entries);
                    DictData.ElementAt(i).SetNegativeDirection(MFData, entries);
                }
                DictData.ParallelCharting(MFData, entries, DictArrays, accuracy, write);
                BaseDict.Update(DictData);

                //Save the dataset to its own xml file
                BaseDict.Write();
                MFData.Write();
            }
        }

    }
}
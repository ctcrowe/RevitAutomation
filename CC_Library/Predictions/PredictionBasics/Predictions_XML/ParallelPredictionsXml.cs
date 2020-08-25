using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;

using CC_Library.Datatypes;

/*
namespace CC_Library.Predictions
{
    public static class CC_ParallelPredictionsXml
    {
        public static void ParallelPredictionsXml(this string filepath, WriteToCMDLine write, Hold hold)
        {
            int adjustedcount = 42;
            Random random = new Random();
            XmlAccuracy accuracy = new XmlAccuracy(MF_XmlAccuracy.Accuracy);
            XmlAccuracy accuracy2 = new XmlAccuracy(MF_XmlAccuracy.Accuracy2);

            var Dataset = Datatype.Masterformat.GetElements();
            var Dictset = Datatype.Dictionary.GetElements();

            Dictionary<string, string[]> Entries = filepath.GetEntryValues(write);

            Dataset.InitializeMasterformat(Entries);
            Dictset.InitializeDictionary(Entries);

            write("Masterformat has " + Dataset.Count() + " Entries.");
            write("Dictionary has " + Dictset.Count() + " Entries");

            hold();

            Dataset.CorrelateArrays(Dictset, Entries, accuracy2, write);

            var PosArrays = Dataset.PossibleArrays(adjustedcount);
            write("Possible Arrays " + PosArrays.Count() + " : " + PosArrays.First().Count() + " : " + PosArrays.First().First().Count());
            int count = 1;
            int Possible = PosArrays.Count();

            while (true)
            {
                //Calculate the total accuracy
                //Revise Accuracy to takae entries and a set of changed elements as the arguments.
                //The list may be empty!
                double Accuracy = accuracy(Dataset, Dictset, Entries);

                List<double[]> changes = new List<double[]>();
                double BaseAcc = 0;

                var ChangedElements = adjustedcount.CollectChangedElements(Dataset, Dictset);
                var ReducedEntries = ChangedElements.CollectReducedEntries(Entries);
                write("Run Number : " + count + ", Current total accuracy : " + Accuracy + ", First changed element : " + ChangedElements.First().Key);

                Parallel.For(0, PosArrays.Count(), i =>
                {
                    var CopyData = Datatype.Masterformat.GetElements();
                    var CopyDict = Datatype.Dictionary.GetElements();

                    //Update the appropriate dictionary and data points in parallel options
                    for (int j = 0; j < PosArrays[i].Count(); j++)
                    {
                        double[] values = new double[PosArrays[i][j].Count()];
                        for (int k = 0; k < values.Count(); k++)
                        {
                            values[k] = PosArrays[i][j][k] * ((1 - Accuracy) / 2);
                        }
                        ChangedElements.ElementAt(j).ElementToChange(CopyData, CopyDict).Move(values);
                    }

                    //Test accuracy to see if it is higher with only the given entries
                    double NewAcc = accuracy2(CopyData, CopyDict, ReducedEntries);
                    if (NewAcc > BaseAcc)
                    {
                        changes = PosArrays[i];
                        BaseAcc = NewAcc;
                    }
                });

                Parallel.For(0, changes.Count(), j =>
                {
                    double[] values = new double[changes[j].Count()];
                    for (int k = 0; k < changes[j].Count(); k++)
                    {
                        values[k] = changes[j][k] * ((1 - Accuracy) / 3);
                    }
                    ChangedElements.ElementAt(j).ElementToChange(Dataset, Dictset).Move(values);
                    ChangedElements.ElementAt(j).ElementToChange(Dataset, Dictset).Write();
                });

                count++;
            }
        }
    }
}*/
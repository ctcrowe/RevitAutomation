using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public delegate void WriteToCMDLine(string s);
    public static class Datasets
    {
        public static void RunPredictions(WriteToCMDLine write)
        {
            //Open the Dataset
            OpenFileDialog ofd = new OpenFileDialog()
            {
                FileName = "Select a csv file",
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Open csv file"
            };
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                var filepath = ofd.FileName;
                Random random = new Random();

                if (Enum.GetNames(typeof(Datatype)).Any(x => filepath.Contains(x)))
                {
                    Datatype datatype = (Datatype)Enum.Parse(typeof(Datatype), Enum.GetNames(typeof(Datatype)).Where(x => filepath.Contains(x)).First());
                    //Check if the dataset exists within the Solution, Load it if it does.
                    Dataset dataset = datatype.LoadDataset(write);
                    Dataset dictionary = Datatype.TextData.LoadDataset(write);

                    dataset.Reduce();
                    dictionary.Reduce();

                    //Note, Datapoints are calculated on the fly. 
                    //It is added at this stage with the entries to the dictionary set.
                    //Added with random data.
                    Dictionary<string, string> entries = filepath.GetEntryValues(write);
                    dataset.InitializeDataset(entries, random, write);
                    dictionary.InitializeDataset(entries, random, write);

                    //Primary Accuracy Test
                    double Accuracy = dataset.CalcAccuracy(dictionary, entries, write);
                    //After each accuracy test, and once when target accuracy is reached, save all datasets to xml files.
                    dictionary.WriteToXML();
                    dataset.WriteToXML();
                    
                    int totalcount = dataset.Data.Count() + dictionary.Data.Count();
                    for(int runcount = 0; runcount < 500; runcount++)
                    {
                        int objectcount = 0;
                        dataset.ShowAccuracy(dictionary, entries, write, runcount);
                        for (int i = 0; i < dataset.Data.Count(); i++)
                        {
                            objectcount++;
                            dataset.Data.ElementAt(i).ChartEntry(dataset, dictionary, entries, write);

                            //Save the dataset to its own xml file
                            dictionary.WriteToXML();
                            dataset.WriteToXML();

                            //Test for accuracy of the dataset.
                            Accuracy = dataset.CalcAccuracy(dictionary, entries, write);
                            write("Scanned " + objectcount + " of " + totalcount + " objects.");
                            write("The Total Accuracy is " + Accuracy);
                            write("The number of loops completed is : " + runcount);
                        }
                        for(int i = 0; i < dictionary.Data.Count(); i++)
                        {
                            objectcount++;
                            dictionary.Data.ElementAt(i).ChartEntry(dictionary, dataset, entries, write);

                            //Save the dataset to its own xml file
                            dictionary.WriteToXML();
                            dataset.WriteToXML();

                            //Test for accuracy of the dataset.
                            Accuracy = dataset.CalcAccuracy(dictionary, entries, write);
                            write("Scanned " + objectcount + " of " + totalcount + " objects.");
                            write("The Total Accuracy is " + Accuracy);
                            write("The number of loops completed is : " + runcount);
                        }
                    }
                }
            }
        }
    }
}
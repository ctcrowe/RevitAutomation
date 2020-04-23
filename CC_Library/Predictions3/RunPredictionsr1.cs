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

                    //Primary Accuracy Test
                    double Accuracy = dataset.CalcAccuracy(dictionary, entries, random, write);
                    //After each accuracy test, and once when target accuracy is reached, save all datasets to xml files.
                    dictionary.WriteToXML();
                    dataset.WriteToXML();

                    while (Accuracy < 1)
                    {
                        for (int i = 0; i < dataset.Data.Count(); i++)
                        {
                            dataset.Data.ElementAt(i).ChartEntry(dataset, dictionary, entries, random, write);
                        }
                        for(int i = 0; i < dictionary.Data.Count(); i++)
                        {
                            dictionary.Data.ElementAt(i).ChartEntry(dictionary, dataset, entries, random, write);
                        }

                        //Test for accuracy of the dataset.
                        Accuracy = dataset.CalcAccuracy(dictionary, entries, random, write);
                        write("The Total Accuracy is " + Accuracy);

                        dataset.Reduce();
                        dictionary.Reduce();

                        //Save the dataset to its own xml file
                        dictionary.WriteToXML();
                        dataset.WriteToXML();
                    }
                }
            }
        }
    }
}
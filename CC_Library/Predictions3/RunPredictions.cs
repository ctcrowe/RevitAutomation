using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class Datasets
    {
        public static void RunPredictions()
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
                if (Enum.GetNames(typeof(Datatype)).Any(x => filepath.Contains(x)))
                {
                    Datatype dt = (Datatype)Enum.Parse(typeof(Datatype), Enum.GetNames(typeof(Datatype)).Where(x => filepath.Contains(x)).First());
                    //Check if the dataset exists within the Solution, Load it if it does.


                    //Randomize starting values for each new element from the dataset.


                    //Primary Accuracy Test
                    double Accuracy = 0;

                    while (Accuracy < 90)
                    {
                        //Modify the numbers in the dataset.
                        //Test for accuracy of the dataset.
                    }
                    //Save the dataset to its own csv file
                }
            }
        }
    }
}

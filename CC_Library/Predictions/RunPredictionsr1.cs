using System;
using System.Linq;
using System.Windows.Forms;

using CC_Library.Datatypes;
using CC_Library.Predictions.RoomPrivacy;
using CC_Library.Predictions.Masterformat;

namespace CC_Library.Predictions
{
    public static class Datasets
    {
        public static void RunPredictions(int Loops, WriteToCMDLine write, Hold hold)
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

                    switch(datatype)
                    {
                        default:
                            break;
                        case Datatype.Masterformat:
                            //filepath.MF_RunPredictions(Loops, write, hold);
                            filepath.MF_RunPredictionsParallel(Loops, write, hold);
                            break;
                        case Datatype.RoomPrivacy:
                            filepath.RP_RunPredictions(write);
                            break;
                    }
                }
            }
        }
    }
}
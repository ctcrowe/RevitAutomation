using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class Datasets
    {
        public static void RunPredictions(WriteToCMDLine write)
        {
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
                    Sample s = filepath.ReadFromBinaryFile<Sample>();
                    Datatype datatype = (Datatype)Enum.Parse(typeof(Datatype), s.Datatype);
                    write("Network Type : " + datatype.ToString());
                    s.PropogateSingle(write);
                }
            }
        }
        public static void FormatOutput()
        {
            
        }
    }
}

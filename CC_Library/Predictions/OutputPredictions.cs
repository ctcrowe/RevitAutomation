using System;
using System.Linq;
using System.Windows.Forms;
using CC_Library.Datatypes;
using System.IO;

namespace CC_Library.Predictions
{
    public static class PredOutput
    {
        public static void OutputPredictions(WriteToCMDLine write)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                FileName = "Select a csv file",
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Open csv file"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var filepath = ofd.FileName;
                Random random = new Random();

                if (Enum.GetNames(typeof(Datatype)).Any(x => filepath.Contains(x)))
                {
                    Datatype datatype = (Datatype)Enum.Parse(typeof(Datatype), Enum.GetNames(typeof(Datatype)).Where(x => filepath.Contains(x)).First());
                    write("Network Type : " + datatype.ToString());
                    var Lines = File.ReadAllLines(filepath);

                    SaveFileDialog sfd = new SaveFileDialog()
                    {
                        FileName = "Create a txt file",
                        Filter = "TXT files (*.txt)|*.txt",
                        Title = "Create a txt file"
                    };
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        var fp = sfd.FileName;

                        switch (datatype)
                        {
                            default:
                            case Datatype.Masterformat:
                                string[] r = MasterformatNetwork.PredictAll(Lines, write);
                                File.WriteAllLines(fp, r);
                                break;
                        }
                    }
                }
            }
        }
    }
}
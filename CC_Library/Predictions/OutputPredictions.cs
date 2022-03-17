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
                FileName = "Select a bin file",
                Filter = "CSV files (*.bin)|*.bin",
                Title = "Open bin file"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var filepath = ofd.FileName;
                write("Network Type : " + datatype.ToString());
                var Lines = File.ReadAllLines(filepath);
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

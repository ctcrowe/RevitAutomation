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
            while(true)
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    FileName = "Select a binary file",
                    Filter = "BIN files (*.bin)|*.bin",
                    Title = "Open bin file"
                };
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    var filepath = ofd.FileName;
                    Random random = new Random();
                    try
                    {
                        Sample s = filepath.ReadFromBinaryfile<Sample>();
                        Datatype datatype = (Datatype)Enum.Parse(typeof(Datatype), s.Datatype);
                        write("Network Type : " + datatype.ToString());
                        s.PropogateSingle(write);
                    }
                    catch (Exception e) { e.OutputError(); }
                }
            }
        }
    }
}

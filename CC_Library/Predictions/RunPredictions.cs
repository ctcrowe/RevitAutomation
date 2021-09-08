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
                FileName = "Select a binary file",
                Filter = "BIN files (*.bin)|*.bin",
                Title = "Open bin file"
            };
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                var filepath = ofd.FileName;
                var dir = Path.GetDirectoryName(filepath);
                var Files = Directory.GetFiles(dir);
                Random random = new Random();
                while(true)
                {
                    string f = Files[random.Next(Files.Count())];
                    try
                    {
                        Sample s = f.ReadFromBinaryfile<Sample>();
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

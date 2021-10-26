using System;
using System.Linq;
using System.IO;
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
                        Sample s = f.ReadFromBinaryFile<Sample>();
                        string datatype = s.Datatype;
                        s.PropogateSingle(write);
                    }
                    catch (Exception e) { e.OutputError(); }
                }
            }
        }
    }
}

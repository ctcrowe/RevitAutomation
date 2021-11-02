using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    public static class ReadSample
    {
        public static void Read(WriteToCMDLine write)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                FileName = "Select a binary file",
                Filter = "BIN files (*.bin)|*.bin",
                Title = "Open bin file"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {

                int runs = 0;
                var filepath = ofd.FileName;
                var dir = Path.GetDirectoryName(filepath);
                var Files = Directory.GetFiles(dir);
                foreach(string f in Files)
                {
                    try
                    {
                        runs++;
                        write("");
                        Sample s = f.ReadFromBinaryFile<Sample>();
                        write("File Number : " + runs);
                        write("GUID : " + s.GUID);
                        write("Input : " + s.TextInput);
                        write("Output : " + s.DesiredOutput.ToList().IndexOf(s.DesiredOutput.Max()));
                        write("");
                        System.Threading.Thread.Sleep(1000);
                    }
                    catch (Exception e) { e.OutputError(); }
                }

            }
        }
    }
}

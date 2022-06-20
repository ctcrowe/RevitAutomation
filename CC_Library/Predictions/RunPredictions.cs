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
                FileName = "Select a text file",
                Title = "Open txt file"
            };
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                int runs = 0;
                double er = 0;
                double acc = 0;
                Random r = new Random();

                var filepath = ofd.FileName;

                Dictionary<int, List<string>> values = new Dictionary<int, List<string>>();

                for(int i = 0; i < 5; i++)
                {
                    try
                    {
                        var files = Directory.GetFiles(Directory.GetDirectoryRoot(filepath)).ToList().OrderBy(x => r.NextDouble()).Take(16);
                        List<string> l = new List<string>();

                        foreach (var f in files)
                        {
                            l.Add(File.ReadAllLines(filepath)[0]);
                        }
                        var lines = l.ToArray();
                        //var error = MasterformatNetwork.Propogate(lines, write, true);
                        //var error = ObjStyleNetwork.Propogate(lines, typeof(ObjectStyles_Doors), write, true);
                        var error = ProjectionLineWeightNetwork.Propogate(lines, write, true);

                        if (error[0] > 0)
                        {
                            runs++;
                            er += error[0];
                            acc += error[1];

                            write("Total Runs : " + runs);
                            write("Total Error : " + er / runs);
                            write("Total Accuracy : " + acc / runs);
                            write("");
                        }
                        else
                        {
                            write("");
                            write("error was 0");
                            write("");
                        }
                    }
                    catch (Exception e) { e.OutputError(); }
                    System.Threading.Thread.Sleep(500);

                }
            }
        }
    }
}

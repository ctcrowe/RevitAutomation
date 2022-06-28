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
        public static double[] RunPredictions(WriteToCMDLine write, int NumberOfCycles = 10000)
        {
            double[] error = new double[2];
            int runs = 0;
            OpenFileDialog ofd = new OpenFileDialog()
            {
                FileName = "Select a text file",
                Title = "Open txt file"
            };
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                double er = 0;
                double acc = 0;
                Random r = new Random();

                var filepath = ofd.FileName;

                Dictionary<int, List<string>> values = new Dictionary<int, List<string>>();

                for(int i = 0; i < NumberOfCycles; i++)
                {
                    try
                    {
                        var files = Directory.GetParent(filepath).GetFiles();
                        Console.WriteLine("FileCount : " + files.Count());
                        var fileset = files.ToList().OrderBy(x => r.NextDouble()).Take(16);
                        List<string> l = new List<string>();

                        foreach (var f in fileset)
                        {
                            l.Add(File.ReadAllLines(f.FullName)[0]);
                        }
                        var lines = l.ToArray();
                        switch(filepath.Split('\\').Last().Split('_').First())
                        {
                            default:
                            case "LineWeightNetwork":
                            case "ProjectionLineWeightNetwork":
                                Console.WriteLine("Training Projection Line Weights");
                                try
                                {
                                    error = LineWeightNetwork.Propogate(
                                        lines,
                                        write,
                                        Transformers.ProjectionLineWeightTransformer(write),
                                        Transformers.ProjectionLineWeightAlpha1(write),
                                        Transformers.ViewNameAlpha(write));
                                }
                                catch (Exception e) { e.OutputError(); }
                                break;
                            case "MasterformatNetwork":
                                Console.WriteLine("Training Masterformat");
                                error = MasterformatNetwork.Propogate(lines, write, true);
                                break;
                        }
                        //var error = MasterformatNetwork.Propogate(lines, write, true);
                        //var error = ObjStyleNetwork.Propogate(lines, typeof(ObjectStyles_Doors), write, true);

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
            error[0] /= runs;
            error[1] /= runs;
            return error;
        }
    }
}

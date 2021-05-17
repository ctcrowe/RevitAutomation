using System;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using CC_Library.Datatypes;

namespace DataAnalysis
{
    internal static class SortClass
    {
        public static void Sort(this Datatype dt)
        {
            if (dt == Datatype.Masterformat)
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
                    var lines = File.ReadAllLines(filepath).ToList();
                    List<List<string>> LineSets = new List<List<string>>();
                    for(int i = 0; i < 40; i++)
                    {
                        List<string> l = new List<string>();
                        for(int j = 0; j < lines.Count(); j++)
                        {
                            int k = int.Parse(lines[j].Split(',').Last());
                            if (k == i)
                                l.Add(lines[j]);
                        }
                        if(l.Any())
                            LineSets.Add(l);
                    }

                    int Max = 0;
                    for (int i = 0; i < LineSets.Count(); i++)
                    {
                        int j = LineSets[i].Count();
                        if (j > Max)
                            Max = j;
                    }
                    Program.Write("Max is : " + Max);
                    Program.Write("LineSet Count is : " + LineSets.Count());
                    Random r = new Random();
                    for (int i = 0; i < LineSets.Count(); i++)
                    {
                        for(int j = LineSets[i].Count(); j < Max; j++)
                        {
                            lines.Add(LineSets[i].OrderBy(x => r.Next()).First());
                        }
                    }

                    var NewLines = lines.OrderBy(x => int.Parse(x.Split(',').Last()));
                    File.WriteAllLines(filepath, NewLines);
                }
            }
        }
    }
}

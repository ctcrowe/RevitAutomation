using System;
using System.Linq;
using System.Windows.Forms;
using CC_Library.Datatypes;
using System.IO;

namespace CC_Library.Predictions
{
    public static class PredRevise
    {
        public static void RevisePredictions(WriteToCMDLine write)
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

                if (Enum.GetNames(typeof(Datatype)).Any(x => filepath.Contains(x)))
                {
                    Datatype datatype = (Datatype)Enum.Parse(typeof(Datatype), Enum.GetNames(typeof(Datatype)).Where(x => filepath.Contains(x)).First());
                    write("Network Type : " + datatype.ToString());

                    SaveFileDialog sfd = new SaveFileDialog()
                    {
                        FileName = "Create a csv file",
                        Filter = "CSV files (*.csv)|*.csv",
                        Title = "Create a csv file"
                    };
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        var fp = sfd.FileName;
                        if (datatype == Datatype.Dictionary)
                        {
                            string[] lines = Enum.GetNames(typeof(Dict));
                            for(int i = 0; i < lines.Count(); i++)
                            {
                                lines[i] = datatype.ToString() + "," + lines[i] + "," + i;
                            }
                            File.WriteAllLines(fp, lines);
                        }

                        else
                        {
                            var Lines = File.ReadAllLines(filepath);
                            for (int i = 0; i < Lines.Count(); i++)
                            {
                                Lines[i] = datatype.ToString() + "," + Lines[i];
                            }
                            File.WriteAllLines(fp, Lines);
                        }
                    }
                }
            }
        }
    }
}
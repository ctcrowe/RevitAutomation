using System;
using System.IO;

namespace CC_Library.Datatypes
{
    public static class CreateCSVInputPoint
    {
        public static void CreateInputData(this Datatype dt, string name, string value)
        {
            string Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string Datafile = Directory + "\\" + dt.ToString() + "_CollectedData.csv";
            string Datapoint = name + "," + value;
            if (File.Exists(Datafile))
            {
                string[] lines = File.ReadAllLines(Datafile);
                string[] newlines = new string[lines.Length + 1];
                for (int i = 0; i < lines.Length; i++)
                {
                    newlines[i] = lines[i];
                }
                newlines[newlines.Length - 1] = Datapoint;
                File.WriteAllLines(Datafile, newlines);
            }
            if (!File.Exists(Datafile))
            {
                string[] lines = new string[1] { Datapoint };
                File.WriteAllLines(Datafile, lines);
            }
        }
    }
}
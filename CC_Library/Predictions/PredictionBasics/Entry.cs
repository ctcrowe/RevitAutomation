using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace CC_Library.Predictions
{
    [Serializable]
    public class Sample
    {
        public string Datatype { get; }
        public string TextInput { get; set; }
        public string SecondaryText { get; set; }
        public string TertiaryText { get; set; }
        public double[] TextOutput { get; set; }
        public double[] SecondaryTextOutput { get; set; }
        public double[] TertiaryTextOutput { get; set; }
        public double[] ValInput { get; set; }
        public double[] ImgInput { get; set; }
        public double[,] MktVals { get; set; }
        public double[] MktOutput { get; set; }
        public double[] DesiredOutput { get; set; }
        public string GUID { get; }
        public Sample(Datatype dt)
        {
            this.Datatype = dt.ToString();
            this.GUID = Guid.NewGuid().ToString();
            this.TextOutput = new double[1]{0};
            this.SecondaryTextOutput = new double[1]{0};
            this.TertiaryTextOutput = new double[1]{0};
            this.TextInput = "";
            this.SecondaryText = "";
            this.TertiaryText = "";
            this.ValInput = new double[1]{0};
            this.ImgInput = new double[1]{0};
            this.MktVals = new double[1,1];
            this.DesiredOutput = new double[1]{0};
            this.MktOutput = new double[1];
        }
    }
    public static class ReadWriteSamples
    {
        private const string fname = "NetworkSamples";
        private static string folder = fname.GetMyDocs();
        public static Dictionary<string, int> ReadSamples(this string s, int Count = 16)
        {
            Dictionary<string, int> samples = new Dictionary<string, int>();
            if(File.Exists(s))
            {
                Random r = new Random();
                var lines = File.ReadAllLines(s);
                for(int i = 0; i < Count; i++)
                {
                    int lineno = r.Next(lines.Length);
                    var line = lines[lineno];
                    var str = line.Split(',').First();
                    if (int.TryParse(line.Split(',').Last(), out int numb))
                    {
                        if (!samples.ContainsKey(str))
                            samples.Add(str, numb - 1);
                    }
                    else
                        Console.WriteLine("Failed at Line : " + lineno + " : " + line);
                }
            }
            return samples;
        }
        public static Dictionary<string, int> ReadSamples(this string[] s, int Count = 16)
        {
            Dictionary<string, int> samples = new Dictionary<string, int>();
            Random r = new Random();
            for (int i = 0; i < Count; i++)
            {
                int lineno = r.Next(s.Count());
                var line = s[lineno];
                var str = line.Split(',').First();
                if (int.TryParse(line.Split(',').Last(), out int numb))
                {
                    if (!samples.ContainsKey(str))
                        samples.Add(str, numb - 1);
                }
                else
                    Console.WriteLine("Failed at Line : " + lineno + " : " + line);
            }
            return samples;
        }
        public static string[] ReadSamples(this Sample s, WriteToCMDLine write)
        {
            List<string> lines = new List<string>();
            if(Directory.Exists(folder))
            {
                string subfolder = folder + "\\" + s.Datatype;
                if(Directory.Exists(subfolder))
                {
                    string[] Files = Directory.GetFiles(subfolder);
                    if(Files.Any())
                    {
                        for(int i = 0; i < Files.Count(); i++)
                        {
                            var samp = Files[i].ReadFromBinaryFile<Sample>();
                            lines.Add(samp.TextInput + "," + samp.DesiredOutput.ToList().IndexOf(samp.DesiredOutput.Max()));
                        }
                    }
                }
            }
            lines = lines.OrderBy(x => int.Parse(x.Split(',').Last())).ToList();
            File.WriteAllLines(folder + "\\" + s.Datatype + "_Samples.txt", lines);
            return lines.ToArray();
        }
        public static Sample[] ReadSamples(this Sample s, int Count = 16)
        {
            if (Directory.Exists(folder))
            {
                string subfolder = folder + "\\" + s.Datatype;
                if(Directory.Exists(subfolder))
                {
                    string[] Files = Directory.GetFiles(subfolder);
                    if(Files.Any())
                    {
                        Random r = new Random();
                        Sample[] output = new Sample[(Count > (Files.Count() + 1))? (Files.Count() + 1) : Count];
                        output[0] = s;
                        for(int i = 1; i < output.Count(); i++)
                        {
                            Sample sample = Files[r.Next(Files.Count())].ReadFromBinaryFile<Sample>();
                            if (sample.Datatype == s.Datatype)
                                output[i] = sample;
                            else
                                output[i] = s;
                        }
                        return output;
                    }
                }
            }
            return new Sample[1]{ s };
        }
        public static Sample[] ReadSamples(this Datatype dt, int Count = 16)
        {
            if (Directory.Exists(folder))
            {
                string subfolder = folder + "\\" + dt;
                if(Directory.Exists(subfolder))
                {
                    string[] Files = Directory.GetFiles(subfolder);
                    if(Files.Any())
                    {
                        Random r = new Random();
                        Sample[] output = new Sample[(Count > (Files.Count() + 1))? (Files.Count() + 1) : Count];
                        for(int i = 1; i < output.Count(); i++)
                        {
                            output[i] = Files[r.Next(Files.Count())].ReadFromBinaryFile<Sample>();
                        }
                        return output;
                    }
                }
            }
            return new Sample[1]{ new Sample(dt) };
        }
        public static void Save(this Sample s)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string subfolder = folder + "\\" + s.Datatype;
            if (!Directory.Exists(subfolder))
                Directory.CreateDirectory(subfolder);
            string FileName = subfolder + "\\" + s.GUID + ".bin";
            FileName.WriteToBinaryFile(s, true);
        }
    }
}

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
        public double[] TextOutput { get; set; }
        public double[] ValInput { get; set; }
        public double[] ImgInput { get; set; }
        public double[] DesiredOutput { get; set; }
        public string GUID { get; }
        public Sample(Datatype dt)
        {
            this.Datatype = dt.ToString();
            this.GUID = Guid.NewGuid().ToString();
            this.TextOutput = new double[1]{0};
            this.TextInput = "";
            this.ValInput = new double[1]{0};
            this.ImgInput = new double[1]{0};
            this.DesiredOutput = new double[1]{0};
        }
    }
    public static class ReadWriteSamples
    {
        private const string fname = "NetworkSamples";
        private static string folder = fname.GetMyDocs();
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

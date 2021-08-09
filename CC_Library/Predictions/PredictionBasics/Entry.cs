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
        private const string fname = "NetworkSamples;
        public static Sample[] ReadSamples(this Sample s, int Count = 16)
        {
            string folder = fname.GetMyDocs();
            if (Directory.Exists(folder))
            {
                string subfolder = folder + "\\" + s.Datatype;
                if(Directory.Exists(subfolder))
                {
                    string[] Files = Directory.GetFiles(subfolder);
                    if(Files.Any())
                    {
                        Random r = new Random();
                        Sample[] output = new Sample[(Count > Files.Count())? Files.Count() : Count];
                        output[0] = s;
                        for(int i = 1; i < output.Count(); i++)
                        {
                            Sample sample = ReadFromBinaryFile<Sample>(Files[r.Next(Files.Count())]);
                            if (sample.Datatype == s.Datatype)
                                output[i] = sample;
                            else
                                output[i] = s;
                        }
                        return output;
                    }
                }
            }
            return new Sample[1]{ sample };
        }
        public static void Save(this Sample s)
        {
            string folder = fname.GetMyDocs();
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string subfolder = folder + "\\" + s.Datatype;
            if (!Directory.Exists(subfolder))
                Directory.CreateDirectory(subfolder);
            string FileName = subfolder + "\\" + s.GUID + ".bin";
            WriteToBinaryFile(FileName, s, true);
        }
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Create(filePath))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }
        public static T ReadFromBinaryFile<T>(this string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
    }
}

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
        public string TextInput;
        public double[] TextOutput { get; set; }
        public double[] ValInput;
        public double[] ImgInput;
        public double[] DesiredOutput;
        public string GUID;
        public Sample(Datatype dt)
        {
            this.Datatype = dt.ToString();
            this.GUID = Guid.NewGuid().ToString();
        }
        public Sample(Datatype dt, string s = null, double[] other = null, double[] img = null)
        {
            this.Datatype = dt.ToString();
            this.GUID = Guid.NewGuid().ToString();
            if(s != null) { this.TextInput = s; }
            if(other != null) { this.ValInput = other; }
            if (img != null) { this.ImgInput = img; }

        }
    }
    public static class ReadWriteSamples
    {
        public static Sample[] ReadSamples(this Datatype dt, int Count = 16)
        {
            string folder = "NetworkSamples".GetMyDocs();
            if (Directory.Exists(folder))
            {
                string subfolder = folder + "\\" + dt.ToString();
                if(Directory.Exists(subfolder))
                {
                    string[] Files = Directory.GetFiles(folder);
                    if(Files.Any())
                    {
                        Random r = new Random();
                        Sample[] output = new Sample[(Count > Files.Count())? Files.Count() : Count];
                        for(int i = 0; i < output.Count(); i++)
                        {
                            output[i] = ReadFromBinaryFile<Sample>(Files[r.Next(Files.Count())]);
                        }
                        return output;
                    }
                }
            }
            return new Sample[1]{ new Sample(dt) };
        }
        public static void Save(this Sample s)
        {
            string folder = "NetworkSamples".GetMyDocs();
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string subfolder = folder + "\\" + s.Datatype;
            if (!Directory.Exists(subfolder))
                Directory.CreateDirectory(subfolder);
            string FileName = subfolder + "\\" + s.GUID + ".bin";
            WriteToBinaryFile(FileName, s, true);
        }
        private static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
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

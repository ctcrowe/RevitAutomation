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
        public string Datatype;
        public string TextInput;
        public double[] ValInput;
        public double[] ImgInput;
        public double[] DesiredOutput;
        public string GUID;
        public Entry(Datatype dt)
        {
            this.Datatype = dt.ToString();
            this.GUID = Guid.NewGuid().ToString();
        }
    }
    public static class ReadWriteSamples
    {
        public static Sample[] ReadSamples(this Datatype dt, int Count = 16)
        {
            string folder = "NetworkSamples".GetMyDocs();
            if (Directory.Exists(Folder))
            {
                string subfolder = folder + "\\" + dt.ToString();
                if(Directory.Exists(subfolder))
                {
                    Random r = new Random();
                    
                    string[] Files = Directory.GetFiles(Folder);
                    if(Files.Any())
                    if(!Files.Any())
                    {
                        Sample[] output = new Sample[(Count > Files.Count() * Files.Count()) + (!Count > Files.Count() * Count)];
                        for(int i = 0; i < output.Count(); i++)
                        {
                            output[i] = ReadFromBinaryFile<Sample>(Files[r.NextInt(Files.Count())]);
                        }
                        return output;
                    }
                }
            }
            return new Sample[1]{ new Sample(dt) };
        }
    }
}

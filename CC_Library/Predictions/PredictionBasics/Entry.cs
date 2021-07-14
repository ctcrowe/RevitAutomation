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
                string subfolder = Directory.
                string[] Files = Directory.GetFiles(Folder);
                if (Files.Any(x => x.Contains(fn)))
                {
                    var doc = Files.Where(x => x.Contains(fn)).First();
                    write("Loaded from MyDocs");
                    return ReadFromBinaryFile<NeuralNetwork>(doc);
                }
            }
            Sample[] output = new Sample[Count];
            string Folder
            return output;
        }
    }
}

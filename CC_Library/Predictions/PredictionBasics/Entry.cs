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
            Sample[] output = new Sample[Count];
            return output;
        }
    }
}

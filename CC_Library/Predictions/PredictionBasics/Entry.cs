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
    public class Entry
    {
        public string Datatype;
        public string TextInput;
        public double[] ValInput;
        public double[] ImgInput;
        public double[] DesiredOutput;
        public Entry(Datatype dt)
        {
            this.Datatype = dt.ToString();
        }
    }
}

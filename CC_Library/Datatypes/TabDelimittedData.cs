using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CC_Library.Datatypes
{
    public class TabDelimittedData
    {
        private Datatype datatype;
        private string Value;
        private string Label;
        private string ID;

        internal TabDelimittedData(Datatype dt, string value, string label, string id)
        {
            this.datatype = dt;
            this.Value = value;
            this.Label = label;
            this.ID = id;
        }

        internal TabDelimittedData()
        {
            this.datatype = Datatype.Null;
            this.Value = null;
            this.Label = null;
            this.ID = null;
        }

        public void WriteData(string FileName)
        {
            string[] lines = new string[4]
            {
                datatype.ToString(),
                Label,
                ID,
                Value
            };
            File.WriteAllLines(FileName, lines);
        }
    }
}
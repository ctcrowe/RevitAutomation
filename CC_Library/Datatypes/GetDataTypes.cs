using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Datatypes
{
    public static class GetDatatypes
    {
        public static TabDelimittedData GetData(this string FileName)
        {
            string[] lines = File.ReadAllLines(FileName);
            if (lines.Count() >= 4)
            {
                Datatype dt = (Datatype)Enum.Parse(typeof(Datatype), lines[0]);
                if (Enum.IsDefined(typeof(Datatype), dt))
                {
                    return new TabDelimittedData(
                    dt,
                    lines[3],
                    lines[1],
                    lines[2]);
                }
            }
            return new TabDelimittedData();
        }
        public static TabDelimittedData CreateData(this string datatype, string name, string id, string value)
        {
            Datatype dt = (Datatype)Enum.Parse(typeof(Datatype), datatype);
            if (Enum.IsDefined(typeof(Datatype), dt))
            {
                return new TabDelimittedData(dt, value, name, id);
            }
            return new TabDelimittedData();
        }
    }
}

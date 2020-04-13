using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class SaveDataFile
    {
        public static void Save(this Datatype dt, List<DataPt> data, WriteToCMDLine wo)
        {
            XDocument doc = new XDocument(new XElement(dt.ToString())) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
            string file = dt.ToString().GetMyDocs(wo) + ".xml";

            foreach (DataPt d in data)
            {
                XElement ele = new XElement(dt.ToString());
                ele.Add(new XAttribute("NAME", d.Phrase));
                for (int i = 0; i < 20; i++)
                {
                    ele.Add(new XAttribute(Enum.GetName(typeof(GetNumber), i), d.GetValue(i)));
                }
                doc.Root.Add(ele);
            }
            doc.Save(file);
        }
    }
    public enum GetNumber
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Eleven = 11,
        Twelve = 12,
        Thirteen = 13,
        Fourteen = 14,
        Fifteen = 15,
        Sixteen = 16,
        Seventeen = 17,
        Eighteen = 18,
        Nineteen = 19
    }
}
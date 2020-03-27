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
        public static void Save(this Datatype dt, List<Data> data, CMDGetMyDocs.WriteOutput wo)
        {
            XDocument doc = new XDocument(new XElement(dt.ToString())) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
            string file = dt.ToString().GetMyDocs(wo) + ".xml";
            string Title = "";
            string Number = "";

            foreach (Data d in data)
            {
                XElement ele = new XElement(Title);
                for (int i = 0; i < 20; i++)
                {
                    ele.Add(new XAttribute(Number, Title));
                }
                doc.Root.Add(ele);
            }
            doc.Save(file);
        }
    }
}

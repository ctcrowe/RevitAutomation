using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CC_Library.Predictions
{
    internal static class SaveDataFile
    {
        public static void Save(this DataFile df, List<Data> data, CMDGetMyDocs.WriteOutput wo)
        {
            XDocument doc = new XDocument(new XElement(df.ToString())) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
            string file = df.ToString().GetMyDocs(wo) + ".xml";
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

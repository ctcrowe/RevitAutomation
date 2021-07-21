using System;
using System.Linq;
using System.Collections.Generic;
using CC_Library.Datatypes;
using System.Xml.Linq;
using System.Reflection;
using System.Xml;
using System.IO;

namespace CC_Library.Predictions
{
    internal static class ReadWriteXML
    {
        public static XDocument First(this Datatype datatype)
        {
            string Folder = datatype.ToString().GetMyDocs();
            if (Directory.Exists(Folder))
            {
                string[] Files = Directory.GetFiles(Folder);
                    var xdoc = XDocument.Load(Files.First());
                    return xdoc;
            }
            XDocument doc = datatype.CreateXDoc("Null");
            return doc;
        }
        public static IEnumerable<XDocument> GetElements(this Datatype datatype, List<string> Elements)
        {
            List<XDocument> XDocs = new List<XDocument>();
            foreach(string ele in Elements)
            {
                XDocs.Add(datatype.GetXMLElement(ele));
            }
            return XDocs;
        }
        public static XDocument GetXMLElement(this Datatype datatype, string ElementName)
        {
            string Folder = datatype.ToString().GetMyDocs();
            if (Directory.Exists(Folder))
            {
                string[] Files = Directory.GetFiles(Folder);
                if(Files.Any(x => x.Split('\\').Last().Split('.').First() == datatype.ToString() + "_" + ElementName))
                {
                    var xdoc = XDocument.Load(Files.Where(x => x.Split('\\').Last().Split('.').First() == datatype.ToString() + "_" + ElementName).First());
                    return xdoc;
                }
            }
            else
            {
                var assembly = typeof(ReadWriteXML).GetTypeInfo().Assembly;
                if(assembly.GetManifestResourceNames().Any(x => x.Contains(datatype.ToString() + "_" + ElementName)))
                {
                    string name = assembly.GetManifestResourceNames().Where(z => z.Contains(datatype.ToString() + "_" + ElementName)).First();
                    using (Stream stream = assembly.GetManifestResourceStream(name))
                    {
                        var xdoc = XDocument.Load(stream);
                        return xdoc;
                    }
                }
            }
            XDocument doc = datatype.CreateXDoc(ElementName);
            doc.Write();
            return doc;
        }
        public static XDocument CreateXDoc(this Datatype type, string ElementName)
        {
            Random r = new Random();
            XDocument doc = new XDocument(new XElement(type.ToString() + "_" + ElementName))
            {
                Declaration = new XDeclaration("1.0", "utf-8", "yes")
            };
            doc.Root.Add(new XAttribute("Label", ElementName));
            doc.Root.Add(new XAttribute("Datatype", type.ToString()));
            double[] values = new double[type.Count()];

            for (int i = 0; i < type.Count(); i++)
            {
                XElement dp = new XElement("Data");
                dp.Add(new XAttribute("Location", i));
                double swap = r.NextDouble();
                double v = r.NextDouble();
                if (swap > 0.50)
                    dp.Add(new XAttribute("Value", v));
                else
                    dp.Add(new XAttribute("Value", -v));
                dp.Add(new XAttribute("Referencing", i));

                doc.Root.Add(dp);
            }

            return doc;
        }
        public static void Write(this XDocument doc)
        {
            string Dir = doc.Root.Attribute("Datatype").Value.ToString();
            string direct = Dir.GetMyDocs();
            if (!Directory.Exists(direct))
                Directory.CreateDirectory(direct);
            string filename = direct + "\\" + doc.Root.Name.ToString() + ".xml";
            doc.Save(filename);
        }
    }
}

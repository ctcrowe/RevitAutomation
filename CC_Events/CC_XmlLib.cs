using System.Linq;
using System.IO;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace CC_Plugin
{
    public class XmlLib
    {
        private static string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string file = dir + "\\CC_Data\\XMLDatabase.xml";
        private const string root = "DB";
        private const string Elements = "DOCS";
        public const string type = "TYPE";
        public const string id = "ID";
        public XElement places { get; }
        private XDocument doc { get; }
        private XElement eles { get; }

        public XmlLib()
        {
            if (File.Exists(file))
            {
                doc = XDocument.Load(file);
                if (doc.Root.Element(Elements) == null)
                    doc.Root.Add(new XElement(Elements));
            }
            else
            {
                doc = new XDocument(new XElement(root)) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
                doc.Root.Add(new XElement(Elements));
            }
            eles = doc.Root.Element(Elements);
        }
        public XElement GetElement(string Id, string TYPE)
        {
            if (!eles.Elements().Any(x => x.Attribute(id).Value == Id))
            {
                XElement ele = new XElement(Elements);
                ele.Add(new XAttribute(id, Id));
                ele.Add(new XAttribute(type, TYPE));
                eles.Add(ele);
                return ele;
            }
            return eles.Elements().Where(x => x.Attribute(id).Value == Id).First();
        }
        public IEnumerable<XElement> GetElements(string TYPE)
        {
            return eles.Elements().Where(x => x.Attribute(type).Value == TYPE);
        }
        public void Save()
        {
            doc.Save(file);
        }
    }
}
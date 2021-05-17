using System.Linq;
using System.IO;
using System.Xml.Linq;
using System;

namespace CC_Plugin
{
    public static class XmlLib
    {
        private static string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string file = dir + "\\CC_Data\\XMLDatabase.xml";
        private const string root = "DB";
        private const string Elements = "DOCS";
        public const string id = "ID";

        public static XElement GetElement(string Id)
        {
            XDocument doc = CreateFile();
            if (!doc.Root.Elements().Any(x => x.Attribute(id).Value == Id))
            {
                XElement ele = new XElement(Elements);
                ele.Add(new XAttribute(id, Id));
                doc.Root.Add(ele);
                return ele;
            }
            return doc.Root.Elements().Where(x => x.Attribute(id).Value == Id).First();
        }
        public static void AddData(string Id, XElement element)
        {
            XDocument doc = CreateFile();
            XElement ele = GetElement(id);
            ele.Add(element);
            doc.Save(file);
        }
        private static XDocument CreateFile()
        {
            if (!Directory.Exists(dir + "\\CC_Data"))
                Directory.CreateDirectory(dir + "\\CC_Data");
            if (File.Exists(file))
            {
                XDocument doc = XDocument.Load(file);
                return doc;
            }
            else
            {
                XDocument doc = new XDocument(new XElement(root)) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
                return doc;
            }
        }
    }
}
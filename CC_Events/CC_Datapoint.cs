using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace CC_Plugin
{
    internal class Datapoint
    {
        private static string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private const string root = "Data";

        public static string GetPreviousElement(string dir)
        {
            var dinfo = new DirectoryInfo(dir);
            if (Directory.GetFiles(dir).Length > 0)
            {
                string file = dinfo.GetFiles().OrderByDescending(x => x.LastWriteTime).First().FullName;
                XDocument doc = XDocument.Load(file);
                if (doc.Root.Attribute("EleID") != null)
                {
                    return doc.Root.Attribute("EleID").Value;
                }
            }
            return "Null ID";
        }
        public static void Create(Dictionary<string, string> data, string dir)
        {
            string guid = Guid.NewGuid().ToString("N");
            string fn = dir + "\\" + guid + ".xml";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            XDocument doc = new XDocument(new XElement(root)) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
            if (data.Keys.Count > 0)
            {
                foreach (KeyValuePair<string, string> kvp in data)
                {
                    if (!string.IsNullOrEmpty(kvp.Value) && !string.IsNullOrEmpty(kvp.Key))
                    {
                        doc.Root.Add(new XAttribute(kvp.Key, kvp.Value));
                    }
                }
                doc.Save(fn);
            }
        }
    }
}

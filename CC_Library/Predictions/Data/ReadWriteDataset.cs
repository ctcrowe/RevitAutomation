using System;
using System.Collections.Generic;
using System.Linq;
using CC_Library.Datatypes;
using System.Xml.Linq;
using System.Reflection;
using System.Xml;
using System.IO;

namespace CC_Library.Predictions
{
    internal static class ReadWriteDataset
    {
        public static Dictionary<string, Data> Read(this Datatype dt, WriteToCMDLine write)
        {
            Dictionary<string, Data> dataset = new Dictionary<string, Data>();
            var assembly = typeof(ReadWriteDataset).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(z => z.Contains(dt.ToString())))
            {
                string name = assembly.GetManifestResourceNames().Where(z => z.Contains(dt.ToString())).First();
                write(name);

                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    var xdoc = new XmlDocument();
                    xdoc.Load(stream);
                    XDocument doc = xdoc.ToXDocument();
                    Datatype datatype = (Datatype)Enum.Parse(typeof(Datatype),
                        Enum.GetNames(typeof(Datatype)).Where(x => doc.Root.Name == x).First());
                    XElement root = doc.Root;

                    foreach (XElement ele in root.Elements("DataPoint"))
                    {
                        Data data = new Data(datatype, ele);
                        if(data.Label != null)
                            dataset.Add(data.Label, data);
                    }
                }
                return dataset;
            }
            write("New Dataset");
            return dataset;
        }
        public static Dictionary<string, Data> Read(this Datatype dt)
        {
            Dictionary<string, Data> dataset = new Dictionary<string, Data>();
            var assembly = typeof(ReadWriteDataset).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(z => z.Contains(dt.ToString())))
            {
                string name = assembly.GetManifestResourceNames().Where(z => z.Contains(dt.ToString())).First();

                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    var xdoc = new XmlDocument();
                    xdoc.Load(stream);
                    XDocument doc = xdoc.ToXDocument();
                    Datatype datatype = (Datatype)Enum.Parse(typeof(Datatype),
                        Enum.GetNames(typeof(Datatype)).Where(x => doc.Root.Name == x).First());
                    XElement root = doc.Root;

                    foreach (XElement ele in root.Elements("DataPoint"))
                    {
                        Data data = new Data(datatype, ele);
                        dataset.Add(data.Label, data);
                    }
                }
                return dataset;
            }
            return dataset;
        }
        public static void Write(this Dictionary<string, Data> dataset)
        {
            int elementcount = 0;
            Datatype datatype = dataset.First().Value.Datatype;
            string filename = datatype.ToString().GetMyDocs() + ".xml";
            XDocument doc = new XDocument(new XElement(datatype.ToString())) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
            foreach (KeyValuePair<string, Data> datapoint in dataset)
            {
                elementcount++;
                doc.Root.Add(datapoint.Value.Write());
            }
            doc.Save(filename);
        }
    }
}

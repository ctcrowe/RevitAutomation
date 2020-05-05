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
    internal class Dataset
    {
        public static readonly int DataSize = 5;
        public Datatype datatype { get; }
        public Dictionary<string, double[]> Data { get; }
        public Dataset(Datatype dtype)
        {
            this.datatype = dtype;
            this.Data = new Dictionary<string, double[]>();
        }
        public Dataset(XDocument doc)
        {
            this.datatype = (Datatype)Enum.Parse(typeof(Datatype),
                Enum.GetNames(typeof(Datatype)).Where(x => doc.Root.Name == x).First());
            XElement root = doc.Root;

            this.Data = new Dictionary<string, double[]>();
            foreach(XElement ele in root.Elements("DataPoint"))
            {
                string Label = string.Empty;
                double[] Values = new double[DataSize];
                if (ele.Attribute("Label") != null)
                    Label = ele.Attribute("Label").Value;
                else
                    Label = "Null";
                for (int i = 0; i < Values.Count(); i++)
                {
                    List<XElement> dps = ele.Elements("Data").Where(x => x.Attribute("Location").Value == i.ToString()).ToList();
                    if (dps.Any())
                        Values[i] = double.Parse(dps.First().Attribute("Value").Value);
                }
                Data.Add(Label, Values);
            }
        }
        public void AddEntry(string Label, Random r)
        {
            double[] values = new double[DataSize];
            for (int i = 0; i < Dataset.DataSize; i++)
            {
                double swap = r.NextDouble();
                double v = r.NextDouble();
                if (swap > 0.50)
                    values[i] = v;
                else
                    values[i] = -v;
            }
            Data.Add(Label, values);
        }
        public void WriteToXML()
        {
            int elementcount = 0;
            string filename = this.datatype.ToString().GetMyDocs() + ".xml";
            XDocument doc = new XDocument(new XElement(this.datatype.ToString())) { Declaration = new XDeclaration("1.0", "utf-8", "yes") };
            foreach(KeyValuePair<string, double[]> datapoint in Data)
            {
                elementcount++;
                XElement ele = new XElement("DataPoint");
                ele.Add(new XAttribute("Label", datapoint.Key));
                for (int i = 0; i < datapoint.Value.Count(); i++)
                {
                    XElement dp = new XElement("Data");
                    dp.Add(new XAttribute("Location", i));
                    dp.Add(new XAttribute("Value", datapoint.Value[i]));

                    ele.Add(dp);
                }
                doc.Root.Add(ele);
            }
            doc.Save(filename);
        }
    }
    internal static class StaticLoadDataset
    {
        public static Dataset LoadDataset(this Datatype dt, WriteToCMDLine write)
        {
            var assembly = typeof(StaticLoadDataset).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(z => z.Contains(dt.ToString())))
            {
                string name = assembly.GetManifestResourceNames().Where(z => z.Contains(dt.ToString())).First();
                write(name);

                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    var xdoc = new XmlDocument();
                    xdoc.Load(stream);
                    XDocument doc = xdoc.ToXDocument();
                    return new Dataset(doc);
                }
            }
            write("New Dataset");
            return new Dataset(dt);
        }
        public static Dataset LoadDataset(this Datatype dt)
        {
            var assembly = typeof(StaticLoadDataset).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(z => z.Contains(dt.ToString())))
            {
                string name = assembly.GetManifestResourceNames().Where(z => z.Contains(dt.ToString())).First();

                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    var xdoc = new XmlDocument();
                    xdoc.Load(stream);
                    XDocument doc = xdoc.ToXDocument();
                    return new Dataset(doc);
                }
            }
            return new Dataset(dt);
        }
        public static void InitializeDataset(this Dataset ds, Dictionary<string, string> EntrySet, Random random, WriteToCMDLine write)
        {
            if(ds.datatype == Datatype.TextData)
            {
                foreach (string key in EntrySet.Keys)
                {
                    foreach (string section in key.SplitTitle())
                    {
                        if (!ds.Data.ContainsKey(section))
                        {
                            ds.AddEntry(section, random);
                        }
                    }
                }
            }
            else
            {
                foreach(string value in EntrySet.Values)
                {
                    if(!ds.Data.ContainsKey(value))
                    {
                        ds.AddEntry(value, random);
                    }
                }
            }
            write(ds.datatype.ToString() + " has " + ds.Data.Count() + " Entries.");
        }
        public static KeyValuePair<string, double[]> GeneratePoint(this Datatype dt)
        {
            string key = "Null";
            double[] values = new double[Dataset.DataSize];
            Random r = new Random();
            for(int i = 0; i < Dataset.DataSize; i++)
            {
                double swap = r.NextDouble();
                double v = r.NextDouble();
                if (swap > 0.50)
                    values[i] = v;
                else
                    values[i] = -v;
            }
            return new KeyValuePair<string, double[]>(key, values);
        }
    }
}
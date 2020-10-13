using CC_Library.Datatypes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace CC_Library.Predictions
{
    internal static class LoadElements
    {
        public static Dictionary<string, Element>[] CloneData(this Dictionary<string, Element>[] eles)
        {
            Dictionary<string, Element>[] Copy = new Dictionary<string, Element>[eles.Count()];
            for(int i = 0; i < eles.Count(); i++)
            {
                Copy[i] = new Dictionary<string, Element>();
            }
            for (int i = 0; i < eles.Count(); i++)
            {
                foreach (var e in eles[i])
                {
                    Copy[i].Add(e.Key, e.Value.Clone());
                }
            }
            return Copy;
        }
        public static Element Clone(this Element ele)
        {
            Element newele = new Element(ele.datatype, ele.Label);
            double[] Loc = new double[ele.Location.Count()];
            int[] Ref = new int[ele.Location.Count()];
            for(int i = 0; i < Loc.Count(); i++)
            {
                Loc[i] = ele.Location[i];
            }
            newele.Location = Loc;
            return newele;
        }
        public static List<Element> GetElements(this Datatype datatype, List<Entry> entries)
        {
            List<Element> eles = new List<Element>();
            foreach (Entry e in entries)
            {
                foreach (string v in e.Values)
                {
                    if (!eles.Any(x => x.Label == v))
                        eles.Add(datatype.GetElement(v));
                }
            }
            return eles;
        }
        public static List<Element> GetElements(this Datatype datatype)
        {
            List<Element> eles = new List<Element>();
            string Folder = datatype.ToString().GetMyDocs();
            if (Directory.Exists(Folder))
            {
                string[] Files = Directory.GetFiles(Folder);
                if (Files.Any(x => x.Split('\\').Last().Split('_').First() == datatype.ToString()))
                {
                    var xdocs = Files.Where(x => x.Split('\\').Last().Split('_').First() == datatype.ToString());
                    for (int i = 0; i < xdocs.Count(); i++)
                    {
                        eles.Add(new Element(XDocument.Load(xdocs.ElementAt(i))));
                    }
                }
            }

            var assembly = typeof(ReadWriteXML).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(x => x.Contains(datatype.ToString() + "_")))
            {
                foreach (string name in assembly.GetManifestResourceNames().Where(x => x.Contains(datatype.ToString() + "_")))
                    using (Stream stream = assembly.GetManifestResourceStream(name))
                    {
                        var ele = new Element(XDocument.Load(stream));
                        if (!eles.Any(x => x.Label == ele.Label))
                            eles.Add(ele);
                    }
            }
            return eles;
        }
        public static Element GetElement(this Datatype datatype, string ElementName)
        {
            var assembly = typeof(ReadWriteXML).GetTypeInfo().Assembly;
            string Folder = datatype.ToString().GetMyDocs();
            if (Directory.Exists(Folder))
            {
                string[] Files = Directory.GetFiles(Folder);
                if (Files.Any(x => x.Split('\\').Last().Split('.').First() == datatype.ToString() + "_" + ElementName))
                {
                    var xdoc = XDocument.Load(Files.Where(x => x.Split('\\').Last().Split('.').First() == datatype.ToString() + "_" + ElementName).First());
                    return new Element(xdoc);
                }
            }
            if (assembly.GetManifestResourceNames().Any(x => x.Contains(datatype.ToString() + "_" + ElementName)))
            {
                string name = assembly.GetManifestResourceNames().Where(z => z.Contains(datatype.ToString() + "_" + ElementName)).First();
                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    var xdoc = XDocument.Load(stream);
                    return new Element(xdoc);
                }
            }
            Element ele = new Element(datatype, ElementName);
            ele.Write();
            return ele;
        }
    }
}

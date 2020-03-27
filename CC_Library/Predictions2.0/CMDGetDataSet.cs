using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Reflection;
using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    //Used to access the datafiles embedded in the dll file from the Enum
    internal static class CMDGetDataSet
    {
        public static List<Data> GetDataSet(this Datatype dt)
        {
            var data = new List<Data>();

            var assembly = typeof(CMDGetDataSet).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(x => x.Contains(dt.ToString())))
            {
                string name = assembly.GetManifestResourceNames().Where(y => y.Contains(dt.ToString())).First();
                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    var xdoc = new XmlDocument();
                    xdoc.Load(stream);
                    XDocument doc = xdoc.ToXDocument();
                    foreach(XElement ele in doc.Root.Elements())
                        data.Add(ele.CreateDataPoint());
                }
            }
            return data;
        }
    }
}
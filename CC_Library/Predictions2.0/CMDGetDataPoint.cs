using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Reflection;

using CC_Library.Datatypes;

namespace CC_Library.Predictions
{
    internal static class GetDataPoint
    {
        public static DataPt ToDataPoint(this string s)
        {
            var assembly = typeof(CMDGetDataSet).GetTypeInfo().Assembly;
            if (assembly.GetManifestResourceNames().Any(x => x.Contains(Datatype.TextData.ToString())))
            {
                string name = assembly.GetManifestResourceNames().Where(y => y.Contains(Datatype.TextData.ToString())).First();
                using (Stream stream = assembly.GetManifestResourceStream(name))
                {
                    var xdoc = new XmlDocument();
                    xdoc.Load(stream);
                    XDocument doc = xdoc.ToXDocument();
                    foreach (XElement ele in doc.Root.Elements())
                        if (ele.Attribute("PHRASE").Value == s)
                            return ele.CreateDataPoint();
                }
            }
            return new DataPt(s);
        }
    }
}

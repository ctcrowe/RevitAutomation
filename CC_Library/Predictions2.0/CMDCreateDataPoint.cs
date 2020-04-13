using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Reflection;

namespace CC_Library.Predictions
{
    internal static class CMDCreateDataPoint
    {
        public static DataPt CreateDataPoint(this XElement ele)
        {
            DataPt d = new DataPt(ele.Attribute("PHRASE").Value);
            foreach(XElement e in ele.Elements())
            {
                int number = int.Parse(e.Attribute("NUMBER").Value);
                int value = int.Parse(e.Attribute("VALUE").Value);

                d.SetValue(number, value);
            }
            return d;
        }
    }
}

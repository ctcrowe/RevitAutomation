using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CC_Library.Predictions
{
    internal static class XmlMoveElement
    {
        public static void Move
            (this XDocument Datum
            , double[] LengthVector)
        {
            for (int i = 0; i < LengthVector.Count(); i++)
            {
                if (Datum.Root.Elements("Data").Any(x => x.Attribute("Location").Value == i.ToString()))
                {
                    XElement ele = Datum.Root.Elements("Data").Where(x => x.Attribute("Location").Value == i.ToString()).First();
                    if (LengthVector[i] + double.Parse(ele.Attribute("Value").Value) <= 1 && LengthVector[i] + double.Parse(ele.Attribute("Value").Value) >= -1)
                        ele.Attribute("Value").Value = (LengthVector[i] + double.Parse(ele.Attribute("Value").Value)).ToString();
                    else
                    {
                        if (LengthVector[i] + double.Parse(ele.Attribute("Value").Value) >= 1)
                            ele.Attribute("Value").Value = "1";
                        if (LengthVector[i] + double.Parse(ele.Attribute("Value").Value) <= -1)
                            ele.Attribute("Value").Value = "-1";
                    }
                }
            }
        }
    }
}

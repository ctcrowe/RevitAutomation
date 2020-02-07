using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System;
using System.Reflection;

namespace CC_Library
{
    internal class PredOption
    {
        public string Name { get; }
        public double Adjustment { get; set; }
        public double Weight { get; set; }
        public int Count { get; set; }
            
        public PredOption(string w)
        {
            this.Name = w;
            this.Adjustment = 0;
            this.Weight = 1;
            this.Count = 1;
        }
        public PredOption(XElement ele)
        {
            this.Name = ele.Attribute("NAME").Value;
            this.Adjustment = double.Parse(ele.Attribute("ADJUSTMENT").Value);
            this.Weight = double.Parse(ele.Attribute("WEIGHT").Value);
            this.Count = int.Parse(ele.Attribute("QTY").Value);
        }
        public XElement CreateOption()
        {
            XElement ele = new XElement("OPTION");
            ele.Add(new XAttribute("NAME", this.Name));
            ele.Add(new XAttribute("ADJUSTMENT", this.Adjustment.ToString()));
            ele.Add(new XAttribute("WEIGHT", this.Weight.ToString());
            ele.Add(new XAttribute("QTY", this.Count.ToString()));
            return ele;
        }
    }
}

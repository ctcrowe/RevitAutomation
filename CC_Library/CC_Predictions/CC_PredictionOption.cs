using System.Xml.Linq;
using System;

namespace CC_Library
{
    internal class PredictionOption
    {
        public string Name { get; }
        public int Positive { get; set; }
        public int Negative { get; set; }
            
        public PredictionOption(string w)
        {
            this.Name = w;
            this.Positive = 0;
            this.Negative = 0;
        }
        public PredictionOption(XElement ele)
        {
            this.Name = ele.Attribute("NAME").Value;
            this.Positive = int.Parse(ele.Attribute("POSITIVE").Value);
            this.Negative = int.Parse(ele.Attribute("NEGATIVE").Value);
        }
        public XElement CreateXML()
        {
            XElement ele = new XElement("OPTION");
            ele.Add(new XAttribute("NAME", this.Name));
            ele.Add(new XAttribute("POSITIVE", this.Positive.ToString()));
            ele.Add(new XAttribute("NEGATIVE", this.Negative.ToString()));
            return ele;
        }
        public void AddPositive() { Positive++; }
        public void AddNegative() { Negative++; }
        public double CalcAdjustment(PredictionElement pe)
        {
            double adj = 0;
            if (Negative > Positive)
                adj = -1;
            else
                adj = 1;
            double v = Math.Abs(Positive - Negative);
            double v2 = v * v;
            double c = Positive + Negative;
            double c2 = c * c;
            double r = Math.Sqrt(v2 / c2) * adj * pe.Weight;
            return r;
        }
    }
}

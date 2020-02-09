using System.Xml.Linq;

namespace CC_Library
{
    internal class PredictionOption
    {
        public string Name { get; }
        public double Ratio { get; set; }
        public double Weight { get; set; }
        public int Count { get; set; }
            
        public PredictionOption(string w)
        {
            this.Name = w;
            this.Ratio = 0;
            this.Weight = 1;
            this.Count = 1;
        }
        public PredictionOption(XElement ele)
        {
            this.Name = ele.Attribute("NAME").Value;
            this.Ratio = double.Parse(ele.Attribute("RATIO").Value);
            this.Weight = double.Parse(ele.Attribute("WEIGHT").Value);
            this.Count = int.Parse(ele.Attribute("QTY").Value);
        }
        public XElement CreateOption()
        {
            XElement ele = new XElement("OPTION");
            ele.Add(new XAttribute("NAME", this.Name));
            ele.Add(new XAttribute("RATIO", this.Ratio.ToString()));
            ele.Add(new XAttribute("WEIGHT", this.Weight.ToString()));
            ele.Add(new XAttribute("QTY", this.Count.ToString()));
            return ele;
        }
        public void AdjustCount() { Count++; }
        public void SetRatio(int i) { this.Ratio = Count / i; }
    }
}

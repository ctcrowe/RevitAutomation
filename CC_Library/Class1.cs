
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System;
using System.Reflection;

namespace CC_Library
{
    /*
    public PredictionElement(string w)
    {
        this.Word = w;
        this.Options = new List<PredOption>();
    }
    public PredOption(string w)
    {
        this.Name = w;
        this.Adjustment = 0;
        this.Weight = 1;
        this.Count = 1;
    }
    public Prediction(string s)
    {
        this.Name = s;
        this.Weight = 0;
        this.Count = 0;
    }
    */
    internal class PredictionPhrase
    {
        public string Predict()
        {
            foreach(PredictionElement e in Elements)
            {
                foreach(PredOption o in e.Options)
                {
                    if(!this.Options.Any(x => x.Name == o.Name))
                    {
                        this.Options.Add(new Prediction(o.Name));
                    }
                    double val = o.Weight * o.Adjustment;
                    this.Options.Where(x => x.Name == o.Name).First().Adjust(val);
                }
            }
            double Max = 0;
            foreach(Prediction p in this.Options)
            {
                if(p.Weight > Max)
                    May = p.Weight;
            }
            string s = Options.Where(x => x.Weight == Max).First().Name;
            this.Prediction = s;
            return s;
        }
        
        public string Phrase { get; }
        public List<PredictionElement> Elements { get; set; }
        public List<Prediction> Options { get; set; }
        public string Prediction { get; set; }
                
        public PredictionPhrase(string p)
        {
            this.Phrase = p;
            this.Elements = GetElements();
            this.Options = new List<Prediction>();
            this.
        }
        private List<PredictionElement> GetElements()
        {
            var Elements = new List<PredictionElement>();
            int b = 0;
            char[] cs = Phrase.ToCharArray();
            for (int i = 1; i < cs.Count(); i++)
            {
                if (!char.IsLetter(cs[i]))
                {
                    if (i > b && b < cs.Count())
                    {
                        string z = string.Empty;
                        for (int j = b; j < i; j++)
                        {
                            z += cs[j];
                        }
                        Elements.Add(new PredictionElement(z));
                    }
                    b = i + 1;
                }
                else
                {
                    if (char.IsUpper(cs[i]) && !char.IsUpper(cs[i - 1]))
                    {
                        if (i > b && b < cs.Count())
                        {
                            string z = string.Empty;
                            for (int j = b; j < i; j++)
                            {
                                z += cs[j];
                            }
                            Elements.Add(new PredictionElement(z));
                        }
                        b = i;
                    }
                }
            }
            return Elements;
        }
    }
    internal class Prediction
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        private int Count { get; set; }
        
        public Prediction(string s)
        {
            this.Name = s;
            this.Weight = 0;
            this.Count = 0;
        }
        public void Adjust(double d)
        {
            double x = this.Weight * this.Count;
            this.Count++;
            x += d;
            this.Weight = x / this.Count;
        }
    }
    internal class PredictionElement
    {
        public string Word { get; }
        internal List<PredOption> Options { get; set; }
        
        public PredictionElement(string w)
        {
            this.Word = w;
            this.Options = new List<PredOption>();
        }
        public PredictionElement(XElement ele)
        {
            this.Word = ele.Attribute("WORD").Value;
            this.Options = new List<PredOption>();
            foreach(XElement e in ele.Elements("OPTION"))
            {
                if(!Options.Any(x => x.Name == e.Attribute("NAME").Value))
                    Options.Add(new PredOption(e));
            }
        }
        public XElement CreateElement()
        {
            XElement ele = new XElement("ELEMENT");
            ele.Add(new XAttribute("WORD", Word));
            foreach(PredOption p in Options)
            {
                ele.Add(p.CreateOption());
            }
            return ele;
        }
        public void AddOption(string s)
        {
            if(!Options.Any(x => x.Name == s))
                Options.Add(new PredOption(s));
        }
    }
}

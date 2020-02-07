
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
                foreach(PredictionOption o in e.Options)
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
                    Max = p.Weight;
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
            this.Elements = new List<PredictionElement>();
            foreach(string s in SplitTitle.Run(p))
            {
                if (!Elements.Any(x => x.Word == s))
                    Elements.Add(new PredictionElement(s));
            }
            this.Options = new List<Prediction>();
            this.Prediction = 0.ToString();
        }
    }
}
